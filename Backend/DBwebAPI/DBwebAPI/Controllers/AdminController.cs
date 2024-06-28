using DBwebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using DBwebAPI.Utils;
using NewsLib.Models;
using NewsLib.Relations;
using TeamLib.Models;
using AdminLib.Models;
using AdminLib.Services;

namespace DBwebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPost()
        {
            try
            {
                return Ok(await _adminService.GetAllPost());
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BanPostbyID(postjson json)
        {
            try
            {
                var res = await _adminService.BanPostbyID(json);
                return Ok(new { ok = res });
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> GetBannedUser()
        {
            try
            {
                return Ok(await _adminService.GetBannedUser());
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> LiftUser(banneduserjson json)
        {
            try
            {
                var res = _adminService.LiftUser(json);
                return Ok(new { ok = res });
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllNews()
        {
            try
            {
                return Ok(await _adminService.GetAllNews());
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        
        [HttpDelete]
        public async Task<IActionResult> DeleteNews(deleteNewsJson json)
        {
            try
            {
                var res = await _adminService.DeleteNews(json);
                return Ok(new { ok = res });
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
       
        [HttpPost]
        public async Task<IActionResult> SearchUser(SearchUserJson json)
        {
            try
            {
                return Ok(await _adminService.SearchUser(json));
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SearchBannedUser(SearchUserJson json)
        {
            try
            {
                return Ok(await _adminService.SearchBannedUser(json));
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllNotice()
        {
            try
            {
                return Ok(await _adminService.GetAllNotice());
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库错误：" + ex.Message);
                return BadRequest(new { error = "数据库错误" });
            }
        }
    }

}