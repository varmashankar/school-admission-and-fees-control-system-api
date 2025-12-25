using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SchoolErpAPI.Models
{
    public class CustomHeaderFilter : ActionFilterAttribute
    {
        #region OnActionExecuting

        //this runs before the api logic is executed
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            //fetch controller and action name
            string controllerNm = actionContext.ControllerContext.ControllerDescriptor.ControllerName.ToString().ToLower();
            string actionNm = actionContext.ActionDescriptor.ActionName.ToString().ToLower();

            //prepare variable to store values from header
            string userId = "", loginAs = "", checkUserAccessResponse;
            IEnumerable<string> headers = new List<string>();

            //tries to read #userId and #loginAs from header
            if (actionContext.Request.Headers.TryGetValues("userId", out headers))
                userId = actionContext.Request.Headers.GetValues("userId").FirstOrDefault();

            if (actionContext.Request.Headers.TryGetValues("loginAs", out headers))
                loginAs = actionContext.Request.Headers.GetValues("loginAs").FirstOrDefault();

            //create object of function class
            Function function = new Function();


            //if both #userId and #loginAs are available, it calls method to check if the user has access to the contoller+action && if not passes null
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(loginAs))
            {
                checkUserAccessResponse = function.checkUserAccess(Convert.ToInt32(userId), Convert.ToInt32(loginAs), controllerNm + "Controller", actionNm);
            }
            else
            {
                checkUserAccessResponse = function.checkUserAccess(null, null, controllerNm + "Controller", actionNm);
            }

            //if access is allowed -> proceed and if not then return a custom response using Return.returnHttp()
            if (checkUserAccessResponse == "TRUE")
            {
                base.OnActionExecuting(actionContext);
            }
            else
            {
                actionContext.Response = Return.returnHttp("215", checkUserAccessResponse);
                return;
            }


        }

        #endregion

        #region OnActionExecuted

        //this run after the api logic is complete
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            string controllerNm = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName.ToString().ToLower();
            string actionNm = actionExecutedContext.ActionContext.ActionDescriptor.ActionName.ToString().ToLower();
            int? id;

            //allow browser to read the token header in the response (for CORS)
            actionExecutedContext.Response.Content.Headers.Add("Access-Control-Expose-Headers", "token");

            IEnumerable<string> headers = new List<string>();

            //it generate new token using the user ID and role and send it the response header
            if (actionExecutedContext.Request.Headers.TryGetValues("userId", out headers) && actionExecutedContext.Request.Headers.TryGetValues("loginAs", out headers))
            {
                int userId = Int32.Parse(actionExecutedContext.Request.Headers.GetValues("userId").FirstOrDefault());
                int loginAs = Int32.Parse(actionExecutedContext.Request.Headers.GetValues("loginAs").FirstOrDefault());

                actionExecutedContext.Response.Content.Headers.Add("token", Function.CreateToken(loginAs, userId));
            }
        }

        #endregion
    }

    #region ContentInterceptorHandler
    //this handler intercept request and changes header/body before they go to the controller
    public class ContentInterceptorHandler : DelegatingHandler
    {

        //this method intercept every request and process it
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //get full URL of the request
            string controllerNm = request.RequestUri.ToString().ToLower();

            //request.Content = null;

            //only proceed if request has body and is not a Swagger request
            if (request.Content != null && !controllerNm.Contains("swagger"))
            {

                //tries to read token from request header
                string token = "";
                IEnumerable<string> headers = new List<string>();

                if (request.Headers.TryGetValues("token", out headers))
                    token = request.Headers.GetValues("token").FirstOrDefault();

                //call decryptToken() to decode the token and get userId and loginAs
                Function function = new Function();

                TokenResponse tokenResponse = function.decryptToken(token);

                System.Diagnostics.Debug.WriteLine("Token received: " + token);
                System.Diagnostics.Debug.WriteLine("Decrypted UserId: " + tokenResponse.userId);
                System.Diagnostics.Debug.WriteLine("Decrypted LoginAs: " + tokenResponse.loginAs);


                //add userId and loginAs to request headers so your filter can use them
                if (!String.IsNullOrEmpty(tokenResponse.userId))
                {
                    request.Headers.Add("userId", tokenResponse.userId);
                }
                if (!String.IsNullOrEmpty(tokenResponse.loginAs))
                {
                    request.Headers.Add("loginAs", tokenResponse.loginAs);
                }

                //Injecting into json body (if not file upload) --reads the body (JSON), adds userId and roleTypeId (loginAs), and writes it back.
                if (!controllerNm.Contains("fileupload"))
                {

                    var requestBody = await request.Content.ReadAsStringAsync();

                    dynamic value = JsonConvert.DeserializeObject<dynamic>(requestBody);

                    if (!String.IsNullOrEmpty(tokenResponse.userId))
                    {
                        value.userId = tokenResponse.userId;
                    }
                    if (!String.IsNullOrEmpty(tokenResponse.loginAs))
                    {
                        value.roleTypeId = tokenResponse.loginAs;
                    }

                    string valueStr = JsonConvert.SerializeObject(value);

                    request.Properties["Content"] = valueStr;
                    request.Content = new StringContent(valueStr, Encoding.UTF8, request.Content.Headers.ContentType.MediaType);

                }
            }

            //passes the request to the controller
            return await base.SendAsync(request, cancellationToken);

        }
    }

    #endregion
}