using AppApi.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AppApi.Helper
{
    public class DbHelper
    {
        private readonly string _connectionString;
        public DbHelper(IConfiguration configuration) 
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public DataTable ExecProcs(List<SqlParameter> parameters, string procedure)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        SqlDataAdapter sqla = new SqlDataAdapter(command);
                        command.CommandText = procedure;
                        command.CommandType = CommandType.StoredProcedure;

                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        sqla.Fill(dt);
                    }
                }
                catch (Exception ex)
                {
                    //logger
                    return dt;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }

            return dt;
        }

        public int ExecProcReturnScalar(List<SqlParameter> parameters, string procedure)
        {
            int response = 0;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = procedure;
                        command.CommandType = CommandType.StoredProcedure;

                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        response = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
                catch(Exception ex) 
                {
                    //log db error
                }
                finally
                {
                    if(connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }

            return response;
        }

        public DataSet ExecDsProc(List<SqlParameter> parameters, string procedure)
        {
            DataSet ds = new DataSet();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        SqlDataAdapter sqlda = new SqlDataAdapter(command);
                        command.CommandText = procedure;
                        command.CommandType = CommandType.StoredProcedure;

                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }

                        sqlda.Fill(ds);
                    }
                }
                catch (Exception ex)
                {
                    //logger
                    return ds;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }

            return ds;
        }


        public byte[] ReadFully(Stream input)
        {
            using (var memoryStream = new MemoryStream())
            {
                input.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

    }
}
