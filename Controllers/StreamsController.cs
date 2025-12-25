using SchoolErpAPI.BAL;
using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolErpAPI.Controllers
{
    public class StreamsController : ApiController
    {
        #region saveStream
        [HttpPost]
        public HttpResponseMessage saveStream(Streams data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.streamName))
                    return Return.returnHttp("201", new { message = "Please Enter Stream Name." });

                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                data.creationTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALStreams bal = new BALStreams();
                SPResponse response = bal.saveStream(data);

                if (response.executionStatus == "TRUE") return Return.returnHttp("200", response.message);
                return Return.returnHttp("201", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", new { message = "Some Internal Issue Occurred. " + ex.Message });
            }
        }

        #endregion

        [HttpPost]
        public HttpResponseMessage getStreamList(StreamFilter filter)
        {
            try
            {
                BALStreams bal = new BALStreams();
                var list = bal.getStreamList(filter);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage changeStatusStreams(Streams data)
        {
            try
            {
                if (!data.id.HasValue) return Return.returnHttp("201", "Invalid stream selected.");

                BALStreams bal = new BALStreams();
                var response = bal.changeStatus(data);
                if (response.executionStatus != "TRUE") return Return.returnHttp("201", response.message);
                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage deleteStreams(Streams data)
        {
            try
            {
                if (!data.id.HasValue) return Return.returnHttp("201", "Invalid stream selected.");

                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                data.deletedTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALStreams bal = new BALStreams();
                var response = bal.deleteStream(data);
                if (response.executionStatus != "TRUE") return Return.returnHttp("201", response.message);
                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }
    }
}