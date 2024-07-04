using System.Data;

namespace AppApi.Helper
{
    public class DbTypeHelper
    {
        public static string? GetString(DataRow row, string columnName)
        {
            bool isNull = row.IsNull(columnName);
            return isNull ? string.Empty : row[columnName].ToString();
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

        public static long GetLong(DataRow row, string columnName)
        {
            bool columnIsNull = row.IsNull(columnName);

            long result = 0;

            if (columnIsNull)
            {
                result = -1;
            }
            else
            {
                try
                {
                    result = row.Field<Int64>(columnName);
                }
                catch (Exception)
                {
                    result = row.Field<Int64>(columnName);
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

        public static DateOnly GetDateOnly(DataRow row, string columnName, DateOnly defaultValue = default)
        {
            if (row.IsNull(columnName))
            {
                return defaultValue;
            }

            try
            {
                return DateOnly.FromDateTime(row.Field<DateTime>(columnName));
            }
            catch (InvalidCastException)
            {
                try
                {
                    return DateOnly.FromDateTime(Convert.ToDateTime(row[columnName]));
                }
                catch
                {
                    return defaultValue;
                }
            }
        }
    }

}
