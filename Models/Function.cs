using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;

namespace SchoolErpAPI.Models
{
    public class Function
    {
        #region CreateToken

        // This method creates a token for user authentication based on the login type and user ID
        public static string CreateToken(int loginAs, int? id)
        {
            try
            {
                byte[] time = BitConverter.GetBytes(DateTime.Now.AddMonths(1).ToBinary()); // Get the current time and convert it to a byte array
                byte[] key = System.Guid.NewGuid().ToByteArray(); // Generate a new GUID and convert it to a byte array
                string timeToken = Convert.ToBase64String(time.Concat(key).ToArray()); // Combine the time and key byte arrays, then convert to a Base64 string


                string token = timeToken.Substring(0, 32 / 4) + "#" + loginAs + "#" + timeToken.Substring(32 / 4, 32 / 2) + String.Format("{0:D7}", id) + timeToken.Substring(3 * (32 / 4)); // Create the token string by concatenating the time token, login type, and user ID

                // Encrypt the token by incrementing each byte value by 2
                byte[] asciiBytes = Encoding.ASCII.GetBytes(token);
                for (int i = 0; i < asciiBytes.Length; i++)
                {
                    ++asciiBytes[i]; // Increment each byte value by 1
                    ++asciiBytes[i]; // Increment each byte value by 1 again
                }
                var cookie = System.Text.Encoding.Default.GetString(asciiBytes); // Convert the modified byte array back to a string

                return cookie; // Return the encrypted token string eg. "201#loginAs#timeToken#userId#timeToken"
            }
            catch
            {
                return "201"; // If an error occurs, return an error code "201"
            }
        }
        #endregion

        #region decryptToken

        // This method decrypts a token string and extracts the login type and user ID
        public TokenResponse decryptToken(string token)
        {
            TokenResponse tokenResponse = new TokenResponse(); // Create a new instance of TokenResponse to hold the extracted information

            try
            {
                if (!String.IsNullOrEmpty(token)) // Check if the token is not null or empty
                {
                    byte[] asciiBytes = Encoding.ASCII.GetBytes(token); // Convert the token string to a byte array using ASCII encoding
                    for (int i = 0; i < asciiBytes.Length; i++)
                    {
                        --asciiBytes[i]; // Decrypt the token by decrementing each byte value by 2
                        --asciiBytes[i]; // Decrement each byte value by 2 again
                    }

                    string str = System.Text.Encoding.Default.GetString(asciiBytes); // Convert the modified byte array back to a string

                    string timeTokenP1 = str.Substring(0, 32 / 4); // Extract the first part of the token (timeTokenP1) from the string
                    string tokenP2 = str.Substring(32 / 4); // Extract the second part of the token (tokenP2) from the string

                    string loginAs = ""; // Initialize the loginAs variable to hold the login type

                    string hashtoken = tokenP2.Substring(1); // Remove the first character from tokenP2 to get hashtoken

                    if (tokenP2.StartsWith("#")) // Check if tokenP2 starts with a hash character
                        loginAs = hashtoken.Substring(0, hashtoken.IndexOf("#")); // Extract the login type from hashtoken by finding the first hash character

                    str = tokenP2; // Reassign str to tokenP2 for further processing
                    str = hashtoken.Substring(hashtoken.IndexOf("#") + 1); // Remove the login type from hashtoken to get the remaining part of the token
                    string timeTokenP2 = str.Substring(0, 32 / 2); // Extract the second part of the token (timeTokenP2) from the string

                    string loginId = str.Replace(timeTokenP2, "").Substring(0, 7);// Extract the user ID from the remaining part of the token by removing timeTokenP2 and taking the first 7 characters
                    string timeTokenP3 = str.Replace(loginId, "");// Extract the third part of the token (timeTokenP3) by removing the user ID from the remaining part of the token

                    string timeToken = timeTokenP1 + timeTokenP2 + timeTokenP3; // Combine all parts of the token to get the complete time token

                    loginId = loginId.TrimStart('0'); // Remove leading zeros from the user ID

                    int? exsists = 0; // Initialize a variable to hold the existence status of the user ID
                    tokenResponse.loginAs = loginAs; // Set the login type in the token response
                    tokenResponse.userId = loginId; // Set the user ID in the token response
                }
                else
                {
                    tokenResponse.msg = "INVALID:NOTOKEN"; // If the token is null or empty, set the message in the token response
                }

                return tokenResponse; // Return the token response containing the extracted information

            }
            catch
            {
                tokenResponse.msg = "INVALID:ERROR"; // If an error occurs during decryption, set the message in the token response
                return tokenResponse; // Return the token response with the error message
            }
        }
        #endregion

