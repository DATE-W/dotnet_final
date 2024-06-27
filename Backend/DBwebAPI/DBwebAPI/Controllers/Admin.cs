using DBwebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using DBwebAPI.Models;
using DBwebAPI.Relations;
using static DBwebAPI.Models.NoticeModel;

namespace DBwebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminController : ControllerBase
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

        [HttpGet]
        public async Task<IActionResult> GetAllPost()
        {
            try
            {
                Console.WriteLine("--------------------------Get GetAllPost--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                
                List<Posts> posts = new List<Posts>();
                posts = await sqlORM.Queryable<Posts>().ToListAsync();
                List<AdminPost> adminPosts = new List<AdminPost>();
                foreach (Posts post in posts)
                {
                    PublishPost publishPost = await sqlORM.Queryable<PublishPost>().SingleAsync(it => it.post_id == post.post_id);
                    Usr user = await sqlORM.Queryable<Usr>().SingleAsync(it=>it.user_id==publishPost.user_id);

                    AdminPost t = new AdminPost();
                    t.post_id = post.post_id;
                    t.title = post.title;
                    t.contains = post.contains;
                    t.isbanned = post.isBanned;
                    t.publishtime = post.publishDateTime;
                    t.author_avatar=user.avatar;
                    t.author_name = user.userName;
                    t.approvalnum = post.approvalNum;
                    adminPosts.Add(t);
                }
                adminPosts=adminPosts.OrderByDescending(it => it.post_id).ToList();
                return Ok(adminPosts.ToArray());
                //return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        public class postjson
        {
            public int post_id { get;set; }
        }
        [HttpPost]
        public async Task<IActionResult> BanPostbyID(postjson json)
        {
            try
            {
                Console.WriteLine("--------------------------Get BanPostbyID--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

                int post_id = json.post_id;
                Posts posts = await sqlORM.Queryable<Posts>().SingleAsync(it=>it.post_id == post_id);
                PublishPost publishPost = await sqlORM.Queryable<PublishPost>().SingleAsync(it=>it.post_id == post_id);
                if (posts == null)
                    return Ok(new { ok = "no" });
                posts.isBanned = 1;
                int count = await sqlORM.Updateable(posts).ExecuteCommandAsync();

                Notice notice = new Notice();
                int? id = sqlORM.Queryable<Notice>().Max(it => it.notice_id);
                if (id.HasValue) id++; else id = 0;
                notice.notice_id = id;
                notice.text = posts.title;
                notice.publishdatetime = DateTime.Now;
                notice.receiver = publishPost.user_id;
                int count2 = await sqlORM.Insertable(notice).ExecuteCommandAsync();

                if (count == 0 || count2==0)
                    return Ok(new { ok = "no" });
                return Ok(new { ok = "yes" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        public class BannedUsrJson
        {
            public int user_id { get; set; }
            public string username { get; set; }
            public int userpoint { get; set; }
            public DateTime? regdate { get;set; }
            public int followednumber { get; set; }
            public string uft { get;set; }
        }
        [HttpGet]
        public async Task<IActionResult> GetBannedUser()
        {
            try
            {
                Console.WriteLine("--------------------------Get GetBannedUser--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                List<BannedUsrJson> bannedUsrJsons = new List<BannedUsrJson>();
                List<Usr> usrs = new List<Usr>();
                usrs = await sqlORM.Queryable<Usr>().Where(it=>it.isBanned==1).ToListAsync();
                foreach (Usr usr in usrs)
                {
                    BannedUsrJson t=new BannedUsrJson();
                    t.user_id = usr.user_id;
                    t.username = usr.userName;
                    t.userpoint = usr.userPoint;
                    t.regdate = usr.createDateTime;
                    t.followednumber = usr.followednumber;
                    UserFavouriteTeam UFT = new UserFavouriteTeam();
                    UFT = await sqlORM.Queryable<UserFavouriteTeam>().SingleAsync(it => it.user_id == usr.user_id);
                    if (UFT == null)
                    {
                        t.uft = "暂无主队";
                    }else
                    {
                        Team team = new Team();
                        team = await sqlORM.Queryable<Team>().SingleAsync(it => it.team_id == UFT.team_id);
                        t.uft = team.chinesename;
                    }
                    bannedUsrJsons.Add(t);
                }
                return Ok(bannedUsrJsons.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        public class banneduserjson
        {
            public int user_id { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> LiftUser(banneduserjson json)
        {
            try
            {
                Console.WriteLine("--------------------------Get LiftUser--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

                int user_id = json.user_id;
                Usr usr = await sqlORM.Queryable<Usr>().SingleAsync(it => it.user_id == user_id);
                if (usr == null)
                    return Ok(new { ok = "no" });
                usr.isBanned = 0;
                int count = await sqlORM.Updateable(usr).ExecuteCommandAsync();
                if (count == 0)
                    return Ok(new { ok = "no" });
                return Ok(new { ok = "yes" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllNews()
        {
            try
            {
                Console.WriteLine("--------------------------Get GetAllNews--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

                List<News> news = new List<News>();
                news = await sqlORM.Queryable<News>().ToListAsync();
                return Ok(news.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        public class deleteNewsJson
        {
            public int id { get; set;}
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteNews(deleteNewsJson json)
        {
            try
            {
                Console.WriteLine("--------------------------Get GetAllNews--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

                int id = json.id;
                int count1 = await sqlORM.Deleteable<NewsHavePicture>()
                    .Where(it => it.news_id == id)
                    .ExecuteCommandAsync();
                int count2 = await sqlORM.Deleteable<News>()
                    .Where(it => it.news_id == id)
                    .ExecuteCommandAsync();
                if (count1 == 0 && count2==0)
                    return Ok(new { ok = "no" });
                return Ok(new { ok = "yes"});
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
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
        [HttpPost]
        public async Task<IActionResult> SearchUser(SearchUserJson json)
        {
            try
            {
                Console.WriteLine("--------------------------Get SearchUser--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

                string searchKey = json.searchkey.ToLower();

                List<Usr> matchingUsers = await sqlORM.Queryable<Usr>()
                    .Where(it => it.isBanned == 0 && (it.userName.Contains(searchKey) || it.userAccount.Contains(searchKey)))
                    .OrderByDescending(it => it.userName.Contains(searchKey) ? 2 : 1)
                    .ToListAsync();

                List<UserJson> userJsonList = matchingUsers.Select(usr => new UserJson
                {
                    user_id = usr.user_id,
                    useraccount = usr.userAccount,
                    username = usr.userName,
                    createtime = usr.createDateTime,
                    point = usr.userPoint,
                    followednum = usr.followednumber
                }).ToList();

                return Ok(userJsonList.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SearchBannedUser(SearchUserJson json)
        {
            try
            {
                Console.WriteLine("--------------------------Get SearchBannedUser--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

                string searchKey = json.searchkey.ToLower();

                List<Usr> matchingUsers = await sqlORM.Queryable<Usr>()
                    .Where(it => it.isBanned == 1 && (it.userName.Contains(searchKey) || it.userAccount.Contains(searchKey)))
                    .OrderByDescending(it => it.userName.Contains(searchKey) ? 2 : 1)
                    .ToListAsync();

                List<UserJson> userJsonList = matchingUsers.Select(usr => new UserJson
                {
                    user_id = usr.user_id,
                    useraccount = usr.userAccount,
                    username = usr.userName,
                    createtime = usr.createDateTime,
                    point = usr.userPoint,
                    followednum = usr.followednumber
                }).ToList();

                return Ok(userJsonList.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllNotice()
        {
            try
            {
                Console.WriteLine("--------------------------Get SearchBannedUser--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

                List<Notice> noticeList = new List<Notice>();
                noticeList = await sqlORM.Queryable<Notice>().Where(it=>it.receiver==0).ToListAsync();
                return Ok(noticeList.OrderByDescending(it=>it.publishdatetime).ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
    }

}