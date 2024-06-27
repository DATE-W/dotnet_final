using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Linq.Expressions;
using DBwebAPI;
using DBwebAPI.Models;
using DBwebAPI.Utils;
using NewsLib.Models;
using NewsLib.Services;

namespace DBwebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VideoController : Controller
    {
        private readonly IVideoService _videoService;
        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }
        [HttpPost]
        public async Task<IActionResult> GetVideoRandomly(GetVideoRequest json)
        {
            return Ok(_videoService.GetVideoRandomly(json));
        }
        [HttpPost]
        public async Task<IActionResult> SearchVideo(SearchVideoRequest json)
        {
            return Ok(_videoService.SearchVideo(json));
        }
    }
}
