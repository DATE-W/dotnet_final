using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using DBwebAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using static DBwebAPI.Controllers.LoginController;
using static DBwebAPI.Controllers.ForumController;
using static DBwebAPI.Controllers.UserController;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System;
using Dm;
using System.Reflection.Metadata;
using System.Security.Principal;
using Newtonsoft.Json.Linq;
using static DBwebAPI.Models.NoticeModel;

namespace DBwebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        public class profileJson {
            public String username { get; set; } = "";
            public String uft { get; set; } = "";
            public int follower_num { get; set; }
            public int follow_num { get; set; }
            public int approval_num { get; set; }
            public String? account { get; set; } = "";
            public String avatar { get; set; } = "";
            public String signature { get; set; } = "";
            public int userpoint { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> profile()
        {
            try
            {
                Console.WriteLine("--------------------------Get profile--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                
                 // 从请求头中获取传递的JWT令牌
                 string authorizationHeader = Request.Headers["Authorization"].First();
                 //验证 Authorization 请求头是否包含 JWT 令牌
                 if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer"))
                 {
                     Console.WriteLine("未提供有效的JWT");
                     return BadRequest(new { ok = "no", value = "未提供有效的JWT" });
                 }
                 //
                 string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                 // 验证并解析JWT令牌
                 var handler = new JwtSecurityTokenHandler();
                 var tokenS = handler.ReadJwtToken(jwtToken);
                 // 获取JWT令牌中的claims信息
                 string? account = tokenS.Claims.First(claim => claim.Type == "account")?.Value;
                 List<Usr> tempUsr = new List<Usr>();
                 tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account)
                     .ToListAsync();
                 //判断用户是否存在
                 if (tempUsr.Count() == 0)
                 {
                     Console.WriteLine("no such user");
                     return Ok(new CustomResponse { ok = "no", value = "错误的用户信息" });//用户账户或密码错误
                 }

                int user_id = tempUsr.FirstOrDefault()?.user_id ?? 1; 
                /*int user_id = 2;
                List <Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.user_id == user_id)
                    .ToListAsync();
                String account = tempUsr.FirstOrDefault().userAccount; */

                //用户名
                String userName = tempUsr.FirstOrDefault()?.userName?? "";
                //个性签名
                String signature = tempUsr.FirstOrDefault()?.signature ??"";
                //头像
                String avatar = tempUsr.FirstOrDefault()?.avatar ?? "http://110.40.206.206/pictures/3ea6beec64369c2642b92c6726f1epng.png";
                //关注数
                int follower_num = tempUsr.FirstOrDefault()?.follownumber??0;
                //粉丝数
                int follow_num = tempUsr.FirstOrDefault()?.followednumber ?? 0;
                //主队
                List <UserFavouriteTeam> tmpUFT_id = new List<UserFavouriteTeam>();
                tmpUFT_id = await sqlORM.Queryable<UserFavouriteTeam>().Where(it => it.user_id == user_id)
                    .ToListAsync();
                int UFT_id = tmpUFT_id.Count() != 0 ? tmpUFT_id.FirstOrDefault()?.team_id ?? 0 : 0;
                List<Team> tmpUFT = new List<Team>();
                tmpUFT = await sqlORM.Queryable<Team>().Where(it => it.team_id == UFT_id)
                    .ToListAsync();
                String UFT = tmpUFT.Count() != 0 ? tmpUFT.FirstOrDefault()?.chinesename?? "暂无主队" : "暂无主队";
                //点赞数
                List<PublishPost> tmpPP = new List<PublishPost>();
                tmpPP = await sqlORM.Queryable<PublishPost>().Where(it => it.user_id == user_id)
                    .ToListAsync();
                int approvalNum = 0;
                foreach (var pp in tmpPP)
                {
                    List<Posts> tmpPost = new List<Posts>();
                    tmpPost = await sqlORM.Queryable<Posts>().Where(it => it.post_id == pp.post_id)
                        .ToListAsync();
                    approvalNum += tmpPost.Count() != 0 ? tmpPost.FirstOrDefault()?.approvalNum ?? 0 : 0;
                }
                profileJson response = new profileJson();
                response.username = userName;
                response.uft = UFT;
                response.approval_num = approvalNum;
                response.follower_num = follower_num;
                response.follow_num = follow_num;
                response.account = account;
                response.avatar = avatar;
                response.signature = signature;
                response.userpoint = tempUsr.FirstOrDefault()?.userPoint ?? 0;
                Console.WriteLine("response.username" + response.username);
                Console.WriteLine("response.uft:" + response.uft);
                Console.WriteLine("response.approval_num:" + response.approval_num);
                Console.WriteLine("response.follower_num:" + response.follower_num);
                Console.WriteLine("response.follow_num:" + response.follow_num);
                Console.WriteLine("response.account:" + response.account);
                Console.WriteLine("response.avatar:" + response.avatar);
                Console.WriteLine("response.signature:" + response.signature);
                return Ok(new CustomResponse { ok = "yes", value = response });
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred:" + ex.Message);
                return BadRequest(new { error = "An error occurred." });
            }
        }
        public class modifyprofileJson
        {
            public string username { get; set; } = "";
            public string avatar { get; set; } = "";
            public string signature { get; set; } = "";
        }
        [HttpPost]
        public async Task<IActionResult> modifyprofile([FromBody] modifyprofileJson json)
        {
            try
            {
                Console.WriteLine("--------------------------Get modifyprofile--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
               
                // 从请求头中获取传递的JWT令牌
                string authorizationHeader = Request.Headers["Authorization"].First();
                //验证 Authorization 请求头是否包含 JWT 令牌
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer"))
                {
                    Console.WriteLine("JWT error");
                    return BadRequest(new { ok = "no", value = "未提供有效的JWT" });
                }
                //
                string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                // 验证并解析JWT令牌
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadJwtToken(jwtToken);
                // 获取JWT令牌中的claims信息
                string? account = tokenS.Claims.First(claim => claim.Type == "account")?.Value;
                List<Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account)
                    .ToListAsync();
                //判断用户是否存在
                if (tempUsr.Count() == 0)
                {
                    Console.WriteLine("no such user");
                    return Ok(new CustomResponse { ok = "no", value = "错误的用户信息" });//用户账户或密码错误
                }

                Usr tmpU = new Usr(); 
                tmpU=tempUsr.First();
                int count = 0;
                int user_id = tempUsr.FirstOrDefault()?.user_id ?? 1;
                Console.WriteLine("userName:"+ json.username);
                Console.WriteLine("avatar:" + json.avatar);
                Console.WriteLine("signature:" + json.signature);

                tmpU.userName = json?.username??"";
                tmpU.avatar =json?.avatar??"/";
                tmpU.signature = json?.signature ?? "";
                count = await sqlORM.Updateable(tmpU).ExecuteCommandAsync();
                if ( count>0)
                {
                    Console.WriteLine("modify success");
                    return Ok(new { ok = "yes" });
                }
                else
                {
                    Console.WriteLine("modify fail");
                    return Ok(new { ok = "no" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("errorResponse: " + ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "数据库连接错误" });//用户账户或密码错误
            }
        }
        public class utfJson
        {
            public String teamname { get; set; } = "";
        }
        [HttpPost]
        public async Task<IActionResult> modifyteam([FromBody] utfJson json)
        {
            try
            {
                Console.WriteLine("--------------------------Get modifyteam--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                
                 // 从请求头中获取传递的JWT令牌
                 string authorizationHeader = Request.Headers["Authorization"].First();
                 //验证 Authorization 请求头是否包含 JWT 令牌
                 if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer"))
                 {
                     Console.WriteLine("未提供有效的JWT");
                     return BadRequest(new { ok = "no", value = "未提供有效的JWT" });
                 }
                 //
                 string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                 // 验证并解析JWT令牌
                 var handler = new JwtSecurityTokenHandler();
                 var tokenS = handler.ReadJwtToken(jwtToken);
                 // 获取JWT令牌中的claims信息
                 string? account = tokenS.Claims.First(claim => claim.Type == "account")?.Value;
                 List<Usr> tempUsr = new List<Usr>();
                 tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account)
                     .ToListAsync();
                 //判断用户是否存在
                 if (tempUsr.Count() == 0)
                 {
                     Console.WriteLine("用户不存在");
                     return Ok(new CustomResponse { ok = "no", value = "错误的用户信息" });//用户账户或密码错误
                 }

                 int user_id = tempUsr.First().user_id;
              /*  int user_id = 8;*/
                if (json.teamname == "")
                {
                    List<UserFavouriteTeam> tmpUFT = new List<UserFavouriteTeam>();
                    tmpUFT = await sqlORM.Queryable<UserFavouriteTeam>().Where(it => it.user_id == user_id)
                        .ToListAsync();
                    int count=0;
                    if (tmpUFT.Count() != 0)
                    {
                        count = await sqlORM.Deleteable<UserFavouriteTeam>()
                       .Where(it => it.user_id == user_id)
                       .ExecuteCommandAsync();                    }
                    if (count > 0)
                    {
                        return Ok(new CustomResponse { ok = "ok", value = "主队删除成功" });
                    }
                    else
                    {
                        return Ok(new CustomResponse { ok = "no", value = "原本就没有主队" });
                    }
                }
                else
                {
                    //查找队伍id
                    int team_id;
                    List<Team> tmpteam = new List<Team>();
                    tmpteam = await sqlORM.Queryable<Team>().Where(it => it.chinesename == json.teamname)
                        .ToListAsync();
                    if (tmpteam.Count() == 0)
                    {
                        return Ok(new CustomResponse { ok = "no", value = "暂无主队" });//查无此队
                    }
                    else
                    {
                        team_id = tmpteam.First().team_id;
                    }
                    List<UserFavouriteTeam> tmpUFT = new List<UserFavouriteTeam>();
                    tmpUFT = await sqlORM.Queryable<UserFavouriteTeam>().Where(it => it.user_id == user_id)
                        .ToListAsync();
                    int count;
                    if (tmpUFT.Count() == 0)
                    {
                        //新建关系
                        UserFavouriteTeam tmp = new UserFavouriteTeam();
                        tmp.user_id = user_id;
                        tmp.team_id = team_id;
                        tmp.createDateTime = DateTime.Now;
                        count = await sqlORM.Insertable(tmp).ExecuteCommandAsync();
                    }
                    else
                    {
                        //修改关系
                        tmpUFT.First().team_id = team_id;
                        tmpUFT.First().modifyDateTime = DateTime.Now;
                        // 更新数据库中的关系
                        count = await sqlORM.Updateable(tmpUFT.FirstOrDefault()).ExecuteCommandAsync();
                    }
                    if (count > 0)
                    {
                        return Ok(new CustomResponse { ok = "ok", value = "主队修改成功" });
                    }
                    else
                    {
                        return Ok(new CustomResponse { ok = "no", value = "主队修改失败" });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("errorResponse: " + ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "数据库连接错误" });//用户账户或密码错误
            }
        }
        public class ActionJson
        {
            public List<action> actions { get; set; } = new List<action>();
            public class action
            {
                public String type { get; set; } = "";
                public String title { get; set; } = "";
                public String contains { get; set; } = "";
                public String name { get; set; } = "";
                public String author { get; set; } = "";
                public int post_id { get; set; }
                public DateTime datetime { get; set; }
                public String comment { get; set; } = "";
                public String follower_name { get; set; } = "";
            }
        }
        [HttpPost]
        public async Task<IActionResult> userAction()
        {
            try
            {
                Console.WriteLine("--------------------------Get userAction--------------------------");

                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                
                // 从请求头中获取传递的JWT令牌
                string authorizationHeader = Request.Headers["Authorization"].First();
                //验证 Authorization 请求头是否包含 JWT 令牌
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer"))
                {
                    Console.WriteLine("未提供有效的JWT");
                    return BadRequest(new { ok = "no", value = "未提供有效的JWT" });
                }
                //
                string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                // 验证并解析JWT令牌
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadJwtToken(jwtToken);
                // 获取JWT令牌中的claims信息
                string? account = tokenS.Claims.First(claim => claim.Type == "account")?.Value;
                List<Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account)
                    .ToListAsync();
                //判断用户是否存在
                if (tempUsr.Count() == 0)
                {
                    Console.WriteLine("用户不存在");
                    return Ok(new CustomResponse { ok = "no", value = "错误的用户信息" });//用户账户或密码错误
                }
                int user_id = tempUsr.First().user_id;

                // 创建 ActionJson 实例
                ActionJson actionJson = new ActionJson();

                //发布帖子
                List<PublishPost> tmpPP = new List<PublishPost>();
                tmpPP = await sqlORM.Queryable<PublishPost>().Where(it => it.user_id == user_id)
                    .ToListAsync();
                foreach (var ap in tmpPP)
                {
                    List<Posts> tmpp = new List<Posts>();
                    tmpp = await sqlORM.Queryable<Posts>().Where(it => it.post_id == ap.post_id)
                        .ToListAsync();
                    List<Usr> tmpusr = new List<Usr>();
                    tmpusr = await sqlORM.Queryable<Usr>().Where(it => it.user_id == ap.user_id)
                        .ToListAsync();
                    ActionJson.action tmpac = new ActionJson.action();
                    tmpac.title = tmpp.First().title;
                    tmpac.contains = tmpp.First().contains;
                    tmpac.datetime = tmpp.First().publishDateTime;
                    tmpac.type = "publish";
                    tmpac.author = tmpusr.First().userName;
                    tmpac.post_id = ap.post_id;

                    actionJson.actions.Add(tmpac);
                }
                //赞同帖子
                List<LikePost> tmpAP = new List<LikePost>();
                tmpAP = await sqlORM.Queryable<LikePost>().Where(it => it.user_id == user_id)
                    .ToListAsync();
                foreach (var ap in tmpAP)
                {
                    List<Posts> tmpp = new List<Posts>();
                    tmpp = await sqlORM.Queryable<Posts>().Where(it => it.post_id == ap.post_id)
                        .ToListAsync();
                    List<PublishPost> tmppp = new List<PublishPost>();
                    tmppp = await sqlORM.Queryable<PublishPost>().Where(it => it.post_id == ap.post_id)
                        .ToListAsync();
                    List<Usr> tmpusr = new List<Usr>();
                    tmpusr = await sqlORM.Queryable<Usr>().Where(it => it.user_id == tmppp.First().user_id)
                        .ToListAsync();
                    ActionJson.action tmpac = new ActionJson.action();
                    tmpac.title = tmpp.First().title;
                    tmpac.contains = tmpp.First().contains;
                    tmpac.datetime = ap.createDateTime;
                    tmpac.type = "like";
                    tmpac.author = tmpusr.First().userName;
                    tmpac.post_id = ap.post_id;

                    actionJson.actions.Add(tmpac);
                }
                //收藏帖子
                List<CollectPost> tmpFP = new List<CollectPost>();
                tmpFP = await sqlORM.Queryable<CollectPost>().Where(it => it.user_id == user_id)
                    .ToListAsync();
                foreach (var ap in tmpFP)
                {
                    List<Posts> tmpp = new List<Posts>();
                    tmpp = await sqlORM.Queryable<Posts>().Where(it => it.post_id == ap.post_id)
                        .ToListAsync();
                    List<PublishPost> tmppp = new List<PublishPost>();
                    tmppp = await sqlORM.Queryable<PublishPost>().Where(it => it.post_id == ap.post_id)
                        .ToListAsync();
                    List<Usr> tmpusr = new List<Usr>();
                    tmpusr = await sqlORM.Queryable<Usr>().Where(it => it.user_id == tmppp.First().user_id)
                        .ToListAsync();
                    ActionJson.action tmpac = new ActionJson.action();
                    tmpac.title = tmpp.First().title;
                    tmpac.contains = tmpp.First().contains;
                    tmpac.datetime = ap.createDateTime;
                    tmpac.type = "collect";
                    tmpac.author = tmpusr.First().userName;
                    tmpac.post_id = ap.post_id;

                    actionJson.actions.Add(tmpac);
                }
                //评论帖子
                List<Comments> tmpCP = new List<Comments>();
                tmpCP = await sqlORM.Queryable<Comments>().Where(it => it.user_id == user_id)
                    .ToListAsync();
                foreach (var ap in tmpCP)
                {
                    List<Posts> tmpp = new List<Posts>();
                    tmpp = await sqlORM.Queryable<Posts>().Where(it => it.post_id == ap.post_id)
                        .ToListAsync();
                    List<PublishPost> tmppp = new List<PublishPost>();
                    tmppp = await sqlORM.Queryable<PublishPost>().Where(it => it.post_id == ap.post_id)
                        .ToListAsync();
                    List<Usr> tmpusr = new List<Usr>();
                    tmpusr = await sqlORM.Queryable<Usr>().Where(it => it.user_id == tmppp.First().user_id)
                        .ToListAsync();
                    ActionJson.action tmpac = new ActionJson.action();
                    tmpac.title = tmpp.First().title;
                    tmpac.contains = tmpp.First().contains;
                    tmpac.datetime = ap.publishDateTime;
                    tmpac.type = "comment";
                    tmpac.author = tmpusr.First().userName;
                    tmpac.post_id = ap.post_id;
                    tmpac.comment = ap.contains;
                    actionJson.actions.Add(tmpac);
                }
                //关注用户
                List<Follow> tmpFU = new List<Follow>();
                tmpFU = await sqlORM.Queryable<Follow>().Where(it => it.follower_id == user_id)
                    .ToListAsync();
                foreach (var ap in tmpFU)
                {
                    List<Usr> tmpusr = new List<Usr>();
                    tmpusr = await sqlORM.Queryable<Usr>().Where(it => it.user_id == ap.follow_id)
                        .ToListAsync();
                    ActionJson.action tmpac = new ActionJson.action();
                    tmpac.datetime = ap.createDateTime;
                    tmpac.type = "follow";
                    tmpac.name = tmpusr.First().userName;
                    actionJson.actions.Add(tmpac);
                }
                // 对 actionJson.actions 数组按照 datetime 降序排序
                actionJson.actions = actionJson.actions.OrderByDescending(a => a.datetime).ToList();
                return Ok(actionJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB error：" + ex.Message);
                return BadRequest(new { error = "DB error" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> userPoint()
        {
            try
            {
                Console.WriteLine("--------------------------Get userPoint--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                
                // 从请求头中获取传递的JWT令牌
                string authorizationHeader = Request.Headers["Authorization"].First();
                //验证 Authorization 请求头是否包含 JWT 令牌
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer"))
                {
                    Console.WriteLine("未提供有效的JWT");
                    return BadRequest(new { ok = "no", value = "未提供有效的JWT" });
                }
                //
                string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                // 验证并解析JWT令牌
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadJwtToken(jwtToken);
                // 获取JWT令牌中的claims信息
                string? account = tokenS.Claims.FirstOrDefault(claim => claim.Type == "account")?.Value;
                List<Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account)
                    .ToListAsync();
                //判断用户是否存在
                if (tempUsr.Count() == 0)
                {
                    Console.WriteLine("用户不存在");
                    return Ok(new CustomResponse { ok = "no", value = "错误的用户信息" });//用户账户或密码错误
                }
                int user_id = tempUsr.First().user_id;
                Console.WriteLine("Point:" + tempUsr.First().userPoint);
                return Ok(new { userpoint = tempUsr.First().userPoint });
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB error：" + ex.Message);
                return BadRequest(new { error = "DB error" });
            }
        }
        public class PointJson
        {
            public List<point> points { get; set; } = new List<point>();
            public class point
            {
                public String type = "";
                public DateTime datetime;
            }
        }
        [HttpPost]
        public async Task<IActionResult> userPointAction()
        {
            try
            {
                Console.WriteLine("--------------------------Get userAction--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                
                // 从请求头中获取传递的JWT令牌
                string authorizationHeader = Request.Headers["Authorization"].First();
                //验证 Authorization 请求头是否包含 JWT 令牌
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer"))
                {
                    Console.WriteLine("未提供有效的JWT");
                    return BadRequest(new { ok = "no", value = "未提供有效的JWT" });
                }
                //
                string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                // 验证并解析JWT令牌
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadJwtToken(jwtToken);
                // 获取JWT令牌中的claims信息
                string? account = tokenS.Claims.First(claim => claim.Type == "account")?.Value;
                List<Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account)
                    .ToListAsync();
                //判断用户是否存在
                if (tempUsr.Count() == 0)
                {
                    Console.WriteLine("用户不存在");
                    return Ok(new CustomResponse { ok = "no", value = "错误的用户信息" });//用户账户或密码错误
                }
                int user_id = tempUsr.First().user_id;
                /*
                int user_id = 12;                8=*/
                // 创建 ActionJson 实例
                PointJson pointJson = new PointJson();


                //发布帖子
                List<PublishPost> tmpPP = new List<PublishPost>();
                tmpPP = await sqlORM.Queryable<PublishPost>().Where(it => it.user_id == user_id)
                    .ToListAsync();
                foreach (var ap in tmpPP)
                {
                    List<Posts> tmpp = new List<Posts>();
                    tmpp = await sqlORM.Queryable<Posts>().Where(it => it.post_id == ap.post_id)
                        .ToListAsync();
                    PointJson.point tmpac = new PointJson.point();
                    tmpac.datetime = tmpp.First().publishDateTime;
                    tmpac.type = "publish";
                    pointJson.points.Add(tmpac);
                }
                //赞同帖子
                List<LikePost> tmpAP = new List<LikePost>();
                tmpAP = await sqlORM.Queryable<LikePost>().Where(it => it.user_id == user_id)
                    .ToListAsync();
                foreach (var ap in tmpAP)
                {
                    PointJson.point tmpac = new PointJson.point();
                    tmpac.datetime = ap.createDateTime;
                    tmpac.type = "like";
                    pointJson.points.Add(tmpac);
                }
                //收藏帖子
                List<CollectPost> tmpFP = new List<CollectPost>();
                tmpFP = await sqlORM.Queryable<CollectPost>().Where(it => it.user_id == user_id)
                    .ToListAsync();
                foreach (var ap in tmpFP)
                {

                    PointJson.point tmpac = new PointJson.point();
                    tmpac.datetime = ap.createDateTime;
                    tmpac.type = "collect";
                    pointJson.points.Add(tmpac);
                }
                //评论帖子
                List<Comments> tmpCP = new List<Comments>();
                tmpCP = await sqlORM.Queryable<Comments>().Where(it => it.user_id == user_id)
                    .ToListAsync();
                foreach (var ap in tmpCP)
                {
                    PointJson.point tmpac = new PointJson.point();
                    tmpac.datetime = ap.publishDateTime;
                    tmpac.type = "comment";
                    pointJson.points.Add(tmpac);
                }
                /*
                //关注用户
                List<Follow> tmpFU = new List<Follow>();
                tmpFU = await sqlORM.Queryable<Follow>().Where(it => it.follower_id == user_id)
                    .ToListAsync();
                foreach (var ap in tmpFU)
                {
                    PointJson.point tmpac = new PointJson.point();
                    tmpac.datetime = ap.createDateTime;
                    tmpac.type = "follow";
                    pointJson.points.Add(tmpac);
                }*/
                //签到
                List<Checkins> tmpCK = new List<Checkins>();
                tmpCK = await sqlORM.Queryable<Checkins>().Where(it=>it.user_id == user_id)
                    .ToListAsync();
                foreach (var ck in tmpCK)
                {
                    PointJson.point tmpck = new PointJson.point();
                    tmpck.datetime = ck.sign_in_date;
                    tmpck.type = "checkin";
                    pointJson.points.Add(tmpck);
                }
                // 对 points 数组按照 datetime 降序排序
                pointJson.points = pointJson.points.OrderByDescending(a => a.datetime).ToList();
                List<String> response = new List<String>();
                // 将 pointJson.points 复制给 response
                foreach (var point in pointJson.points)
                {
                    response.Add(point.type);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        public class NoticeJson 
        {
            public DateTime[] notice_dates { get; set; } = new DateTime[0];
            public String[] notice_people { get; set; } = new String[0];
            public String[] notice_contents { get; set; } = new String[0];
        }
        public class NoticeClass
        {
            public  DateTime dateTime { get; set; }
            public String people { get; set; } = "";
            public String? content { get; set; } = "";
        }
        //通知
        [HttpPost]
        public async Task<IActionResult> Notice()
        {
            try
            {
                Console.WriteLine("--------------------------Get Notice--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                
                 // 从请求头中获取传递的JWT令牌
                 string authorizationHeader = Request.Headers["Authorization"].First();
                 //验证 Authorization 请求头是否包含 JWT 令牌
                 if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer"))
                 {
                     Console.WriteLine("未提供有效的JWT");
                     return BadRequest(new { ok = "no", value = "未提供有效的JWT" });
                 }
                 //
                 string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                 // 验证并解析JWT令牌
                 var handler = new JwtSecurityTokenHandler();
                 var tokenS = handler.ReadJwtToken(jwtToken);
                 // 获取JWT令牌中的claims信息
                 string? account = tokenS.Claims.First(claim => claim.Type == "account")?.Value;
                 List<Usr> tempUsr = new List<Usr>();
                 tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account)
                     .ToListAsync();
                 //判断用户是否存在
                 if (tempUsr.Count() == 0)
                 {
                     Console.WriteLine("用户不存在");
                     return Ok(new CustomResponse { ok = "no", value = "错误的用户信息" });//用户账户或密码错误
                 }
                 int user_id = tempUsr.First().user_id;
               
                //int user_id = 12;
                // 创建 NoticeClass 实例
                List<NoticeClass> noticeList = new List<NoticeClass>();

                //获得点赞
                List<Posts> posts = await sqlORM.Queryable<Posts, PublishPost>((p, pp) => p.post_id == pp.post_id)
                    .Where((p, pp) => pp.user_id == user_id)
                    .Select((p, pp) => p)
                    .ToListAsync();
                List<LikePost> approvePosts = await sqlORM.Queryable<LikePost, Posts>((ap, p) => ap.post_id == p.post_id)
                    .Where(ap => posts.Select(post => post.post_id).Contains(ap.post_id))
                    .Select((ap, p) => ap)
                    .ToListAsync();
                foreach (var approvePost in approvePosts)
                {
                    Usr matchingUser = await sqlORM.Queryable<Usr>().SingleAsync(u => u.user_id == approvePost.user_id);

                    if (matchingUser != null)
                    {
                        NoticeClass notice = new NoticeClass
                        {
                            dateTime = approvePost.createDateTime,
                            people = matchingUser.userName,
                            content = "like"
                        };
                        noticeList.Add(notice);
                    }
                }

                //获得收藏
                List<Posts> posts2 = await sqlORM.Queryable<Posts, PublishPost>((p, pp) => p.post_id == pp.post_id)
                    .Where((p, pp) => pp.user_id == user_id)
                    .Select((p, pp) => p)
                    .ToListAsync();
                List<CollectPost> followPosts = await sqlORM.Queryable<CollectPost, Posts>((ap, p) => ap.post_id == p.post_id)
                    .Where(ap => posts2.Select(post => post.post_id).Contains(ap.post_id))
                    .Select((ap, p) => ap)
                    .ToListAsync();
                foreach (var a in followPosts)
                {
                    Usr matchingUser = await sqlORM.Queryable<Usr>().SingleAsync(u => u.user_id == a.user_id);

                    if (matchingUser != null)
                    {
                        NoticeClass notice = new NoticeClass
                        {
                            dateTime = a.createDateTime,
                            people = matchingUser.userName,
                            content = "collect"
                        };
                        noticeList.Add(notice);
                    }
                }
                //评论帖子
                List<Posts> posts3 = await sqlORM.Queryable<Posts, PublishPost>((p, pp) => p.post_id == pp.post_id)
                    .Where((p, pp) => pp.user_id == user_id)
                    .Select((p, pp) => p)
                    .ToListAsync();
                List<Comments> commentPosts = await sqlORM.Queryable<Comments, Posts>((ap, p) => ap.post_id == p.post_id)
                    .Where(ap => posts3.Select(post => post.post_id).Contains(ap.post_id))
                    .Select((ap, p) => ap)
                    .ToListAsync();
                foreach (var a in commentPosts)
                {
                    Usr matchingUser = await sqlORM.Queryable<Usr>().SingleAsync(u => u.user_id == a.user_id);

                    if (matchingUser != null)
                    {
                        NoticeClass notice = new NoticeClass
                        {
                            dateTime = a.publishDateTime,
                            people = matchingUser.userName,
                            content = "comment"
                        };
                        noticeList.Add(notice);
                    }
                }
                //站务通知
                List<Notice> tmpnotice = new List<Notice>();
                tmpnotice = await sqlORM.Queryable<Notice>().Where(it=>true).ToListAsync();
                foreach(var t in tmpnotice)
                {
                    if (t.receiver == 0)
                    {
                        NoticeClass notice = new NoticeClass
                        {
                            dateTime = t.publishdatetime,
                            people = "notice",
                            content = t.text,
                        };
                        noticeList.Add(notice);
                    }
                    if(t.receiver == user_id)
                    {
                        NoticeClass notice = new NoticeClass
                        {
                            dateTime = t.publishdatetime,
                            people = "notice",
                            content = "你的帖子："+t.text+" 已被管理员删除，请注意遵守社区秩序",
                        };
                        noticeList.Add(notice);
                    }
                }
                // 对 notices 数组按照 datetime 降序排序
                noticeList = noticeList.OrderByDescending(a => a.dateTime).ToList();

                NoticeJson response = new NoticeJson();
                List<DateTime> timeList = new List<DateTime>();
                List<String> peopleList = new List<string>();
                List<String> contentList = new List<string>();
                foreach (var notice in noticeList)
                {
                    timeList.Add(notice.dateTime);
                    peopleList.Add(notice.people);
                    contentList.Add(notice.content ?? ""); 
                }
                response.notice_people = peopleList.ToArray();
                response.notice_dates = timeList.ToArray();
                response.notice_contents = contentList.ToArray();
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UserCheckin()
        {
            try
            {
                Console.WriteLine("--------------------------Get userAction--------------------------");
                Console.WriteLine("Get UserCheckin");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                
                // 从请求头中获取传递的JWT令牌
                string authorizationHeader = Request.Headers["Authorization"].First();
                //验证 Authorization 请求头是否包含 JWT 令牌
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer"))
                {
                    Console.WriteLine("未提供有效的JWT");
                    return BadRequest(new { ok = "no", value = "未提供有效的JWT" });
                }
                //
                string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                // 验证并解析JWT令牌
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadJwtToken(jwtToken);
                // 获取JWT令牌中的claims信息
                string? account = tokenS.Claims.FirstOrDefault(claim => claim.Type == "account")?.Value;
                List<Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account)
                    .ToListAsync();
                //判断用户是否存在
                if (tempUsr.Count() == 0)
                {
                    Console.WriteLine("用户不存在");
                    return Ok(new CustomResponse { ok = "no", value = "错误的用户信息" });//用户账户或密码错误
                }
                int user_id = tempUsr.First().user_id;

                DateTime dateTime = DateTime.Now;
                Checkins checkins = new Checkins();
                checkins.user_id = user_id;
                checkins.sign_in_date = dateTime;
                Usr user = tempUsr.First();
                user.userPoint += 5;
                int count = await sqlORM.Insertable(checkins).ExecuteCommandAsync();
                if(count > 0)
                {
                    int count2 = await sqlORM.Updateable(user).ExecuteCommandAsync();
                    if(count2 > 0) {
                        Console.WriteLine("签到 积分+5");
                        Console.WriteLine("checkin success");
                        Console.WriteLine("userid:  " + user_id + "  checkinTime:   " + dateTime);
                        return Ok(new CustomResponse { ok = "yes", value = "签到成功" });
                    }
                    else
                    {
                        Console.WriteLine("checkin fail");
                        return Ok(new CustomResponse { ok = "yes", value = "签到失败" });
                    }
                }
                else
                {
                    Console.WriteLine("checkin fail");
                    return Ok(new CustomResponse { ok = "yes", value = "签到失败" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UserCheckTime()
        {
            try
            {
                Console.WriteLine("--------------------------Get UserCheckin--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                Console.WriteLine("Get userAction");
                // 从请求头中获取传递的JWT令牌
                string authorizationHeader = Request.Headers["Authorization"].First();
                //验证 Authorization 请求头是否包含 JWT 令牌
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer"))
                {
                    Console.WriteLine("未提供有效的JWT");
                    return BadRequest(new { ok = "no", value = "未提供有效的JWT" });
                }
                //
                string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                // 验证并解析JWT令牌
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadJwtToken(jwtToken);
                // 获取JWT令牌中的claims信息
                string? account = tokenS.Claims.FirstOrDefault(claim => claim.Type == "account")?.Value;
                List<Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account)
                    .ToListAsync();
                //判断用户是否存在
                if (tempUsr.Count() == 0)
                {
                    Console.WriteLine("用户不存在");
                    return Ok(new CustomResponse { ok = "no", value = "错误的用户信息" });//用户账户或密码错误
                }
                int user_id = tempUsr.First().user_id;


                List<DateTime> times = new List<DateTime>();
                List<Checkins> checkins = new List<Checkins>();
                checkins = await sqlORM.Queryable<Checkins>().Where(it => it.user_id == user_id)
                    .ToListAsync();
                foreach (var check in checkins)
                {
                    times.Add(check.sign_in_date);
                }
                DateTime[] timesArray = times.ToArray();
                return Ok(timesArray);
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        private class Followed
        {
            public int user_id { get; set; }
            public String userName { get; set; } = "";
            public String uft { get; set; } = "";
            public int userpoint { get; set; }
            public String signature { get; set; } = "";
            public int follownum { get;set; }
            public int likenum { get;set; }
            public int fansnum { get;set; }
            public string avatar { get; set; } = "";        
            public int isfollowed { get;set; }
        }
        //关注列表
        [HttpPost]
        public async Task<IActionResult> followList()
        {
            try
            {
                Console.WriteLine("--------------------------Get followList--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                
                // 从请求头中获取传递的JWT令牌
                string authorizationHeader = Request.Headers["Authorization"].First();
                //验证 Authorization 请求头是否包含 JWT 令牌
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer"))
                {
                    Console.WriteLine("未提供有效的JWT");
                    return BadRequest(new { ok = "no", value = "未提供有效的JWT" });
                }
                //
                string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                // 验证并解析JWT令牌
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadJwtToken(jwtToken);
                // 获取JWT令牌中的claims信息
                string? account = tokenS.Claims.FirstOrDefault(claim => claim.Type == "account")?.Value;
                List<Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account)
                    .ToListAsync();
                //判断用户是否存在
                if (tempUsr.Count() == 0)
                {
                    Console.WriteLine("用户不存在");
                    return Ok(new CustomResponse { ok = "no", value = "错误的用户信息" });//用户账户或密码错误
                }
                int user_id = tempUsr.First().user_id;
                //
                
                List<Followed> followed = new List<Followed>();
                List<Follow> tmpF = new List<Follow>();
                tmpF = await sqlORM.Queryable<Follow>().Where(it => it.follower_id == user_id)
                    .ToListAsync();
                foreach (var f in tmpF)
                {
                    Usr user = await sqlORM.Queryable<Usr>().Where(it => it.user_id == f.follow_id)
                        .FirstAsync();
                    Followed t = new Followed();
                    t.user_id = user.user_id;
                    t.userName =user.userName;
                    t.signature =user.signature;
                    t.avatar =user.avatar;
                    t.userpoint = user.userPoint;
                    //主队
                    List<UserFavouriteTeam> tmpUFT_id = new List<UserFavouriteTeam>();
                    tmpUFT_id = await sqlORM.Queryable<UserFavouriteTeam>().Where(it => it.user_id == user.user_id)
                        .ToListAsync();
                    int UFT_id = tmpUFT_id.Count() != 0 ? tmpUFT_id.First().team_id : 0;
                    List<Team> tmpUFT = new List<Team>();
                    tmpUFT = await sqlORM.Queryable<Team>().Where(it => it.team_id == UFT_id)
                        .ToListAsync();
                    String UFT = tmpUFT.Count() != 0 ? tmpUFT.First().chinesename : "暂无主队";
                    t.uft = UFT;
                    
                    //点赞数
                    List<PublishPost> tmpPP = new List<PublishPost>();
                    tmpPP = await sqlORM.Queryable<PublishPost>().Where(it => it.user_id == user.user_id)
                        .ToListAsync();
                    int approvalNum = 0;
                    foreach (var pp in tmpPP)
                    {
                        List<Posts> tmpPost = new List<Posts>();
                        tmpPost = await sqlORM.Queryable<Posts>().Where(it => it.post_id == pp.post_id)
                            .ToListAsync();
                        approvalNum += tmpPost.Count() != 0 ? tmpPost.First().approvalNum : 0;
                    }
                    t.likenum=approvalNum;
                    //关注数
                    t.follownum = user.follownumber;

                    //粉丝数
                    t.fansnum = user.followednumber;
                 
                    //是否关注
                    List<Follow> tmpf = new List<Follow> ();
                    tmpf = await sqlORM.Queryable<Follow>().Where(it => it.follow_id == user.user_id && it.follower_id==user_id)
                        .ToListAsync();
                    t.isfollowed = tmpf.Count() > 0 ? 1 : 0;

                    if (user != null) { followed.Add(t); };
                }
               
                return Ok(followed);
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        //粉丝列表
        [HttpPost]
        public async Task<IActionResult> fansList()
        {
            try
            {
                Console.WriteLine("--------------------------Get fansList--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
               
                // 从请求头中获取传递的JWT令牌
                string authorizationHeader = Request.Headers["Authorization"].First();
                //验证 Authorization 请求头是否包含 JWT 令牌
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer"))
                {
                    Console.WriteLine("未提供有效的JWT");
                    return BadRequest(new { ok = "no", value = "未提供有效的JWT" });
                }
                //
                string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                // 验证并解析JWT令牌
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadJwtToken(jwtToken);
                // 获取JWT令牌中的claims信息
                string? account = tokenS.Claims.First(claim => claim.Type == "account")?.Value;
                List<Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account)
                    .ToListAsync();
                //判断用户是否存在
                if (tempUsr.Count() == 0)
                {
                    Console.WriteLine("用户不存在");
                    return Ok(new CustomResponse { ok = "no", value = "错误的用户信息" });//用户账户或密码错误
                }
                int user_id = tempUsr.First().user_id;
                //

                List<Followed> followed = new List<Followed>();
                List<Follow> tmpF = new List<Follow>();
                tmpF = await sqlORM.Queryable<Follow>().Where(it => it.follow_id == user_id)
                    .ToListAsync();
                foreach (var f in tmpF)
                {
                    Usr user = await sqlORM.Queryable<Usr>().Where(it => it.user_id == f.follower_id)
                        .FirstAsync();
                    Followed t = new Followed();
                    t.user_id = user.user_id;
                    t.userName = user.userName;
                    t.signature = user.signature;
                    t.avatar = user.avatar;
                    t.userpoint = user.userPoint;
                    //主队
                    List<UserFavouriteTeam> tmpUFT_id = new List<UserFavouriteTeam>();
                    tmpUFT_id = await sqlORM.Queryable<UserFavouriteTeam>().Where(it => it.user_id == user.user_id)
                        .ToListAsync();
                    int UFT_id = tmpUFT_id.Count() != 0 ? tmpUFT_id.First().team_id : 0;
                    List<Team> tmpUFT = new List<Team>();
                    tmpUFT = await sqlORM.Queryable<Team>().Where(it => it.team_id == UFT_id)
                        .ToListAsync();
                    String UFT = tmpUFT.Count() != 0 ? tmpUFT.First().chinesename : "暂无主队";
                    t.uft = UFT;

                    //点赞数
                    List<PublishPost> tmpPP = new List<PublishPost>();
                    tmpPP = await sqlORM.Queryable<PublishPost>().Where(it => it.user_id == user.user_id)
                        .ToListAsync();
                    int approvalNum = 0;
                    foreach (var pp in tmpPP)
                    {
                        List<Posts> tmpPost = new List<Posts>();
                        tmpPost = await sqlORM.Queryable<Posts>().Where(it => it.post_id == pp.post_id)
                            .ToListAsync();
                        approvalNum += tmpPost.Count() != 0 ? tmpPost.First().approvalNum : 0;
                    }
                    t.likenum = approvalNum;
                    //关注数
                    t.follownum = user.follownumber;

                    //粉丝数
                    t.fansnum = user.followednumber;

                    //是否关注
                    List<Follow> tmpf = new List<Follow>();
                    tmpf = await sqlORM.Queryable<Follow>().Where(it => it.follow_id == user.user_id && it.follower_id == user_id)
                        .ToListAsync();
                    t.isfollowed = tmpf.Count() > 0 ? 1 : 0;

                    if (user != null) { followed.Add(t); };
                }

                return Ok(followed);
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        public class TeamsJson
        {
            public string chinesename { get;set; } = "";
            public string logo { get; set; } = "";
        }
        [HttpPost]
        public async Task<IActionResult> getallteam()
        {
            try
            {
                Console.WriteLine("--------------------------Get getallteam--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

                // 从请求头中获取传递的JWT令牌
                string authorizationHeader = Request.Headers["Authorization"].First();
                //验证 Authorization 请求头是否包含 JWT 令牌
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer"))
                {
                    Console.WriteLine("未提供有效的JWT");
                    return BadRequest(new { ok = "no", value = "未提供有效的JWT" });
                }
                //
                string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                // 验证并解析JWT令牌
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadJwtToken(jwtToken);
                // 获取JWT令牌中的claims信息
                string? account = tokenS.Claims.FirstOrDefault(claim => claim.Type == "account")?.Value;
                List<Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account)
                    .ToListAsync();
                //判断用户是否存在
                if (tempUsr.Count() == 0)
                {
                    Console.WriteLine("用户不存在");
                    return Ok(new CustomResponse { ok = "no", value = "错误的用户信息" });//用户账户或密码错误
                }
                int user_id = tempUsr.First().user_id;

                List<Team> allteams = new List<Team>();
                allteams = await sqlORM.Queryable<Team>().ToListAsync();
                List<TeamsJson> tmpTeams = new List<TeamsJson>();
                Team uft = new Team();
                TeamsJson tmpteam = new TeamsJson();
                UserFavouriteTeam userFavouriteTeam = await sqlORM.Queryable<UserFavouriteTeam>().SingleAsync(it=>it.user_id== tempUsr.First().user_id);
                if (userFavouriteTeam != null)
                {
                    uft = await sqlORM.Queryable<Team>().SingleAsync(it => it.team_id == userFavouriteTeam.team_id);
                    tmpteam.chinesename = uft.chinesename;
                    tmpteam.logo = uft.logo;
                }
                else
                {
                    tmpteam.chinesename = "无主队";
                    tmpteam.logo = "/";
                }
                tmpTeams.Add(tmpteam);

                foreach (var t in allteams)
                {
                    TeamsJson tmp = new TeamsJson();
                    tmp.chinesename = t.chinesename;
                    tmp.logo = t.logo;
                    tmpTeams.Add(tmp);
                }
                TeamsJson[] teamsJson = tmpTeams.ToArray();
                return Ok(teamsJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        public class ThemeJson
        {
            public int id { get; set; }
            public string name { get; set; } = "";
            public string image1 { get; set; } = "";
            public string image2 { get; set; } = "";
            public string image3 { get; set; } = "";
        }
        [HttpPost]
        public async Task<IActionResult> getalltheme()
        {
            try
            {
                Console.WriteLine("--------------------------Get getalltheme--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                
                // 从请求头中获取传递的JWT令牌
                string authorizationHeader = Request.Headers["Authorization"].First();
                //验证 Authorization 请求头是否包含 JWT 令牌
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer"))
                {
                    Console.WriteLine("未提供有效的JWT");
                    return BadRequest(new { ok = "no", value = "未提供有效的JWT" });
                }
                //
                string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                // 验证并解析JWT令牌
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadJwtToken(jwtToken);
                // 获取JWT令牌中的claims信息
                string? account = tokenS.Claims.FirstOrDefault(claim => claim.Type == "account")?.Value;
                List<Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account)
                    .ToListAsync();
                //判断用户是否存在
                if (tempUsr.Count() == 0)
                {
                    Console.WriteLine("用户不存在");
                    return Ok(new CustomResponse { ok = "no", value = "错误的用户信息" });//用户账户或密码错误
                }
                int user_id = tempUsr.First().user_id;
                /*
                int user_id = 4;
                List<Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.user_id == user_id)
                    .ToListAsync();*/
                List<Theme> allthemes = new List<Theme>();
                allthemes = await sqlORM.Queryable<Theme>().ToListAsync();
                List<ThemeJson> tmpThemes = new List<ThemeJson>();

                

                foreach (var t in allthemes)
                {
                    ThemeJson tmp = new ThemeJson();
                    tmp.id = t.id;
                    tmp.name = t.name;
                    tmp.image1 = t.image1;
                    tmp.image2 = t.image2;
                    tmp.image3 = t.image3;
                    tmpThemes.Add(tmp);
                }
                tmpThemes=tmpThemes.OrderBy(t => t.id).ToList();

                Theme userTheme = await sqlORM.Queryable<Theme>().SingleAsync(it => it.id == tempUsr.First().themeType);
                ThemeJson userThemejson = new ThemeJson();
                userThemejson.id = userTheme.id;
                userThemejson.name = userTheme.name;
                userThemejson.image1 = userTheme.image1;
                userThemejson.image2 = userTheme.image2;
                userThemejson.image3 = userTheme.image3;
                tmpThemes.Add(userThemejson);

                ThemeJson[] themesJson = tmpThemes.ToArray();
                return Ok(themesJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> gettheme()
        {
            try
            {
                Console.WriteLine("--------------------------Get gettheme--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

                // 从请求头中获取传递的JWT令牌
                string authorizationHeader = Request.Headers["Authorization"].First();
                //验证 Authorization 请求头是否包含 JWT 令牌
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer"))
                {
                    Console.WriteLine("未提供有效的JWT");
                    return BadRequest(new { ok = "no", value = "未提供有效的JWT" });
                }
                //
                string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                // 验证并解析JWT令牌
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadJwtToken(jwtToken);
                // 获取JWT令牌中的claims信息
                string? account = tokenS.Claims.FirstOrDefault(claim => claim.Type == "account")?.Value;
                List<Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account)
                    .ToListAsync();
                //判断用户是否存在
                if (tempUsr.Count() == 0)
                {
                    Console.WriteLine("用户不存在");
                    return Ok(new CustomResponse { ok = "no", value = "错误的用户信息" });//用户账户或密码错误
                }
                int user_id = tempUsr.First().user_id;
                /*
                int user_id = 4;
                List<Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.user_id == user_id)
                    .ToListAsync();*/
                List<Theme> allthemes = new List<Theme>();
                allthemes = await sqlORM.Queryable<Theme>().ToListAsync();
                List<ThemeJson> tmpThemes = new List<ThemeJson>();

                Theme userTheme = await sqlORM.Queryable<Theme>().SingleAsync(it => it.id == tempUsr.First().themeType);
                ThemeJson userThemejson = new ThemeJson();
                userThemejson.id = userTheme.id;
                userThemejson.name = userTheme.name;
                userThemejson.image1 = userTheme.image1;
                userThemejson.image2 = userTheme.image2;
                userThemejson.image3 = userTheme.image3;
                Console.WriteLine("userAccount" +account);
                Console.WriteLine("userThemejson.id:"+ userThemejson.id);
                Console.WriteLine("userThemejson.name:" + userThemejson.name);
                Console.WriteLine("userThemejson.image1:" + userThemejson.image1);
                Console.WriteLine("userThemejson.image2:" + userThemejson.image2);
                Console.WriteLine("userThemejson.image3:" + userThemejson.image3);
                return Ok(userThemejson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        public class ModifyThemeJson
        {
            public int theme_id { get;set; }
        }
        [HttpPost]
        public async Task<IActionResult> modifytheme([FromBody]ModifyThemeJson json)
        {
            try
            {
                Console.WriteLine("--------------------------Get modifytheme--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

                // 从请求头中获取传递的JWT令牌
                string authorizationHeader = Request.Headers["Authorization"].First();
                //验证 Authorization 请求头是否包含 JWT 令牌
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer"))
                {
                    Console.WriteLine("未提供有效的JWT");
                    return BadRequest(new { ok = "no", value = "未提供有效的JWT" });
                }
                //
                string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                // 验证并解析JWT令牌
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadJwtToken(jwtToken);
                // 获取JWT令牌中的claims信息
                string? account = tokenS.Claims.FirstOrDefault(claim => claim.Type == "account")?.Value;
                List<Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account)
                    .ToListAsync();
                Usr usr = tempUsr.First();
                //判断用户是否存在
                if (tempUsr.Count() == 0)
                {
                    Console.WriteLine("用户不存在");
                    return Ok(new CustomResponse { ok = "no", value = "错误的用户信息" });//用户账户或密码错误
                }
                int user_id = tempUsr.First().user_id;
                /*
                int user_id = 4;
                List<Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.user_id == user_id)
                    .ToListAsync();
                Usr usr = tempUsr.FirstOrDefault();*/

                Theme modify = await sqlORM.Queryable<Theme>().SingleAsync(it=>it.id == json.theme_id);
                int count = 0;
                if (modify != null)
                {
                    usr.themeType = json.theme_id;
                    count = await sqlORM.Updateable(usr).ExecuteCommandAsync();
                    if (count > 0)
                    {
                        Console.WriteLine("modify success");
                        return Ok(new CustomResponse { ok = "yes", value = "success" });
                    }
                    else
                    {
                        Console.WriteLine("modify fail");
                        return Ok(new CustomResponse { ok = "no", value = "fail" });
                    }
                }
                else
                {
                    Console.WriteLine("no such theme");
                    return Ok(new CustomResponse { ok = "no", value = "no such theme" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        public class PostinfoJson
        {
            public int post_id { get; set; }
            public string title { get; set; } = "";
            public string contains { get; set; } = "";
            public int approvalNum { get; set; }
            public int collectNum { get;set; }
        }
        
        [HttpGet]
        public async Task<IActionResult> GetMyPost()
        {
            try
            {
                Console.WriteLine("--------------------------Get GetMyPost--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

                // 从请求头中获取传递的JWT令牌
                string authorizationHeader = Request.Headers["Authorization"].First();
                //验证 Authorization 请求头是否包含 JWT 令牌
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer"))
                {
                    Console.WriteLine("未提供有效的JWT");
                    return BadRequest(new { ok = "no", value = "未提供有效的JWT" });
                }
                //
                string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                // 验证并解析JWT令牌
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadJwtToken(jwtToken);
                // 获取JWT令牌中的claims信息
                string? account = tokenS.Claims.FirstOrDefault(claim => claim.Type == "account")?.Value;
                List<Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account)
                    .ToListAsync();
                Usr usr = tempUsr.First();
                //判断用户是否存在
                if (tempUsr.Count() == 0)
                {
                    Console.WriteLine("用户不存在");
                    return Ok(new CustomResponse { ok = "no", value = "错误的用户信息" });//用户账户或密码错误
                }
                int user_id = tempUsr.First().user_id;
                /*
                               int user_id = 2;
                               List<Usr> tempUsr = new List<Usr>();
                               tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.user_id == user_id)
                                   .ToListAsync();
                               Usr usr = tempUsr.FirstOrDefault();               */

                // 获取指定 user_id 的帖子
                List<Posts> myPosts = await sqlORM.Queryable<Posts, PublishPost>((p, pp) => new object[]
                    {
                JoinType.Inner, p.post_id == pp.post_id
                    })
                    .Where((p, pp) => pp.user_id == user_id && p.isBanned != 1)
                    .Select((p, pp) => p)
                    .ToListAsync();
                List< PostinfoJson> postinfo = new List<PostinfoJson>();
                foreach(var post in myPosts)
                {
                    PostinfoJson tmp=new PostinfoJson();
                    tmp.title = post.title;
                    tmp.contains = post.contains;
                    tmp.approvalNum = post.approvalNum;
                    tmp.collectNum = post.favouriteNum;
                    tmp.post_id= post.post_id;
                    postinfo.Add(tmp);
                }
                return Ok(postinfo.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetMyCollectPost()
        {
            try
            {
                Console.WriteLine("--------------------------Get GetMyCollectPost--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

                // 从请求头中获取传递的JWT令牌
                string authorizationHeader = Request.Headers["Authorization"].First();
                //验证 Authorization 请求头是否包含 JWT 令牌
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer"))
                {
                    Console.WriteLine("未提供有效的JWT");
                    return BadRequest(new { ok = "no", value = "未提供有效的JWT" });
                }
                //
                string jwtToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                // 验证并解析JWT令牌
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadJwtToken(jwtToken);
                // 获取JWT令牌中的claims信息
                string? account = tokenS.Claims.FirstOrDefault(claim => claim.Type == "account")?.Value;
                List<Usr> tempUsr = new List<Usr>();
                tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account)
                    .ToListAsync();
                Usr usr = tempUsr.First();
                //判断用户是否存在
                if (tempUsr.Count() == 0)
                {
                    Console.WriteLine("用户不存在");
                    return Ok(new CustomResponse { ok = "no", value = "错误的用户信息" });//用户账户或密码错误
                }
                int user_id = tempUsr.First().user_id;    
 /*               
                               int user_id = 12;
                               List<Usr> tempUsr = new List<Usr>();
                               tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.user_id == user_id)
                                   .ToListAsync();
                               Usr usr = tempUsr.FirstOrDefault();
*/
                // 获取指定 user_id 的帖子
                List<Posts> myPosts = await sqlORM.Queryable<Posts, CollectPost>((p, pp) => new object[]
                    {
                JoinType.Inner, p.post_id == pp.post_id
                    })
                    .Where((p, pp) => pp.user_id == user_id && p.isBanned!=1)
                    .Select((p, pp) => p)
                    .ToListAsync();
                List<PostinfoJson> postinfo = new List<PostinfoJson>();
                foreach (var post in myPosts)
                {
                    PostinfoJson tmp = new PostinfoJson();
                    tmp.title = post.title;
                    tmp.contains = post.contains;
                    tmp.approvalNum = post.approvalNum;
                    tmp.collectNum = post.favouriteNum;
                    tmp.post_id = post.post_id;
                    postinfo.Add(tmp);
                }
                Console.WriteLine("myPosts:" + myPosts.Count());
                Console.WriteLine("postinfo:" + postinfo.Count());
                return Ok(postinfo.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        public class userinfoJson
        {
            public int author_id { get; set; }
        }
        public class UserJson
        {
            public string name { get; set; } = "";
            public string avatar { get; set; } = "";
            public string uft { get; set; } = "";
            public string signature { get; set; } = "";
            public int follownum { get; set; }
            public int followednum { get; set; }
            public int likenum { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> UserInfo([FromBody] userinfoJson json)
        {
            try
            {
                Console.WriteLine("--------------------------Get UserInfo--------------------------");
                ORACLEconn ORACLEConnectTry = new ORACLEconn();
                if (!ORACLEConnectTry.getConn())
                {
                    Console.WriteLine("数据库连接失败");
                    return BadRequest("数据库连接失败");
                };
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

                //找到发帖人

                Usr User = await sqlORM.Queryable<Usr>().SingleAsync(it => it.user_id == json.author_id);
                if (User == null)
                {
                    return BadRequest();
                }
                UserJson response = new UserJson();
                response.signature = User.signature;
                response.name = User.userName;
                response.avatar = User.avatar;
                response.followednum = User.followednumber;
                response.follownum = User.follownumber;
                //点赞数
                List<PublishPost> tmpPP = new List<PublishPost>();
                tmpPP = await sqlORM.Queryable<PublishPost>().Where(it => it.user_id == json.author_id)
                    .ToListAsync();
                int approvalNum = 0;
                foreach (var pp in tmpPP)
                {
                    List<Posts> tmpPost = new List<Posts>();
                    tmpPost = await sqlORM.Queryable<Posts>().Where(it => it.post_id == pp.post_id)
                        .ToListAsync();
                    approvalNum += tmpPost.Count() != 0 ? tmpPost.FirstOrDefault()?.approvalNum ?? 0 : 0;
                }
                response.likenum = approvalNum;
                //主队
                UserFavouriteTeam uft = await sqlORM.Queryable<UserFavouriteTeam>().SingleAsync(it => it.user_id == json.author_id);
                if (uft == null)
                    response.uft = "暂无主队";
                else
                {
                    Team team = await sqlORM.Queryable<Team>().SingleAsync(it => it.team_id == uft.team_id);
                    if (team != null)
                        response.uft = team.chinesename;
                    else
                        response.uft = "暂无主队";
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("errorResponse: " + ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "数据库连接错误" });//用户账户或密码错误
            }
        }
    }
}