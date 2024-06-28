using Microsoft.AspNetCore.Mvc;

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
