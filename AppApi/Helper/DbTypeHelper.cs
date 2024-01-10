using System.Data;

namespace AppApi.Helper
{
    public class DbTypeHelper
    {
        public static string? GetString(DataRow row, string columnName)
        {
            bool isNull = row.IsNull(columnName);
            return isNull ? string.Empty : row.Field<string>(columnName);
        }

        public static int GetInt(DataRow row, string columnName)
        {
            bool columnIsNull = row.IsNull(columnName);

            int result = 0;

            if (columnIsNull)
            {
                result = -1;
            }
            else
            {
                try
                {
                    result = row.Field<Int32>(columnName);
                }
                catch (Exception)
                {
                    result = row.Field<Int16>(columnName);
                }
            }

            return result;
        }

        public static bool GetBool(DataRow row, string columnName)
        {
            bool returnValue = false;

            bool columnIsNull = row.IsNull(columnName);
            if (!columnIsNull)
            {
                try
                {
                    returnValue = (row.Field<Int32>(columnName) == 1) ? true : false;
                }
                catch (InvalidCastException)
                {
                    if (row[columnName].ToString().ToLower() == "true")
                    {
                        returnValue = true;
                    }
                    else if (row[columnName].ToString() == "1")
                    {
                        returnValue = true;
                    }
                    else
                    {
                        returnValue = false;
                    }
                }
            }

            return returnValue;
        }

        public static decimal GetDecimal(DataRow row, string column)
        {
            bool isNull = row.IsNull(column);
            return isNull ? 0 : Convert.ToDecimal(row[column]);
        }
    }
}
