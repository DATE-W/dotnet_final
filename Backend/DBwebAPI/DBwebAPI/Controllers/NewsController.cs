using Microsoft.AspNetCore.Mvc;
using NewsLib.Models;
using NewsLib.Services;

namespace DBwebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }
        
        [HttpGet]
        public async Task<IActionResult> Init()
        {
            return Ok(await _newsService.Init());
        }
        [HttpGet] 
        public async Task<IActionResult> GetNewsNum()
        {
            return Ok(await _newsService.GetNewsNum());
        }
        [HttpPost]
        public async Task<IActionResult> GetNews([FromBody] GetNewsRequest json)
        {
            return Ok(await _newsService.GetNews(json));
        }
        [HttpPost]
        public async Task<IActionResult> SearchNews([FromBody] SearchNewsRequest json)
        {
            return Ok(await _newsService.SearchNews(json));
        }
    }
}
