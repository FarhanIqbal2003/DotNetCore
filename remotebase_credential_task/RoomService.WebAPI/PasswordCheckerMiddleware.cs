using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RoomService.WebAPI
{
    public class PasswordCheckerMiddleware
    {
        private readonly RequestDelegate _next;
        public PasswordCheckerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            System.Net.Http.Headers.HttpRequestHeaders headers = context.Request.Headers;
            if (headers.Contains("passwordKey"))
            {
                if (headers.GetValues("passwordKey").Equals("passwordKey123456789"))
                {
                    await _next.Invoke(context);
                }
                else
                {
                    context.Response.StatusCode = 401;
                    return;
                }
            }
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                //Extract credentials
                string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(System.Convert.FromBase64String(encodedUsernamePassword));

                int seperatorIndex = usernamePassword.IndexOf(':');

                var username = usernamePassword.Substring(0, seperatorIndex);
                var password = usernamePassword.Substring(seperatorIndex + 1);

                if(username == "passwordKey" && password == "passwordKey123456789" )
                {
                    await _next.Invoke(context);
                }
            }
            else
            {
                context.Response.StatusCode = 401;
                return;
            }
        }
    }
}
