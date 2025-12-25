using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace SchoolErpAPI.Models
{
    public class ReturnObject // This class is used to structure the response object that will be returned by the API.
    {
        public string response_code; // This property holds the response code, which indicates the status of the request (e.g., success, error).
        public object obj; // This property holds the object that will be returned in the response, which can be any type of data (e.g., a list, a single object, etc.).

    }
    public class Return
    {
        #region returnHttp


        public static HttpResponseMessage returnHttp(string response_code, Object obj)  // This method returns a HttpResponseMessage with the given response_code and object as JSON.
        {
            try
            {
                var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer(); // Using JavaScriptSerializer to serialize the object to JSON.
                ReturnObject ro = new ReturnObject(); // Creating an instance of ReturnObject to hold the response code and object.
                ro.response_code = response_code; // Setting the response code.
                ro.obj = obj; // Setting the object to be returned.

                string json = JsonConvert.SerializeObject(ro, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore }); // Serializing the ReturnObject to JSON format with indentation for readability and ignoring null values.
                return new HttpResponseMessage() // Creating a new HttpResponseMessage to return the JSON response.
                {
                    Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") // Setting the content of the response to the serialized JSON string with UTF-8 encoding and specifying the content type as application/json.
                };
            }
            catch
            {
                var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer(); // Creating a new instance of JavaScriptSerializer in case of an error.
                ReturnObject ro = new ReturnObject(); // Creating a new instance of ReturnObject to hold the error response.
                ro.response_code = "201"; // Setting the response code to indicate an error (201 is typically used for created resources, but here it indicates an error in parsing).
                ro.obj = "There was an error parsing the Data. Please try again.";// Setting the object to an error message.

                string json = JsonConvert.SerializeObject("Parse error", new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }); // Serializing the error message to JSON format, ignoring null values.
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") // Creating a new HttpResponseMessage with the error message as content.
                };

            }
        }

        #endregion
    }
}