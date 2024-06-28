using ReportLib.Models;

namespace ReportLib.Services
{
    public interface IReportService
    {
        public Task<CustomResponse> getUsrInfo();
        public Task<CustomResponse> getReportInfo();
        public Task<CustomResponse> cancelReport(dealReportVal json);
        public Task<CustomResponse> agreeReport(dealReportVal json);
        public Task<CustomResponse> banUsr(banUsrPara json);
    }
}
