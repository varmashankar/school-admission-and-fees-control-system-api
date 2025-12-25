using System.Data.SqlClient;

namespace SchoolErpAPI.Models
{
    public class DBConnection
    {

        #region GlobalConnection
        public static SqlConnection GlobalConnection()
        {
            try
            {
                SqlConnection conn = new SqlConnection(Configuration.GetConnectionString());
                return conn;
            }
            catch
            {
                throw;
            }
        }

        #endregion

    }
}