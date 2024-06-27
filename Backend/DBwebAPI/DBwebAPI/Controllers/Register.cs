using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using DBwebAPI.Models;
using DBwebAPI.Controllers;

namespace DBwebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class Register : ControllerBase  
    {
        public class RegisterRequest
        {
            public string Account { get; set; }
            public string Password { get; set; }
            public string UserName { get; set; }
            public string UserSecQue { get; set; }
            public string UserSecAns { get; set; }
        }
        public class CustomResponse
        {
            public string ok { get; set; }
            public object value { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> normalRegisterAsync([FromBody] RegisterRequest registerRequest)
        {
            int count = -1;
            Console.WriteLine("--------------------------Get normalRegisterAsync--------------------------");
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            if (ORACLEConnectTry.getConn() == true)
            {
                try
                {
                    SqlSugarScope sqlOrm = ORACLEConnectTry.sqlORM;
                    //进行用户查询

                    int user_id = sqlOrm.Queryable<Usr>().Max(it => it.user_id) + 1;
                    Usr usr = new Usr();
                    usr.user_id = user_id;
                    usr.userName = registerRequest.UserName;
                    usr.userPassword = registerRequest.Password;
                    usr.userAccount = registerRequest.Account;
                    usr.userSecQue = registerRequest.UserSecQue;
                    usr.userSecAns = registerRequest.UserSecAns;
                    usr.userPoint = 0;
                    usr.follownumber = 0;
                    usr.themeType = 1;
                    usr.followednumber = 0;
                    usr.avatar = "http://110.40.206.206/pictures/3ea6beec64369c2642b92c6726f1epng.png";
                    usr.signature = "这个人很懒，什么都没有留下";
                    usr.createDateTime = DateTime.Now;
                    Console.WriteLine("user_id= " + user_id);
                    Console.WriteLine("userName= " + usr.userName);
                    Console.WriteLine("userPassword= " + usr.userPassword);
                    Console.WriteLine("userAccount= " + usr.userAccount);
                    Console.WriteLine("userSecQue= " + usr.userSecQue);
                    Console.WriteLine("userSecAns= " + usr.userSecAns);
                    Console.WriteLine("createDateTime= " + usr.createDateTime);
                    bool accountExists = sqlOrm.Queryable<Usr>().Any(u => u.userAccount == registerRequest.Account);
                    if (accountExists)
                    {
                        Console.WriteLine("账号已存在");
                        return Ok(new CustomResponse { ok = "no", value = "账户已存在" });
                    }
                    else
                    {
                        count = await sqlOrm.Insertable(usr).ExecuteCommandAsync();
                        if (count == 1)
                        {
                            Console.WriteLine("注册成功");
                            return Ok(new CustomResponse { ok = "yes", value = "Success" });
                        }
                        else
                        {
                            Console.WriteLine("注册失败");
                            return Ok(new CustomResponse { ok = "no", value = "Fail" });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("未知错误");
                    System.Console.WriteLine(ex.Message);
                }
            }
            return Ok(new CustomResponse { ok = "no", value = "UNKNOWN" });//未知错误
        }
    }
}
