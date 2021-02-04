using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace playground.Interfaces
{
    /// <summary>
    /// Sets cookies in http context
    /// </summary>
    public interface ICookiesService
    {
        /// <summary>
        /// Sets cookies based on passed dictionary where Key is a Cookie name and Value is a Cookie value
        /// </summary>
        /// <param name="data"></param>
        public void SetCookies(IDictionary<string, string> data, CookieOptions cookieOptions, HttpResponse response);

        /// <summary>
        /// Reads user name from cookie
        /// </summary>
        /// <param name="cookieKeyName"></param>
        /// <param name="request"></param>
        /// <returns>Returns string username</returns>
        public string GetUserName(string cookieKeyName, HttpRequest request);
    }
}
