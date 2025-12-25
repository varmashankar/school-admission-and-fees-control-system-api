using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.Models
{
    public class Streams
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public string streamName { get; set; }
        public bool deleted { get; set; }
        public string deletedTimestamp { get; set; }
        public int? deletedById { get; set; }
        public bool status { get; set; } = true;

        public int? userId { get; set; }
        public int? roleTypeId { get; set; }
    }

    public class GetStream
    {
        public int? id { get; set; }
        public string created_timestamp { get; set; }
        public int? created_by_id { get; set; }

        public string stream_name { get; set; }
        public bool? deleted { get; set; }
        public string deleted_timestamp { get; set; }
        public int? deleted_by_id { get; set; }
        public bool? status { get; set; }
    }

    public class StreamFilter
    {
        public int? id { get; set; }
        public bool? deleted { get; set; } = false;
        public bool? status { get; set; } = true;
        public int? pageNo { get; set; }
    }
}