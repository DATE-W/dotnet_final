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
    public class NewsController : ControllerBase
    {
        public class NewsCompare : IComparer<NewsWithPicture>
        {
            public int Compare(NewsWithPicture? x, NewsWithPicture? y)
            {
                Func<NewsWithPicture, int> eval = tmp =>
                {
                    string t = tmp.newsBody.matchTag;
                    if (t == "中超")
                    {
                        return 1;
                    }
                    if (t == "英超")
                    {
                        return 2;
                    }
                    if (t == "西甲")
                    {
                        return 3;
                    }
                    if (t == "德甲")
                    {
                        return 4;
                    }
                    if (t == "意甲")
                    {
                        return 5;
                    }
                    if (t == "法甲")
                    {
                        return 6;
                    }
                    return -1;
                };
                int nx = eval(x), ny = eval(y);
                return nx == ny ? 0 : (nx < ny ? -1 : 1);
            }
        }
        public class NewsWithPicture
        {
            public News newsBody { get; set; }
            public List<string>? pictureRoutes { get; set; }
        }
        public class InitResponse
        {
            public List<List<NewsWithPicture>> news { get; set; }
            public List<List<Video>> videos { get; set; }
        }
        public class GetNewsRequest
        {
            public int num { get; set; }
            public string matchTag { get; set; }
            public string propertyTag { get; set; }
        }
        public class SearchNewsRequest
        {
            public string keyword { get; set; }
        }
        [HttpGet]
        public async Task<IActionResult> Init()
        {
            Console.WriteLine("GET Login!");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            if(ORACLEConnectTry.getConn() == true)
            {
                try
                {
                    List<NewsWithPicture> news = new List<NewsWithPicture>();
                    SqlSugarScope db = ORACLEConnectTry.sqlORM;
                    List<News> a = new List<News>();
                    List<Video> v = new List<Video>();
                    a.AddRange(await db.Queryable<News>().Where(n => n.propertyTag == "普通").OrderBy(st => SqlFunc.GetRandom()).Take(8).ToListAsync());
                    v.AddRange(await db.Queryable<Video>().OrderBy(st => SqlFunc.GetRandom()).Take(11).ToListAsync());
                    var b = await db.Queryable<News>().
                        Where(n => n.propertyTag == "普通").Select(n => new
                        {
                            index = SqlFunc.RowNumber(n.publishDateTime, n.matchTag),//order by id partition by name
                                                                                     //多字段排序  order by id asc ,name desc
                                                                                     //SqlFunc.RowNumber($"{it.Id} asc ,{it.Name} desc ",$"{it.Name}")
                            news_id = n.news_id,
                            publishDateTime = n.publishDateTime,
                            title = n.title,
                            summary = n.summary,
                            contains = n.contains,
                            matchTag = n.matchTag,
                            propertyTag = n.propertyTag
                        })
                        .MergeTable()//将结果合并成一个表
                        .Where(it => it.index <= 4) //相同的name只取一条记录
                                                     //前20条用Where(it=>it.index2=<=20) 
                        .ToListAsync();
                    foreach(var x in b)
                    {
                        a.Add(new News { news_id = x.news_id, publishDateTime = x.publishDateTime, title = x.title, summary = x.summary, contains = x.contains, matchTag = x.matchTag, propertyTag = x.propertyTag});
                    }
                    a.AddRange(await db.Queryable<News>().Where(n => n.propertyTag == "八卦").OrderBy(st => SqlFunc.GetRandom()).Take(12).ToListAsync());
                    foreach(var n in a)
                    {
                        int id = n.news_id;
                        List<string> pictureRoutes = await db.Queryable<NewsHavePicture>().
                                Where(n => n.news_id == id).
                                Select(nhp => nhp.pictureRoute).
                                ToListAsync();
                        news.Add(new NewsWithPicture { newsBody = n, pictureRoutes = pictureRoutes });
                    }
                    Console.WriteLine(news.Count);
                    Console.WriteLine(news.GetRange(0, 4).Count);
                    InitResponse ret = new InitResponse();
                    ret.news = new List<List<NewsWithPicture>>();
                    ret.videos = new List<List<Video>>();
                    ret.news.Add(news.GetRange(0, 4));
                    ret.news.Add(news.GetRange(4, 2));
                    ret.news.Add(news.GetRange(6, 2));

                    
                    news.Sort(8, 24, new NewsCompare());

                    //Console.WriteLine(news[8].newsBody.matchTag);
                    //Console.WriteLine(news[12].newsBody.matchTag);
                    //Console.WriteLine(news[16].newsBody.matchTag);
                    //Console.WriteLine(news[20].newsBody.matchTag);
                    //Console.WriteLine(news[24].newsBody.matchTag);
                    //Console.WriteLine(news[28].newsBody.matchTag);

                    ret.news.Add(news.GetRange(8, 4));
                    ret.news.Add(news.GetRange(12, 4));
                    ret.news.Add(news.GetRange(16, 4));
                    ret.news.Add(news.GetRange(20, 4));
                    ret.news.Add(news.GetRange(24, 4));
                    ret.news.Add(news.GetRange(28, 4));

                    ret.news.Add(news.GetRange(32, 4));
                    ret.news.Add(news.GetRange(36, 3));
                    ret.news.Add(news.GetRange(39, 5));

                    ret.videos.Add(v.GetRange(0, 4));
                    ret.videos.Add(v.GetRange(4, 2));
                    ret.videos.Add(v.GetRange(6, 5));
                    return Ok(new CustomResponse { ok = "yes", value = ret });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return Ok(new CustomResponse { ok = "no", value = "服务器内部错误" });
                }
            }
            else
            {
                Console.WriteLine("数据库连接失败");
                return Ok(new CustomResponse { ok = "no", value = "数据库连接失败" });
            }
        }
        [HttpGet] 
        public async Task<IActionResult> GetNewsNum()
        {
            Console.WriteLine("GET Login!");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            int num;
            if (ORACLEConnectTry.getConn() == true)
            {
                try
                {
                    SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                    //List<News> news = await sqlORM.Queryable<News>().OrderBy(it => it.publishDateTime).ToListAsync();
                    num = await sqlORM.Queryable<News>().CountAsync();
                    return Ok(new CustomResponse { ok = "yes", value = num });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return Ok(new CustomResponse { ok = "no", value = -1 }); // Internal server error
                }
            }
            else { return Ok(new CustomResponse { ok = "no", value = -1 }); };
        }
        [HttpPost]
        public async Task<IActionResult> GetNews([FromBody] GetNewsRequest json)
        {
            Console.WriteLine("--------------------------GetNews--------------------------");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            int num = json.num;
            string mtag = json.matchTag;
            string ptag = json.propertyTag;
            if(ORACLEConnectTry.getConn() == true)
            {
                try
                {
                    SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                    int sum = sqlORM.Queryable<Video>().Count();
                    if (num == -1 || num > sum)
                    {
                        num = sum;
                    }
                    //List<News> news = await sqlORM.Queryable<News>().OrderBy(it => it.publishDateTime).Take(num).ToListAsync();
                    //List<NewsWithPicture> newsWithPictures = new List<NewsWithPicture>();
                    //for(int i = 0; i < news.Count; i++)
                    //{
                    //    
                    //}
                    //var news = await sqlORM.Queryable<News>().LeftJoin<NewsHavePicture>((n, nhp)=> n.news_id == nhp.news_id).Select((n, nhp)=>new NewsWithPicture 
                    //{news_id = n.news_id, publishDateTime = n.publishDateTime, summary = n.summary, contains = n.contains, matchTag = n.matchTag, propertyTag = n.propertyTag})
                    //    .ToListAsync();
                    List<News> news = new List<News>();
                    List<NewsWithPicture> ret = new List<NewsWithPicture>();
                    if (mtag == "" && ptag == "")
                    {
                        news = await sqlORM.Queryable<News>().
                            OrderBy(n => n.publishDateTime, OrderByType.Desc).
                            Take(num).ToListAsync();
                        ret = new List<NewsWithPicture>();
                        Console.WriteLine(news.Count);

                        for (int i = 0; i < news.Count; i++)
                        {
                            int id = news[i].news_id;
                            List<string> pictureRoutes = await sqlORM.Queryable<NewsHavePicture>().
                                Where(n => n.news_id == id).
                                Select(nhp => nhp.pictureRoute).
                                ToListAsync();
                            ret.Add(new NewsWithPicture { newsBody = news[i], pictureRoutes = pictureRoutes });
                        }
                    }
                    else if(mtag != "" && ptag == "")
                    {
                        news = await sqlORM.Queryable<News>().Where(n => n.matchTag == mtag).
                            OrderBy(n => n.publishDateTime, OrderByType.Desc).Take(num).ToListAsync();
                        ret = new List<NewsWithPicture>();
                        for (int i = 0; i < news.Count; i++)
                        {
                            int id = news[i].news_id;
                            List<string> pictureRoutes = await sqlORM.Queryable<NewsHavePicture>().
                                Where(n => n.news_id == id).
                                Select(nhp => nhp.pictureRoute).
                                ToListAsync();
                            ret.Add(new NewsWithPicture { newsBody = news[i], pictureRoutes = pictureRoutes });
                        }
                    }
                    else if(mtag == "" && ptag != "")
                    {
                        news = await sqlORM.Queryable<News>().Where(n => n.propertyTag == ptag).
                            OrderBy(n => n.publishDateTime, OrderByType.Desc).Take(num).ToListAsync();
                        ret = new List<NewsWithPicture>();
                        for (int i = 0; i < news.Count; i++)
                        {
                            int id = news[i].news_id;
                            List<string> pictureRoutes = await sqlORM.Queryable<NewsHavePicture>().
                                Where(n => n.news_id == id).
                                Select(nhp => nhp.pictureRoute).
                                ToListAsync();
                            ret.Add(new NewsWithPicture { newsBody = news[i], pictureRoutes = pictureRoutes });
                        }
                    }
                    else
                    {
                        news = await sqlORM.Queryable<News>().Where(n => n.matchTag == mtag && n.propertyTag == ptag).//LeftJoin<NewsHavePicture>((n, nhp) => n.news_id == nhp.news_id).Where(n => n.matchTag == mtag && n.propertyTag == ptag).
                            OrderBy(n => n.publishDateTime, OrderByType.Desc).Take(num).ToListAsync();
                        ret = new List<NewsWithPicture>();
                        for (int i = 0; i < news.Count; i++)
                        {
                            int id = news[i].news_id;
                            List<string> pictureRoutes = await sqlORM.Queryable<NewsHavePicture>().
                                Where(n => n.news_id == id).
                                Select(nhp => nhp.pictureRoute).
                                ToListAsync();
                            ret.Add(new NewsWithPicture { newsBody = news[i], pictureRoutes = pictureRoutes });
                        }
                    }
                    return Ok(new CustomResponse { ok = "yes", value = ret });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return Ok(new CustomResponse { ok = "no", value = "服务器内部错误" }); // Internal server error
                }
            }
            else { return Ok(new CustomResponse { ok = "no", value = "数据库连接失败！" }); };
        }
        [HttpPost]
        public async Task<IActionResult> SearchNews([FromBody] SearchNewsRequest json)
        {
            Console.WriteLine("--------------------------SearchNews--------------------------");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            string keyword = json.keyword;
            if (ORACLEConnectTry.getConn() == true)
            {
                try
                {
                    SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                    List<NewsWithPicture> ret = new List<NewsWithPicture>();
                    //Expression<Func<News, bool>> exp = Expressionable.Create<News>() //创建表达式
                    //    .AndIF(p > 0, it => it.Id == p)
                    //    .AndIF(name != null, it => it.Name == name && it.Sex == 1)
                    //    .ToExpression();//注意 这一句 不能少
                    List<News> news = await sqlORM.Queryable<News>().Where(it => (it.title.Contains(keyword) || it.summary.Contains(keyword) || it.contains.Contains(keyword))).ToListAsync();
                    Func<News, int> evaluate = x =>
                    {
                        int num = 0;
                        if (x.title.Contains(keyword))
                        {
                            num += 4;
                        }
                        if (x.summary.Contains(keyword))
                        {
                            num += 2;
                        }
                        if (x.contains.Contains(keyword))
                        {
                            num += 1;
                        }
                        return num;
                    };
                    
                    news.Sort((a, b) => 
                    { 
                        int na = evaluate(a), nb = evaluate(b);
                        return (na == nb ? 0 : (na > nb ? -1 : 1));
                    });
                    ret = new List<NewsWithPicture>();
                    for (int i = 0; i < news.Count; i++)
                    {
                        int id = news[i].news_id;
                        List<string> pictureRoutes = await sqlORM.Queryable<NewsHavePicture>().
                            Where(n => n.news_id == id).
                            Select(nhp => nhp.pictureRoute).
                            ToListAsync();
                        ret.Add(new NewsWithPicture { newsBody = news[i], pictureRoutes = pictureRoutes });
                    }
                    return Ok(new CustomResponse { ok = "yes", value = ret});
                }
                catch
                {
                    //return new InternalError(new CustomResponse { ok = "no", value = "Internal Error" });
                    return Ok(new CustomResponse { ok = "no", value = "Internal Error" });
                }
            }
            else
            {
                //return new ServiceUnavailable(new CustomResponse { ok = "no", value = "ConnectionFailed" });
                return Ok(new CustomResponse { ok = "no", value = "ConnectionFailed" });
            }
        }
    }
}
