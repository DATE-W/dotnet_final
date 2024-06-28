using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLib.Models
{
    public class AdminPost
    {
        public int post_id { get; set; }
        public string? author_avatar { get; set; }
        public string? author_name { get; set; }
        public string title { get; set; }
        public string? contains { get; set; }
        public DateTime publishtime { get; set; }
        public int approvalnum { get; set; }
        public int isbanned { get; set; }
    }

    public class postjson
    {
        public int post_id { get; set; }
    }

    public class BannedUsrJson
    {
        public int user_id { get; set; }
        public string username { get; set; }
        public int userpoint { get; set; }
        public DateTime? regdate { get; set; }
        public int followednumber { get; set; }
        public string uft { get; set; }
    }

    public class banneduserjson
    {
        public int user_id { get; set; }
    }

    public class deleteNewsJson
    {
        public int id { get; set; }
    }

    public class SearchUserJson
    {
        public string searchkey { get; set; }
    }
    public class UserJson
    {
        public int user_id { get; set; }
        public string useraccount { get; set; }
        public string username { get; set; }
        public DateTime? createtime { get; set; }
        public int point { get; set; }
        public int followednum { get; set; }
    }



}
