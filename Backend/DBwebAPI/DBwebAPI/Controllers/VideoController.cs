using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Linq.Expressions;
using DBwebAPI;
using DBwebAPI.Models;
using DBwebAPI.Relations;

namespace DBwebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VideoController : Controller
    {
        public class GetVideoRequest
        {
            public int num { get; set; }
            public string matchTag { get; set; }
            public string propertyTag { get; set; }
        }
        public class SearchVideoRequest
        {
            public string keyword { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> GetVideoRandomly(GetVideoRequest json)
        {
            Console.WriteLine("--------------------------GetVideoRandomly--------------------------");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            int num = json.num;
            string mtag = json.matchTag;
            string ptag = json.propertyTag;
            if (ORACLEConnectTry.getConn() == true)
            {
                try
                {
                    SqlSugarScope db = ORACLEConnectTry.sqlORM;
                    int sum = db.Queryable<Video>().Count();
                    if (num == -1 || num > sum)
                    {
                        num = sum;
                    }
                    List<Video> videos = new List<Video>();
                    int case_num = 0;
                    if (mtag != "")
                    {
                        case_num += 2;
                    }
                    if (ptag != "")
                    {
                        case_num += 1;
                    }
                    switch (case_num)
                    {
                        //mtag与ptag都不需要
                        case 0:
                            videos = await db.Queryable<Video>().OrderBy(st => SqlFunc.GetRandom()).Take(num).ToListAsync();
                            break;
                        //需要ptag
                        case 1:
                            videos = await db.Queryable<Video>().Where(v => v.propertyTag == ptag).OrderBy(st => SqlFunc.GetRandom()).Take(num).ToListAsync();
                            break;
                        //需要mtag
                        case 2:
                            videos = await db.Queryable<Video>().Where(v => v.matchTag == mtag).OrderBy(st => SqlFunc.GetRandom()).Take(num).ToListAsync();
                            break;
                        //需要mtag与ptag
                        case 3:
                            videos = await db.Queryable<Video>().Where(v => v.matchTag == mtag && v.propertyTag == ptag).OrderBy(st => SqlFunc.GetRandom()).Take(num).ToListAsync();
                            break;
                    }
                    return Ok(new CustomResponse { ok = "yes", value = videos });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return Ok(new CustomResponse { ok = "no", value = "服务器内部错误" });
                }
            }
            else
            {
                Console.WriteLine("数据库连接失败");
                return Ok(new CustomResponse { ok = "no", value = "数据库连接失败！" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SearchVideo(SearchVideoRequest json)
        {
            Console.WriteLine("--------------------------SearchVideo--------------------------");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            string key = json.keyword;
            if(ORACLEConnectTry.getConn() == true)
            {
                try
                {
                    SqlSugarScope db = ORACLEConnectTry.sqlORM;
                    List<Video> videos = new List<Video>();
                    videos = await db.Queryable<Video>().Where(v => v.title.Contains(key) || v.matchTag.Contains(key) || v.propertyTag.Contains(key)).ToListAsync();
                    Func<Video, int> evaluate = (v) =>
                    {
                        int val = 0;
                        if(v.title.Contains(key))
                        {
                            val += 4;
                        }
                        if(v.matchTag.Contains(key))
                        {
                            val += 1;
                        }
                        if(v.propertyTag.Contains(key))
                        {
                            val += 2;
                        }
                        return val;
                    };
                    videos.Sort((a,b) =>
                    {
                        int na = evaluate(a), nb = evaluate(b);
                        return (na == nb ? 0 : (na > nb ? -1 : 1));
                    });
                    return Ok(new CustomResponse { ok = "yes", value = videos });
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return Ok(new CustomResponse { ok = "no", value = "服务器内部错误" });
                }
            }
            else
            {
                return Ok(new CustomResponse { ok = "no", value = "数据库连接失败！" });
            }
        }
    }
}
