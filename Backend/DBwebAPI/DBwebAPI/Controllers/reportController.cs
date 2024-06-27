using SqlSugar;
using DBwebAPI.Models;
using Microsoft.AspNetCore.Mvc;
//using static DBwebAPI.Controllers.noticeController;
using static DBwebAPI.Models.NoticeModel;
using static DBwebAPI.Controllers.Register;
using System.Runtime.CompilerServices;

namespace DBwebAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class reportController : ControllerBase
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

        [HttpGet]
        public async Task<IActionResult> getUsrInfo()
        {
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            ORACLEConnectTry.getConn();
            try
            {
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

                var ans = await sqlORM.Queryable<Usr>()
                    .Select(it => new UsrInfo
                    {
                        user_id = it.user_id,
                        userName = it.userName,
                        userPoint = it.userPoint,
                        followedNumber = it.followednumber,
                        isbanned = it.isBanned,
                        regDate = it.createDateTime.Value.ToString("yyyy-MM-dd")
                    })
                    .ToListAsync();

                return Ok(new CustomResponse { ok = "yes", value = ans });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "查看失败" });
            }
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
        [HttpGet]
        public async Task<IActionResult> getReportInfo()
        {
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            ORACLEConnectTry.getConn();
            try
            {
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                var ans = await sqlORM.Queryable<Reports>()
                    .LeftJoin<Usr>((r, er) => r.reporter_id == er.user_id)
                    .LeftJoin<PublishPost>((r, er, pp) => r.post_id == pp.post_id)
                    .LeftJoin<Usr>((r, er, pp, ee) => pp.user_id == ee.user_id)
                    .LeftJoin<Posts>((r, er, pp, ee, p) => pp.post_id == p.post_id)
                    .Where((r, er, pp, ee, p)=>r.status=="unhandled")
                    .Select((r, er, pp, ee, p) => new reportInfo
                    {
                        post_id = pp.post_id,
                        reporter_id =r.reporter_id,
                        title = p.title,
                        publisherName = ee.userName,
                        reporterName = er.userName,
                        description = r.descriptions
                    })
                    .ToListAsync();

                return Ok(new CustomResponse { ok = "yes", value = ans });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "查看失败" });
            }
        }


        public class dealReportVal
        {
            public int? reporter_id { get; set; }
            public int? post_id { get; set; }
            public string? reply { get;set; }
        }

        [HttpPost]
        public async Task<IActionResult> cancelReport([FromBody]dealReportVal json)
        {

            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            ORACLEConnectTry.getConn();
            try
            {
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

                int? reporter_id = json.reporter_id;
                int? post_id=json.post_id;
                string? reply = json.reply;

                Console.WriteLine("处理举报中......");

                var result2 = await sqlORM.Updateable<Reports>()
                    .SetColumns(it => it.reply == reply)
                    .Where(it => it.post_id == post_id && it.reporter_id == reporter_id && it.status == "unhandled")
                    .ExecuteCommandAsync();

                var result1 = await sqlORM.Updateable<Reports>()
                    .SetColumns(it => it.status == "failed")
                    .Where(it => it.post_id == post_id && it.reporter_id == reporter_id && it.status == "unhandled")
                    .ExecuteCommandAsync();

                if (result1!=0&&result2!=0)
                {
                    Console.WriteLine("撤回举报成功");
                    return Ok(new CustomResponse { ok = "yes", value = "撤回举报成功" });
                }
                else
                {
                    Console.WriteLine("未找到相应举报");
                    return Ok(new CustomResponse { ok = "no", value = "未找到相应举报" });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "操作失败" });
            }

        }



        [HttpPost]
        public async Task<IActionResult> agreeReport([FromBody] dealReportVal json)
        {

            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            ORACLEConnectTry.getConn();
            try
            {
                
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

                int? reporter_id = json.reporter_id;
                int? post_id = json.post_id;
                string? reply=json.reply;

                Console.WriteLine("处理举报中......");

                var result2 = await sqlORM.Updateable<Reports>()
                    .SetColumns(it => it.reply == reply)
                    .Where(it => it.post_id == post_id && it.reporter_id == reporter_id && it.status == "unhandled")
                    .ExecuteCommandAsync();

                var result1 = await sqlORM.Updateable<Reports>()
                    .SetColumns(it => it.status == "success")
                    .Where(it => it.post_id == post_id && it.reporter_id == reporter_id && it.status == "unhandled")
                    .ExecuteCommandAsync();


                if (result1 != 0&&result2!=0)
                {
                    // 记得设置帖子isBanned
                    int temp = await sqlORM.Updateable<Posts>()
                        .SetColumns(it => it.isBanned == 1)
                        .Where(it => it.post_id == post_id)
                        .ExecuteCommandAsync();
                    if (temp != 0)
                    {
                        Console.WriteLine("处理举报成功");
                        return Ok(new CustomResponse { ok = "yes", value = "处理举报成功" });
                    }
                    else
                    {
                        Console.WriteLine("帖子状态更新失败");
                        return Ok(new CustomResponse { ok = "no", value = "帖子状态更新失败" });
                    }

                }
                else
                {
                    Console.WriteLine("未找到相应举报");
                    return Ok(new CustomResponse { ok = "no", value = "未找到相应举报" });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "操作失败" });
            }

        }


        public class banUsrPara
        {
            public int? user_id { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> banUsr([FromBody] banUsrPara json)
        {

            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            ORACLEConnectTry.getConn();
            try
            {
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                int count =await sqlORM.Updateable<Usr>()
                    .SetColumns(it => it.isBanned == 1)
                    .Where(it => it.user_id == json.user_id)
                    .ExecuteCommandAsync();

                if (count != 0)
                {
                    Console.WriteLine("封禁成功");
                    return Ok(new CustomResponse { ok = "yes", value = "封禁成功" });
                }
                else
                {
                    Console.WriteLine("封禁失败,检查用户id是否存在");
                    return Ok(new CustomResponse { ok = "no", value = "封禁失败,检查用户id是否存在" });
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "操作失败" });
            }

        }


    }
}
