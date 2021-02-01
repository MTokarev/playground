using Microsoft.AspNetCore.Http;
using playground.Interfaces;
using System.Collections.Generic;

namespace playground.Services
{
    class CookiesService : ICookiesService
    {
        public void SetCookies(IDictionary<string, string> data, CookieOptions cookieOptions, HttpResponse response)
        {

            foreach (var item in data.Keys)
            {
                response.Cookies.Append(item, data[item], cookieOptions);
            }
        }

        string ICookiesService.GetUserName(string cookieKeyName, HttpRequest request)
        {
            return request.Cookies["X-Username"];
        }
    }
}
