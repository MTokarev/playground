using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace playground.Extensions
{
    public static class CustomConfiguration
    {
        public static IApplicationBuilder ConfigureCustomService(this IApplicationBuilder app)
        {
            // Redirect all unauthorized requests to the login page
            app.UseStatusCodePages(async context =>
            {
                int responseCode = context.HttpContext.Response.StatusCode;
                if (responseCode == (int)HttpStatusCode.Unauthorized ||
                    responseCode == (int)HttpStatusCode.Forbidden)
                {
                    context.HttpContext.Response.Redirect($"/error/showError?errorCode={responseCode}");
                }
            });

            // Enable authentication and auth
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
