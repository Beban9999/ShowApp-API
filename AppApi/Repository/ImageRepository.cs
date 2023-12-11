using AppApi.Repository.Contract;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using AppApi.Helper;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AppApi.Models;

namespace AppApi.Repository
{
    public class ImageRepository : IImageRepository
    {
        public DbHelper _dbHelper;
        public ImageRepository(DbHelper dbHelper) 
        {
            _dbHelper = dbHelper;
        }

        //ako nam ova uopste treba, ako cemo to zvaci preko GetPost
        public byte[] Get_Image(int imageId)
        {
            byte[] image;
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@ImageID", imageId)
            };

            DataTable dt = _dbHelper.ExecProc(parameters, "GetImage");
            DataRow dr = dt.Rows[0];
            image = (byte[])dr["ImageData"];

            return image;
        }

        public RequestResponse UploadImage(List<IFormFile> imageData, int postId)
        {
            RequestResponse response = new RequestResponse();

            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                if (imageData != null && imageData.Count != 0)
                {
                    foreach (var image in imageData)
                    {
                        parameters.Add(new SqlParameter
                        {
                            ParameterName = "@ImageData",
                            SqlDbType = SqlDbType.VarBinary,
                            Size = -1,
                            Value = _dbHelper.ReadFully(image.OpenReadStream())
                        });

                        parameters.Add(new SqlParameter
                        {
                            ParameterName = "@FileName",
                            SqlDbType = SqlDbType.NVarChar,
                            Value = image.FileName
                        });

                        parameters.Add(new SqlParameter
                        {
                            ParameterName = "@PostId",
                            SqlDbType = SqlDbType.Int,
                            Value = postId
                        });

                        int resp = _dbHelper.ExecProcReturnScalar(parameters, "UploadImage");
                        if (resp == 1)
                        {
                            response.IsSuccessfull = true;
                        }
                        else
                        {
                            response.IsSuccessfull = false;
                            response.ErrorMessage = "Image not inserted!";
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                response.IsSuccessfull = false;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    
    }
}
