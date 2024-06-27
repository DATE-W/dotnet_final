using DBwebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using static DBwebAPI.Controllers.LoginController;
using static DBwebAPI.Models.NoticeModel;

namespace DBwebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HighlightController:ControllerBase
    {

        public class getHighlightsVal
        {
            public string? photo { get; set; }
            public string? videoUrl { get; set; }
        }


        [HttpGet]
        public async Task<IActionResult> getHighlights()
        {
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            ORACLEConnectTry.getConn();
            try
            {
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                var ans =await sqlORM.Queryable<Highlight>()
                    .Select(it => new getHighlightsVal
                    {
                        photo = it.photo,
                        videoUrl = it.videoUrl
                    })
                    .ToListAsync();

                return Ok(new CustomResponse { ok = "yes", value = ans });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "获得失败" });
            }
        }
    }
}
