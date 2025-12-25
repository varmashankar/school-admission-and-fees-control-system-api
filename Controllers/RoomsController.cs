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
    public class RoomsController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage saveRoom(Rooms data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.roomNo))
                    return Return.returnHttp("201", new { message = "Please Enter Room Number." });

                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                data.createdTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALRooms bal = new BALRooms();
                SPResponse response = bal.saveRoom(data);

                if (response.executionStatus == "TRUE") return Return.returnHttp("200", response.message);
                return Return.returnHttp("201", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", new { message = "Some Internal Issue Occurred. " + ex.Message });
            }
        }

        [HttpPost]
        public HttpResponseMessage getRoomList(RoomFilter filter)
        {
            try
            {
                BALRooms bal = new BALRooms();
                var list = bal.getRoomList(filter);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage changeStatusRooms(Rooms data)
        {
            try
            {
                if (!data.id.HasValue) return Return.returnHttp("201", "Invalid room selected.");

                BALRooms bal = new BALRooms();
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
        public HttpResponseMessage deleteRooms(Rooms data)
        {
            try
            {
                if (!data.id.HasValue) return Return.returnHttp("201", "Invalid room selected.");

                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                data.deletedTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALRooms bal = new BALRooms();
                var response = bal.deleteRoom(data);
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