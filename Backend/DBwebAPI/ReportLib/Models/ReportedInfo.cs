using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportLib.Models
{
    public class UsrInfo
    {
        public int? user_id { get; set; }
        public string? userName { get; set; }
        public int? userPoint { get; set; }
        public string? regDate { get; set; }
        public int? followedNumber { get; set; }
        public int? isbanned { get; set; }
    }

    public class banUsrPara
    {
        public int? user_id { get; set; }
    }

    public class dealReportVal
    {
        public int? reporter_id { get; set; }
        public int? post_id { get; set; }
        public string? reply { get; set; }
    }

    public class reportInfo
    {
        public int? post_id { get; set; }
        public string? title { get; set; }
        public string? publisherName { get; set; }
        public string? reporterName { get; set; }
        public int? reporter_id { get; set; }
        public string? description { get; set; }
    }
}
