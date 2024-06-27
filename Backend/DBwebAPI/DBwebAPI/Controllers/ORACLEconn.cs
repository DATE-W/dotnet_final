using DBwebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace DBwebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ORACLEconn : Controller
    {
        public SqlSugarScope sqlORM = null;
        [HttpGet]
        public bool getConn()
        {
            Console.WriteLine("get access!");

            //创建数据库连接
            DBconn dbconn = new DBconn();
            //打开ORM
            dbconn.Conn();
            
            sqlORM = dbconn.sqlORM;

            if (sqlORM != null)
            {
                Console.WriteLine("open success!"); return true; 
            }
            else return false;
        }
    }
}
