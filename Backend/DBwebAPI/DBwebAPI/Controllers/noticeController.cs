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
    public class noticeController : ControllerBase
    {

        public class createNoticePara
        {
            public int? admin_id { get; set; }
            public string? text { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> createNotice([FromBody] createNoticePara json)
        {
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            ORACLEConnectTry.getConn();
            try
            {
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                Notice notice = new Notice();
                int? id=sqlORM.Queryable<Notice>().Max(it=>it.notice_id);
                if (id.HasValue) {
                    id++;
                }else{
                    id = 0;
                }
                notice.notice_id = id;
                notice.text = json.text;
                notice.publishdatetime = DateTime.Now;

                adminPublishNotice adminPublishNotice = new adminPublishNotice();
                adminPublishNotice.notice_id = id;
                adminPublishNotice.admin_id = json.admin_id;

                bool adminExists = sqlORM.Queryable<Admins>().Any(it => it.admin_id == json.admin_id);
                if (!adminExists)
                {
                    return Ok(new CustomResponse { ok = "no", value = "管理员不存在" });
                }

                int count1=await sqlORM.Insertable<Notice>(notice).ExecuteCommandAsync();
                int count2= await sqlORM.Insertable<adminPublishNotice>(adminPublishNotice).ExecuteCommandAsync();
                if (count1 == 1&&count2==1)
                {
                    Console.WriteLine("new notice id = " + id.ToString());
                    Console.WriteLine("new notice text = "+json.text);
                    return Ok(new CustomResponse { ok="yes",value="发布成功"});
                }
                else
                {
                    Console.WriteLine("publish Notice Failed");
                    return Ok(new CustomResponse { ok = "no", value = "发布失败" });
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "发布失败" });
            }

        }





    }
}