        #region getAllController

        //find all controllers and actions in the current assembly and save them to the database
        public static string getAllController()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var controlleractionlist = asm.GetTypes() // Get all types in the assembly
                    .Where(type => typeof(ApiController).IsAssignableFrom(type))
                    .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Select(x => new // Create a new anonymous object for each action
                    {
                        Controller = x.DeclaringType.Name, // Get the controller name
                        Action = x.Name, // Get the action name
                        Area = x.DeclaringType.CustomAttributes.Where(c => c.AttributeType == typeof(ApiController)) // Get the area attribute if it exists

                    }).ToList();
            var list = new List<ControllerActions>(); // Create a list to hold the controller actions
            foreach (var item in controlleractionlist) //Iterate through each action in the list
            {
                if (item.Area.Count() != 0) // Check if the area attribute exists
                {
                    list.Add(new ControllerActions() // Add a new ControllerActions object to the list
                    {
                        Controller = item.Controller, // Set the controller name
                        Action = item.Action, // Set the action name
                        Area = item.Area.Select(v => v.ConstructorArguments[0].Value.ToString()).FirstOrDefault() // Set the area name if it exists
                    });
                }
                else
                {
                    list.Add(new ControllerActions() // If the area attribute does not exist, add a new ControllerActions object without the area
                    {
                        Controller = item.Controller, // Set the controller name
                        Action = item.Action, // Set the action name
                        Area = null, // Set the area name to null
                    });
                }
            }

            //API Calling
            List<ControllerActions> actionsList = list.OfType<ControllerActions>().ToList(); // Convert the list to a list of ControllerActions objects

            DataTable dtActionsList = new DataTable(); // Create a new DataTable to hold the controller actions

            var actions = actionsList.Select(a => new
            {
                controller_name = a.Controller,
                action_name = a.Action
            }).ToList();// Select the controller and action names from the list of ControllerActions objects

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(actions); // Serialize the list of controller actions to JSON
            dtActionsList = JsonConvert.DeserializeObject<DataTable>(json); // Deserialize the JSON to a DataTable

