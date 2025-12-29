using System;

namespace SchoolErpAPI.Models
{
    public class FeeHeadFilter
    {
        public int? id { get; set; }
        public bool? status { get; set; }
        public bool? deleted { get; set; }
        public int? pageNo { get; set; }
    }

    public class FeeHead
    {
        public int id { get; set; }
        public DateTime? creation_timestamp { get; set; }
        public int created_by_id { get; set; }

        public string name { get; set; }
        public string code { get; set; }
        public string frequency { get; set; }
        public decimal default_amount { get; set; }

        public bool? deleted { get; set; }
        public int? deleted_by_id { get; set; }
        public DateTime? deleted_timestamp { get; set; }
        public bool? status { get; set; }
    }

    public class SaveFeeHeadRequest
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string frequency { get; set; }
        public decimal? default_amount { get; set; }
        public bool? status { get; set; }

        public int? created_by_id { get; set; }
    }

    public class DeleteFeeHeadRequest
    {
        public int? id { get; set; }
        public int? deleted_by_id { get; set; }
    }
}
