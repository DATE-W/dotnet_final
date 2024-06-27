using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DBwebAPI
{
    public class UnavailableRequest : StatusCodeResult
    {
        object response { get; set; }
        public UnavailableRequest([ActionResultStatusCode] int statusCode, object Response) : base(statusCode)
        {
            statusCode = 503;
            response = Response;
        }
    }
}