            SqlConnection con = DBConnection.GlobalConnection(); // Get the global database connection
            SqlCommand cmd = new SqlCommand("saveActionNames", con); // Create a new SqlCommand to execute the stored procedure
            cmd.CommandType = CommandType.StoredProcedure;

            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"); // Get the Indian Standard Time zone
            DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), INDIAN_ZONE); // Convert the current UTC time to Indian Standard Time

            cmd.Parameters.AddWithValue("@creationTimestamp", datetime); // Add the creation timestamp parameter to the command
            cmd.Parameters.AddWithValue("@temp", dtActionsList); // Add the DataTable of controller actions as a parameter to the command

            con.Open();
            cmd.ExecuteNonQuery().ToString();
            con.Close();

            return null;
        }

        #endregion

        #region Bind/Map DataTable
        /// Bind DataTable to a List of Objects
        public static T BindData<T>(DataRow dr, List<string> columns)
        {
            // Create object
            var ob = Activator.CreateInstance<T>();

            // Get all fields
            var fields = typeof(T).GetFields();
            foreach (var fieldInfo in fields)
            {
                if (columns.Contains(fieldInfo.Name))
                {
                    fieldInfo.SetValue(ob, dr[fieldInfo.Name]);
                }
            }

            DefaultContractResolver ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            var properties = typeof(T).GetProperties();

            // NOTE: legacy direct assignment removed; conversion-aware mapping below handles strings, nullables, enums.

            foreach (var propertyInfo in properties)
            {
                string name = ContractResolver.GetResolvedPropertyName(propertyInfo.Name);
                if (!columns.Contains(name))
                    continue;

                object raw = dr[name];
                if (raw == null || raw == DBNull.Value)
                    continue;

                var targetType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                try
                {
                    if (targetType == typeof(string))
                    {
                        propertyInfo.SetValue(ob, Convert.ToString(raw));
                        continue;
                    }

                    object converted;
                    if (targetType.IsEnum)
                    {
                        converted = Enum.Parse(targetType, Convert.ToString(raw), true);
                    }
                    else
                    {
                        converted = Convert.ChangeType(raw, targetType);
                    }

                    propertyInfo.SetValue(ob, converted);
                }
                catch (Exception ex)
                {
                    string srcType = raw == null ? "<null>" : raw.GetType().FullName;
                    string dstType = propertyInfo.PropertyType == null ? "<null>" : propertyInfo.PropertyType.FullName;
                    string msg = "BindData mapping failed. Model=" + typeof(T).FullName + ", Column=" + name + ", ValueType=" + srcType + ", Property=" + propertyInfo.Name + ", PropertyType=" + dstType + ", Value=" + Convert.ToString(raw) + ". Error=" + ex.Message;
                    throw new InvalidOperationException(msg, ex);
                }
            }

            return ob;
        }

        #endregion

        #region addClassAttributes
        // Add class attributes to SqlCommand 
        public void addClassAttributes<T>(ref SqlCommand cmd, object data)
        {
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                if (prop.PropertyType.FullName.StartsWith("System.Collections.Generic.List"))
                    continue;

                object value = prop.GetValue(data);

                cmd.Parameters.AddWithValue(
                    "@" + prop.Name,
                    value ?? DBNull.Value
                );
            }
        }


        public void addClassAttributes<T>(ref SqlDataAdapter Adp, Object dataString) // Add class attributes to SqlDataAdapter
        {
            var properties = typeof(T).GetProperties(); // Get all properties of the class T

            List<string> columnList = new List<string>(); // Create a list to store column names

            foreach (var propertyInfo in properties)  // Iterate through each property in the class T
            {
                string name = propertyInfo.Name; // Get the property name

                if (name != "createdById" && !columnList.Contains(name + "Id")) // Check if the property is not already added
                    Adp.SelectCommand.Parameters.AddWithValue("@" + name, propertyInfo.GetValue(dataString)); // Add the property value as a parameter to the SqlDataAdapter's SelectCommand

                columnList.Add(name); // Add the property name to the column list to avoid duplicates
            }

        }

        #endregion

        #region addDefaultSPOutput 
        public void addDefaultSPOutput(ref SqlCommand cmd)
        {
            // Add output parameters to the SqlCommand for default stored procedure response
            cmd.Parameters.Add("@outputId", SqlDbType.Int);
            cmd.Parameters["@outputId"].Direction = ParameterDirection.Output; // Output parameter for ID

            // Add output parameters for message and execution status
            cmd.Parameters.Add("@message", SqlDbType.NVarChar, 500);
            cmd.Parameters["@message"].Direction = ParameterDirection.Output;

            // Add output parameter for execution status
            cmd.Parameters.Add("@executionStatus", SqlDbType.NVarChar, 500);
            cmd.Parameters["@executionStatus"].Direction = ParameterDirection.Output;

        }

        #endregion

        #region getDefaultSPOutput
        public SPResponse getDefaultSPOutput(SqlCommand cmd, SqlConnection con) // Execute the SqlCommand and return a response object
        {
            SPResponse response = new SPResponse();

            try
            {
                if (con == null) throw new ArgumentNullException("con");

                if (con.State != ConnectionState.Open)
                    con.Open();

                cmd.ExecuteNonQuery();

                object msgObj = cmd.Parameters.Contains("@message") ? cmd.Parameters["@message"].Value : null;
                object outIdObj = cmd.Parameters.Contains("@outputId") ? cmd.Parameters["@outputId"].Value : null;
                object execObj = cmd.Parameters.Contains("@executionStatus") ? cmd.Parameters["@executionStatus"].Value : null;

                response.message = (msgObj == null || msgObj == DBNull.Value) ? string.Empty : Convert.ToString(msgObj);
                response.id = (outIdObj == null || outIdObj == DBNull.Value) ? (int?)null : Convert.ToInt32(outIdObj);
                response.executionStatus = (execObj == null || execObj == DBNull.Value) ? string.Empty : Convert.ToString(execObj);
            }
            catch (Exception ex)
            {
                response.executionStatus = "FALSE";
                response.message = ex.Message;
                response.id = null;
            }
            finally
            {
                try
                {
                    if (con != null && con.State == ConnectionState.Open)
                        con.Close();
                }
                catch { }
            }

            return response;
        }
        #endregion

        #region checkUserAccess

        // This method checks if a user has access to a specific controller and action based on their role type
        public string checkUserAccess(int? userId, int? roleTypeId, string controllerName, string actionName)
        {
            try
            {
                SqlConnection con = new SqlConnection();
                con = DBConnection.GlobalConnection();

                SqlCommand cmd = new SqlCommand("checkUserAccess", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@userId", userId.ToString());
                cmd.Parameters.AddWithValue("@roleTypeId", roleTypeId.ToString());
                cmd.Parameters.AddWithValue("@controllerName", controllerName);
                cmd.Parameters.AddWithValue("@actionName", actionName);

                cmd.Parameters.Add("@message", SqlDbType.NVarChar, 500).Value = string.Empty; // Add an output parameter for the message
                cmd.Parameters["@message"].Direction = ParameterDirection.Output;// Set the direction of the output parameter to Output

                con.Open();
                cmd.ExecuteNonQuery();
                string message = Convert.ToString(cmd.Parameters["@message"].Value); // Get the message from the output parameter
                con.Close();

                return message;
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        #endregion

        #region OTP

        // Generate a 6-digit OTP (One-Time Password) using an array of integers eg. { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
        public static string OTP(int[] abc)
        {
            const string valid = "1234567890"; // Valid characters for the OTP
            StringBuilder res = new StringBuilder(); // StringBuilder to build the OTP string
            Random rnd = new Random(); // Random number generator
            for (int i = 0; i < 6; i++) // Loop to generate 6 characters for the OTP
            {
                res.Append(valid[abc[i]]); // Append a random character from the valid characters based on the index from the abc array
            }
            return res.ToString(); // Return the generated OTP as a string eg. "123456", "789012", etc.
        }

        #endregion

        #region sendEmail
        public string sendEmail(string email, string subject, string msg, string filePath)
        {
            try
            {
                MailMessage Msg = new MailMessage();
                Msg.From = new MailAddress("shankarvarma315@gmail.com", "Testing");
                Msg.To.Add(new MailAddress(email));
                Msg.Subject = subject;

                // Attach file if path is valid
                if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                {
                    Attachment attachment = new Attachment(filePath);
                    Msg.Attachments.Add(attachment);
                }


                Msg.Body = msg;
                Msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 25; //25 or 587
                smtp.Credentials = new System.Net.NetworkCredential("shankarvarma315@gmail.com", "vdbc htxl upxo xyka");
                smtp.EnableSsl = true;
                smtp.Timeout = 50000;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(Msg);
                Msg = null;
            }
            catch (Exception e1)
            {
                return "201" + e1.Message;
            }
            return "200";
        }
        #endregion          

        #region CreatePassword

        // This method generates a random password of length 8 using an array of integers to index into a string of valid characters
        public static string CreatePassword(int[] abc)
        {
            // Define a string of valid characters for the password
            const string valid = "abcdefghjkmnopqrstuvwxyzABCDEFGHJKMNOPQRSTUVWXYZ234567890"; // Valid characters for the password
            StringBuilder res = new StringBuilder(); // StringBuilder to build the password string
            Random rnd = new Random(); // Random number generator
            for (int i = 0; i < 8; i++) // Loop to generate 8 characters for the password
            {
                res.Append(valid[abc[i]]); // Append a random character from the valid characters based on the index from the abc array
            }
            return res.ToString(); // Return the generated password as a string eg. "aBc12345", "XyZ78901", etc.
        }

        #endregion
    }
}