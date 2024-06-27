using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using DBwebAPI.Models;
using DBwebAPI.Controllers;
using Newtonsoft.Json.Linq;
using System.Security.Principal;
using Newtonsoft.Json;
using static DBwebAPI.Models.AdminController;

namespace DBwebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        public class LoginRequest
        {
            public string Account { get; set; }
            public string Password { get; set; }
        }
        public class CustomResponse
        {
            public string ok { get; set; }
            public object value { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> LoginPassword([FromBody] LoginRequest json)
        {
            Console.WriteLine("--------------------------Get LoginPassword--------------------------");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            //提取参数
            string account = json.Account;
            string passwordHash = json.Password;
            Console.WriteLine("account=" + account);
            Console.WriteLine("passwordHash= " + passwordHash);

            if (ORACLEConnectTry.getConn() == true)
            {
                try
                {
                    SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                    //进行用户查询
                    List<Usr> tempUsr = new List<Usr>();
                    tempUsr = await sqlORM.Queryable<Usr>().Where(it => it.userAccount == account
                    && it.userPassword == passwordHash)
                        .ToListAsync();
                    //判断用户是否存在
                    if (tempUsr.Count() == 0)
                    {
                        Console.WriteLine("登录失败");
                        return Ok(new CustomResponse { ok = "no", value = "Fail" });//用户账户或密码错误
                    }
                    else
                    {

                        if (tempUsr[0].isBanned==1)
                        {
                            Console.WriteLine("被封禁的用户");
                            return Ok(new CustomResponse { ok = "no", value = "banned" });//被封禁
                        }
                        Console.WriteLine("登录成功");
                        // 生成JWT令牌
                        string token = new CreateToken().createToken(account, "Normal");//生成token，标定用户为Normal
                        Console.WriteLine(token);
                        // 返回登录成功及JWT令牌
                        return Ok(new CustomResponse { ok = "yes", value = token });
                    }
                }
                catch (Exception ex)
                {
                    return Ok(new CustomResponse { ok = "no", value = ex }); // Internal server error
                }
            }
            else {return Ok(new CustomResponse { ok = "no", value = "数据库连接失败" }); };
        }




        [HttpPost]
        public async Task<IActionResult> adminLogin([FromBody] LoginRequest json)
        {
            Console.WriteLine("GET Login!");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            //提取参数
            string account = json.Account;
            string passwordHash = json.Password;
            Console.WriteLine("adminName=" + account);
            Console.WriteLine("passwordHash= " + passwordHash);

            if (ORACLEConnectTry.getConn() == true)
            {
                try
                {
                    SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                    //进行用户查询
                    List<Admins> tempAdmin = new List<Admins>();
                    tempAdmin = await sqlORM.Queryable<Admins>().Where(it => it.adminName == account
                    && it.adminPassword == passwordHash)
                        .ToListAsync();
                    //判断用户是否存在
                    if (tempAdmin.Count() == 0)
                    {
                        Console.WriteLine("登录失败");
                        return Ok(new CustomResponse { ok = "no", value = "Fail" });//用户账户或密码错误
                    }
                    else
                    {

                        Console.WriteLine("登录成功");
                        // 生成JWT令牌
                        string token = new CreateToken().createToken(account, "Admin");//生成token，标定用户为Admin
                        Console.WriteLine(token);
                        // 返回登录成功及JWT令牌
                        return Ok(new CustomResponse { ok = "yes", value = token });
                    }
                }
                catch (Exception ex)
                {
                    return Ok(new CustomResponse { ok = "no", value = ex }); // Internal server error
                }
            }
            else { return Ok(new CustomResponse { ok = "no", value = "数据库连接失败" }); };
        }




    }
}
