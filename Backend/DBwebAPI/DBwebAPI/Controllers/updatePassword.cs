using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using DBwebAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace DBwebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ForgetPasswordController : ControllerBase
    {
        public class UserAccountRequest
        {
            public string UserAccount { get; set; }
        }
        public class PasswordUpdateRequest
        {
            public string UserAccount { get; set; }
            public string UserSecAns { get; set; }
            public string NewPassword { get; set; }
        }
        public class CustomResponse
        {
            public string ok { get; set; }
            public string value { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> GetUserSecQue([FromBody] UserAccountRequest json)
        {
            string account = json.UserAccount;
            Console.WriteLine("account= " + account);
            // Check if the ORACLE connection is successful
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            ORACLEConnectTry.getConn();
            try
            {
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                // Query the database to find the userSecQue based on the userAccount
                List<string> userSecQues = await sqlORM.Queryable<Usr>()
                    .Where(u => u.userAccount == account)
                    .Select(u => u.userSecQue)
                    .ToListAsync();

                if (userSecQues.Count != 0)
                {
                    Console.WriteLine(userSecQues.FirstOrDefault());
                    return Ok(new CustomResponse { ok = "yes", value = userSecQues.FirstOrDefault() });
                }
                else
                {
                    Console.WriteLine("WrongAccount");
                    return Ok(new CustomResponse { ok = "no", value = "WrongAccount" }); // User account not found 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UNKNOWN");
                Console.WriteLine(ex);
                return Ok(new CustomResponse { ok = "no", value = "UNKNOWN" }); // Internal server error
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordUpdateRequest json)
        {
            string account = json.UserAccount;
            string userSecAns = json.UserSecAns;
            string newPassword = json.NewPassword;
            Console.WriteLine("UpdatePassword");
            // Check if the ORACLE connection is successful
            ORACLEconn ORACLEConnectTry = new ORACLEconn();
            ORACLEConnectTry.getConn();
            try
            {
                SqlSugarScope sqlORM = ORACLEConnectTry.sqlORM;
                // Query the database to find the user account and security answer
                var user = await sqlORM.Queryable<Usr>()
                    .Where(u => u.userAccount == account)
                    .Select(u => new { u.userSecAns, u.userPassword })
                    .FirstAsync();

                if (user == null)
                {
                    Console.WriteLine("WrongAccount");
                    return Ok(new CustomResponse { ok = "no", value = "WrongAccount" }); // User account not found
                }

                if (user.userSecAns != userSecAns)
                {
                    Console.WriteLine("WrongAns");
                    return Ok(new CustomResponse { ok = "no", value = "WrongAns" }); // Security answer does not match
                }

                // Update the user password
                var updateResult = await sqlORM.Updateable<Usr>()
                    .SetColumns(u => new Usr { userPassword = newPassword })
                    .Where(u => u.userAccount == account)
                    .ExecuteCommandAsync();

                if (updateResult > 0)
                {
                    Console.WriteLine("Success");
                    return Ok(new CustomResponse { ok = "yes", value = "Success" }); // Password updated successfully
                }
                else
                {
                    Console.WriteLine("updateFailed");
                    return Ok(new CustomResponse { ok = "no", value = "updateFailed" }); // Failed to update password
                }
            }
            catch (Exception)
            {
                Console.WriteLine("UNKNOWN");
                return Ok(new CustomResponse { ok = "no", value = "UNKNOWN" }); // Internal server error
            }
        }
    }
}
