using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using DBwebAPI.Models;
using DBwebAPI.Controllers;

namespace DBwebAPI.Controllers
{
    [ApiController]
    [Route("api/[action]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public async Task<string> testTeam(string testStr)
        {
            Console.WriteLine("received!");
        
            return testStr;
                

        }
    }
}
