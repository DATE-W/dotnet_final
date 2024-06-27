using NewsLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsLib.Models;

namespace NewsLib.Services
{
    public interface IVideoService
    {
        public Task<CustomResponse> GetVideoRandomly(GetVideoRequest json);

        public Task<CustomResponse> SearchVideo(SearchVideoRequest json);
    }
}
