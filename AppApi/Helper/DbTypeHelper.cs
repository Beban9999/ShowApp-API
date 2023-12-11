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
    }
}
