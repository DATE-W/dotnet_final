using DBwebAPI.Utils;
using Microsoft.AspNetCore.Mvc;

namespace DBwebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserTokenController : ControllerBase
    {
        [HttpPost]
        public IActionResult UserToken()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var valid = new ValidateToken();
            try
            {
                bool pass = valid.ValidateJwtToken(token, ValidTokenAuthority.Normal);
                if (pass)
                {
                    return Ok(new { ok = "yes" });
                }
                else
                {
                    return Ok(new { ok = "no" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Test(string token)
        {
            //var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var valid = new ValidateToken();
            try
            {
                bool pass = valid.ValidateJwtToken(token, ValidTokenAuthority.Normal);
                if (pass)
                {
                    return Ok(new { ok = "yes" });
                }
                else
                {
                    return Ok(new { ok = "no" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AdminTest(string token)
        {
            //var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var valid = new ValidateToken();
            try
            {
                bool pass = valid.ValidateJwtToken(token, ValidTokenAuthority.Admin);
                if (pass)
                {
                    return Ok(new { ok = "yes" });
                }
                else
                {
                    return Ok(new { ok = "no" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
