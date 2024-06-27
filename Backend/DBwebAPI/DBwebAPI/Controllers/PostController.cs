using SqlSugar;
using DBwebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using static DBwebAPI.Controllers.Register;
using static DBwebAPI.Controllers.updateTeamController;
using static DBwebAPI.Models.NoticeModel;



namespace DBwebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PostController : ControllerBase
    {
        public class getPostInfoPara
        {
            public int? post_id { get; set; }
        }
        public class getPostInfoVal
        {
            public string? title { get; set; }
            public string? contains { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> getPostInfo([FromBody] getPostInfoPara json)
        {
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            ORACLEConnectTry.getConn();
            try
            {
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                int? post_id = json.post_id;
                var ans = await sqlORM.Queryable<Posts>()
                    .Where(it => it.post_id == post_id)
                    .Select(it => new getPostInfoVal
                    {
                        title = it.title,
                        contains = it.contains
                    })
                    .ToListAsync();

                if (ans.Count() != 0)
                {
                    Console.WriteLine("found ! post id = " + post_id.ToString());
                    return Ok(new CustomResponse { ok = "yes", value = ans[0] });
                }
                else
                {
                    return Ok(new CustomResponse { ok = "no", value = "未知的帖子id" });
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "查看失败" });
            }
        }


        public class  getYestdayInfoVal
        {
            public int? newUsrNum { get; set; }
            public int? newPostNum { get; set; }
            public int? signInNum { get; set; }

        }

        [HttpGet]
        public async Task<IActionResult> getYestdayInfo()
        {
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            ORACLEConnectTry.getConn();
            try
            {
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                getYestdayInfoVal ans = new getYestdayInfoVal();

                DateTime now = DateTime.Now;
                DateTime yestdayStart = new DateTime(now.Year,now.Month,now.Day-1);
                DateTime yestdayEnd = new DateTime(now.Year, now.Month, now.Day);

                DateTime todayStart = new DateTime(now.Year, now.Month, now.Day);
                DateTime todayEnd = new DateTime(now.Year, now.Month, now.Day+1);

                var newUsrNum = await sqlORM.Queryable<Usr>()
                    .Where(it=>it.createDateTime.Value>= yestdayStart&& it.createDateTime.Value < yestdayEnd)
                    .Select(it => SqlFunc.AggregateCount(it.user_id))
                    .ToListAsync();

                ans.newUsrNum = newUsrNum[0];
                Console.WriteLine("new user count = " + ans.newUsrNum.ToString());

                var newPostNum = await sqlORM.Queryable<Posts>()
                    .Where(it => it.publishDateTime >= yestdayStart && it.publishDateTime < yestdayEnd)
                    .Select(it => SqlFunc.AggregateCount(it.post_id))
                    .ToListAsync();

                ans.newPostNum = newPostNum[0];
                Console.WriteLine("new post count = " + ans.newPostNum.ToString());

                var signInNum = await sqlORM.Queryable<Checkins>()
                    .Where(it => it.sign_in_date>= todayStart && it.sign_in_date < todayEnd)
                    .Select(it => SqlFunc.AggregateCount(it.user_id))
                    .ToListAsync();

                ans.signInNum = signInNum[0];
                Console.WriteLine("signin count = " + ans.signInNum.ToString());

                return Ok(new CustomResponse { ok = "yes", value = ans });


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "查看失败" });
            }

        }

    }
}
