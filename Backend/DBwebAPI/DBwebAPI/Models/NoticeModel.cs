using SqlSugar;


namespace DBwebAPI.Models
{
    public class NoticeModel
    {

        public class Admins
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int? admin_id { get; set; }
            public string? adminName { get; set; }
            public string? adminPassword { get; set; }
            public string? avatar { get; set; }
            public DateTime? createDateTime { get; set; }
        }

        public class Notice
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int? notice_id { get; set; }
            public string? text { get; set; }
            public DateTime publishdatetime { get; set; }
            public int? receiver { get; set; }
        }

        public class adminPublishNotice
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int? notice_id { get; set; }
            [SugarColumn(IsPrimaryKey = true)]
            public int? admin_id { get; set; }
        }


        public class Reports
        {
            [SugarColumn(IsPrimaryKey = true)]
            public DateTime? report_time { get; set; }
            [SugarColumn(IsPrimaryKey = true)]
            public int reporter_id { get; set; }
            [SugarColumn(IsPrimaryKey = true)]
            public int? post_id { get; set; }
            public string? descriptions { get; set; }
            public string? reply { get; set; }
            public string? status { get; set; }
        }


    }
}
