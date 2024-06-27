using Microsoft.AspNetCore.Mvc;

namespace DBwebAPI
{
    public class CustomResponse
    {
        public string ok { get; set; }
        public object value { get; set; }
    }
}
