using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace SchoolErpAPI.Models
{
    public class Configuration
    {
        #region GetConnectionString

        public static string GetConnectionString()
        {
            //return the connection string
            return ConfigurationManager.ConnectionStrings["schoolerp"].ConnectionString;
        }

        #endregion
    }
}