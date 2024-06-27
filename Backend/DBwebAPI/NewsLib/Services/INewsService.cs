using NewsLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsLib.Services
{
    public interface INewsService
    {
        public Task<CustomResponse> Init();
        public Task<CustomResponse> GetNewsNum();
        public Task<CustomResponse> GetNews(GetNewsRequest json);
        public Task<CustomResponse> SearchNews(SearchNewsRequest json);
    }
}
