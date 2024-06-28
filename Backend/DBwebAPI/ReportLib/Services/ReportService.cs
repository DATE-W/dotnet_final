using AdminLib.Models;
using ReportLib.Models;
using ReportLib.Utils;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AdminLib.Models.NoticeModel;

namespace ReportLib.Services
{
    public class ReportService : IReportService
    {
        public async Task<CustomResponse> agreeReport(dealReportVal json)
        {
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            ORACLEConnectTry.getConn();
            SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

            int? reporter_id = json.reporter_id;
            int? post_id = json.post_id;
            string? reply = json.reply;

            Console.WriteLine("处理举报中......");

            var result2 = await sqlORM.Updateable<Reports>()
                .SetColumns(it => it.reply == reply)
                .Where(it => it.post_id == post_id && it.reporter_id == reporter_id && it.status == "unhandled")
                .ExecuteCommandAsync();

            var result1 = await sqlORM.Updateable<Reports>()
                .SetColumns(it => it.status == "success")
                .Where(it => it.post_id == post_id && it.reporter_id == reporter_id && it.status == "unhandled")
                .ExecuteCommandAsync();


            if (result1 != 0 && result2 != 0)
            {
                // 记得设置帖子isBanned
                int temp = await sqlORM.Updateable<Posts>()
                    .SetColumns(it => it.isBanned == 1)
                    .Where(it => it.post_id == post_id)
                    .ExecuteCommandAsync();
                if (temp != 0)
                {
                    Console.WriteLine("处理举报成功");
                    return (new CustomResponse { ok = "yes", value = "处理举报成功" });
                }
                else
                {
                    Console.WriteLine("帖子状态更新失败");
                    return (new CustomResponse { ok = "no", value = "帖子状态更新失败" });
                }

            }
            else
            {
                Console.WriteLine("未找到相应举报");
                return (new CustomResponse { ok = "no", value = "未找到相应举报" });
            }

        }

        public async Task<CustomResponse> banUsr(banUsrPara json)
        {
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            ORACLEConnectTry.getConn();
            SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
            int count = await sqlORM.Updateable<Usr>()
                .SetColumns(it => it.isBanned == 1)
                .Where(it => it.user_id == json.user_id)
                .ExecuteCommandAsync();

            if (count != 0)
            {
                Console.WriteLine("封禁成功");
                return (new CustomResponse { ok = "yes", value = "封禁成功" });
            }
            else
            {
                Console.WriteLine("封禁失败,检查用户id是否存在");
                return (new CustomResponse { ok = "no", value = "封禁失败,检查用户id是否存在" });
            }
        }

        public async Task<CustomResponse> cancelReport(dealReportVal json)
        {
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            ORACLEConnectTry.getConn();
            SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;

            int? reporter_id = json.reporter_id;
            int? post_id = json.post_id;
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

            if (result1 != 0 && result2 != 0)
            {
                Console.WriteLine("撤回举报成功");
                return (new CustomResponse { ok = "yes", value = "撤回举报成功" });
            }
            else
            {
                Console.WriteLine("未找到相应举报");
                return (new CustomResponse { ok = "no", value = "未找到相应举报" });
            }

        }

        public async Task<CustomResponse> getReportInfo()
        {
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            ORACLEConnectTry.getConn();
            SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
            var ans = await sqlORM.Queryable<Reports>()
                .LeftJoin<Usr>((r, er) => r.reporter_id == er.user_id)
                .LeftJoin<PublishPost>((r, er, pp) => r.post_id == pp.post_id)
                .LeftJoin<Usr>((r, er, pp, ee) => pp.user_id == ee.user_id)
                .LeftJoin<Posts>((r, er, pp, ee, p) => pp.post_id == p.post_id)
                .Where((r, er, pp, ee, p) => r.status == "unhandled")
                .Select((r, er, pp, ee, p) => new reportInfo
                {
                    post_id = pp.post_id,
                    reporter_id = r.reporter_id,
                    title = p.title,
                    publisherName = ee.userName,
                    reporterName = er.userName,
                    description = r.descriptions
                })
                .ToListAsync();

            return (new CustomResponse { ok = "yes", value = ans });
        }

        public async Task<CustomResponse> getUsrInfo()
        {
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            ORACLEConnectTry.getConn();
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

            return (new CustomResponse { ok = "yes", value = ans });
        }
    }
}
