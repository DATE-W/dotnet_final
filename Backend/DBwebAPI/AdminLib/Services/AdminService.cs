using AdminLib.Models;
using AdminLib.Utils;
using NewsLib.Models;
using NewsLib.Relations;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamLib.Models;
using static AdminLib.Models.NoticeModel;

namespace AdminLib.Services
{
    public class AdminService : IAdminService
    {
        public async Task<AdminPost[]> GetAllPost()
        {
            Console.WriteLine("--------------------------Get GetAllPost--------------------------");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            if (!ORACLEConnectTry.getConn())
            {
                Console.WriteLine("数据库连接失败");
            };
            SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

            List<Posts> posts = new List<Posts>();
            posts = await sqlORM.Queryable<Posts>().ToListAsync();
            List<AdminPost> adminPosts = new List<AdminPost>();
            foreach (Posts post in posts)
            {
                PublishPost publishPost = await sqlORM.Queryable<PublishPost>().SingleAsync(it => it.post_id == post.post_id);
                Usr user = await sqlORM.Queryable<Usr>().SingleAsync(it => it.user_id == publishPost.user_id);

                AdminPost t = new AdminPost();
                t.post_id = post.post_id;
                t.title = post.title;
                t.contains = post.contains;
                t.isbanned = post.isBanned;
                t.publishtime = post.publishDateTime;
                t.author_avatar = user.avatar;
                t.author_name = user.userName;
                t.approvalnum = post.approvalNum;
                adminPosts.Add(t);
            }
            adminPosts = adminPosts.OrderByDescending(it => it.post_id).ToList();
            return adminPosts.ToArray();
        }
        public async Task<string> BanPostbyID(postjson json)
        {
            Console.WriteLine("--------------------------Get BanPostbyID--------------------------");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            if (!ORACLEConnectTry.getConn())
            {
                Console.WriteLine("数据库连接失败");
            };
            SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

            int post_id = json.post_id;
            Posts posts = await sqlORM.Queryable<Posts>().SingleAsync(it => it.post_id == post_id);
            PublishPost publishPost = await sqlORM.Queryable<PublishPost>().SingleAsync(it => it.post_id == post_id);
            if (posts == null)
                return "no";
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

            if (count == 0 || count2 == 0)
                return "no";
            return "yes";
        }
        public async Task<BannedUsrJson[]> GetBannedUser()
        {
            Console.WriteLine("--------------------------Get GetBannedUser--------------------------");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            if (!ORACLEConnectTry.getConn())
            {
                Console.WriteLine("数据库连接失败");
            };
            SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
            List<BannedUsrJson> bannedUsrJsons = new List<BannedUsrJson>();
            List<Usr> usrs = new List<Usr>();
            usrs = await sqlORM.Queryable<Usr>().Where(it => it.isBanned == 1).ToListAsync();
            foreach (Usr usr in usrs)
            {
                BannedUsrJson t = new BannedUsrJson();
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
                }
                else
                {
                    Team team = new Team();
                    team = await sqlORM.Queryable<Team>().SingleAsync(it => it.team_id == UFT.team_id);
                    t.uft = team.chinesename;
                }
                bannedUsrJsons.Add(t);
            }
            return (bannedUsrJsons.ToArray());
        }

        public async Task<string> LiftUser(banneduserjson json)
        {
            Console.WriteLine("--------------------------Get LiftUser--------------------------");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            if (!ORACLEConnectTry.getConn())
            {
                Console.WriteLine("数据库连接失败");
            };
            SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

            int user_id = json.user_id;
            Usr usr = await sqlORM.Queryable<Usr>().SingleAsync(it => it.user_id == user_id);
            if (usr == null)
                return "no";
            usr.isBanned = 0;
            int count = await sqlORM.Updateable(usr).ExecuteCommandAsync();
            if (count == 0)
                return "no";
            return "yes";
        }

        public async Task<News[]> GetAllNews()
        {
            Console.WriteLine("--------------------------Get GetAllNews--------------------------");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            if (!ORACLEConnectTry.getConn())
            {
                Console.WriteLine("数据库连接失败");
            };
            SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

            List<News> news = new List<News>();
            news = await sqlORM.Queryable<News>().ToListAsync();
            return (news.ToArray());
        }

        public async Task<string> DeleteNews(deleteNewsJson json)
        {
            Console.WriteLine("--------------------------Get GetAllNews--------------------------");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            if (!ORACLEConnectTry.getConn())
            {
                Console.WriteLine("数据库连接失败");
            };
            SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

            int id = json.id;
            int count1 = await sqlORM.Deleteable<NewsHavePicture>()
                .Where(it => it.news_id == id)
                .ExecuteCommandAsync();
            int count2 = await sqlORM.Deleteable<News>()
                .Where(it => it.news_id == id)
                .ExecuteCommandAsync();
            if (count1 == 0 && count2 == 0)
                return "no";
            return "yes";
        }

        public async Task<UserJson[]> SearchUser(SearchUserJson json)
        {
            Console.WriteLine("--------------------------Get SearchUser--------------------------");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            if (!ORACLEConnectTry.getConn())
            {
                Console.WriteLine("数据库连接失败");
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

            return (userJsonList.ToArray());
        }

        public async Task<UserJson[]> SearchBannedUser(SearchUserJson json)
        {
            Console.WriteLine("--------------------------Get SearchBannedUser--------------------------");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            if (!ORACLEConnectTry.getConn())
            {
                Console.WriteLine("数据库连接失败");
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

            return (userJsonList.ToArray());
        }

        public async Task<Notice[]> GetAllNotice()
        {
            Console.WriteLine("--------------------------Get SearchBannedUser--------------------------");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            if (!ORACLEConnectTry.getConn())
            {
                Console.WriteLine("数据库连接失败");
            };
            SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
            List<Notice> noticeList = new List<Notice>();
            noticeList = await sqlORM.Queryable<Notice>().Where(it => it.receiver == 0).ToListAsync();
            return (noticeList.OrderByDescending(it => it.publishdatetime).ToArray());
        }
    }
}
