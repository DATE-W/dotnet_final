using AdminLib.Models;
using NewsLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AdminLib.Models.NoticeModel;

namespace AdminLib.Services
{
    public interface IAdminService
    {
        public Task<AdminPost[]> GetAllPost();
        public Task<string> BanPostbyID(postjson json);
        public Task<BannedUsrJson[]> GetBannedUser();
        public Task<string> LiftUser(banneduserjson json);
        public Task<News[]> GetAllNews();
        public Task<string> DeleteNews(deleteNewsJson json);
        public Task<UserJson[]> SearchUser(SearchUserJson json);
        public Task<UserJson[]> SearchBannedUser(SearchUserJson json);
        public Task<Notice[]> GetAllNotice();
    }
}
