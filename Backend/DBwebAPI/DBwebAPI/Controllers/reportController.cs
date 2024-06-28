using SqlSugar;
using Microsoft.AspNetCore.Mvc;
using DBwebAPI.Utils;
using AdminLib.Models;
using static AdminLib.Models.NoticeModel;
using ReportLib.Models;
using ReportLib.Services;

namespace DBwebAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class reportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public reportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> getUsrInfo()
        {
            try
            {
                return Ok(await _reportService.getUsrInfo());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "查看失败" });
            }
        }


        [HttpGet]
        public async Task<IActionResult> getReportInfo()
        {
            try
            {
                return Ok(await _reportService.getReportInfo());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "查看失败" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> cancelReport([FromBody]dealReportVal json)
        {
            try
            {
                return Ok(await _reportService.cancelReport(json));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "操作失败" });
            }

        }

        [HttpPost]
        public async Task<IActionResult> agreeReport([FromBody] dealReportVal json)
        {
            try
            {
                return Ok(await _reportService.agreeReport(json));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "操作失败" });
            }
        }
        

        [HttpPost]
        public async Task<IActionResult> banUsr([FromBody] banUsrPara json)
        {
            try
            {
                return Ok(await _reportService.banUsr(json));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new CustomResponse { ok = "no", value = "操作失败" });
            }

        }

    }
}
