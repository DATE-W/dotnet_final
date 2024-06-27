using SqlSugar;
using System.Security.Principal;

namespace DBwebAPI.Models
{
    public class AdminController
    {

        public class Admins
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int? admin_id { get; set; }

            [SugarColumn(IsNullable = false, Length = 50)]
            public string? adminName { get; set; }

            [SugarColumn(IsNullable = false, Length = 50)]
            public string? adminPassword { get; set; }
            public DateTime? createDateTime { get; set; }
        }


    }
}
