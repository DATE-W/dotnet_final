using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsLib.Models
{
    public class GetVideoRequest
    {
        public int num { get; set; }
        public string matchTag { get; set; }
        public string propertyTag { get; set; }
    }
}
