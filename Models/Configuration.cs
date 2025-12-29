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

        public static bool IsFeeWhatsappEnabled()
        {
            bool v;
            if (bool.TryParse(ConfigurationManager.AppSettings["FeeWhatsapp:Enabled"], out v))
                return v;
            return false;
        }

        public static int GetFeeWhatsappDefaultQueueMaxRecipients()
        {
            int v;
            if (int.TryParse(ConfigurationManager.AppSettings["FeeWhatsapp:DefaultQueueMaxRecipients"], out v))
                return v;
            return 200;
        }

        public static int GetFeeWhatsappDefaultSendBatchSize()
        {
            int v;
            if (int.TryParse(ConfigurationManager.AppSettings["FeeWhatsapp:DefaultSendBatchSize"], out v))
                return v;
            return 25;
        }

        public static string GetFeeWhatsappProviderBaseUrl()
        {
            return ConfigurationManager.AppSettings["FeeWhatsapp:ProviderBaseUrl"];
        }

        public static string GetFeeWhatsappProviderToken()
        {
            return ConfigurationManager.AppSettings["FeeWhatsapp:ProviderToken"];
        }
    }
}