using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsLib.Models
{
    public class NewsCompare : IComparer<NewsWithPicture>
    {
        public int Compare(NewsWithPicture? x, NewsWithPicture? y)
        {
            Func<NewsWithPicture, int> eval = tmp =>
            {
                string t = tmp.newsBody.matchTag;
                if (t == "中超")
                {
                    return 1;
                }
                if (t == "英超")
                {
                    return 2;
                }
                if (t == "西甲")
                {
                    return 3;
                }
                if (t == "德甲")
                {
                    return 4;
                }
                if (t == "意甲")
                {
                    return 5;
                }
                if (t == "法甲")
                {
                    return 6;
                }
                return -1;
            };
            int nx = eval(x), ny = eval(y);
            return nx == ny ? 0 : (nx < ny ? -1 : 1);
        }
    }
    public class NewsWithPicture
    {
        public News newsBody { get; set; }
        public List<string>? pictureRoutes { get; set; }
    }
    public class InitResponse
    {
        public List<List<NewsWithPicture>> news { get; set; }
        public List<List<Video>> videos { get; set; }
    }
    public class GetNewsRequest
    {
        public int num { get; set; }
        public string matchTag { get; set; }
        public string propertyTag { get; set; }
    }
    public class SearchNewsRequest
    {
        public string keyword { get; set; }
    }
}
