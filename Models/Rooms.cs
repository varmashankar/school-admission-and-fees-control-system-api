using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.Models
{
    public class Rooms
    {
        public int? id { get; set; }
        public string createdTimestamp { get; set; }
        public int? userId { get; set; }

        public string roomNo { get; set; }
        public string floor { get; set; }
        public int? capacity { get; set; }

        public bool deleted { get; set; } = false;
        public string deletedTimestamp { get; set; }
        public int? deletedById { get; set; }
        public bool status { get; set; } = true;
        public int? roleTypeId { get; set; }
    }

    public class GetRoom
    {
        public int? id { get; set; }
        public string created_timestamp { get; set; }
        public int? created_by_id { get; set; }

        public string room_no { get; set; }
        public string floor { get; set; }
        public int? capacity { get; set; }

        public bool? deleted { get; set; } = false;
        public string deleted_timestamp { get; set; }
        public int? deleted_by_id { get; set; }
        public bool? status { get; set; } = true;
    }

    public class RoomFilter
    {
        public int? id { get; set; }
        public bool? deleted { get; set; } = false;
        public bool? status { get; set; } = true;
        public int? pageNo { get; set; }
    }
}