using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VCSLib.HMAC {
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class HMACAuthorizeAttribute : ActionFilterAttribute {
        private const string HMACAuthorize = "HMACAuthorize";
        private readonly IHashCodeHMAC _hashCodeHMAC;
        public HMACAuthorizeAttribute() {
            _hashCodeHMAC = new HashCodeHMAC();
        }


        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
            var bodyContent = await GetRawBodyAsync(context.HttpContext.Request);
            var requestContentStringBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(bodyContent));

            if (!context.HttpContext.Request.Headers.TryGetValue(HMACAuthorize, out var extractedHMAC)) {
                context.Result = new ContentResult() {
                    StatusCode = 401,
                    Content = "HMAC Authorize was not provided"
                };
                return;
            }

            // Get content from request.
            var authorizationHeaderArray = extractedHMAC.ToString().Split(':');

            var hashCodeHMACSHA1Client = authorizationHeaderArray[0];
            var requestNonce = authorizationHeaderArray[1];
            var requestTimeStamp = authorizationHeaderArray[2];

            var hashCodeHMACSHA1Server = _hashCodeHMAC.ComputeHashCodeHMACSHA256(requestContentStringBase64, requestNonce, requestTimeStamp);

            if (!hashCodeHMACSHA1Server.Equals(hashCodeHMACSHA1Client)) {
                context.Result = new ContentResult() {
                    StatusCode = 401,
                    Content = "HashCode HMAC is not valid"
                };
                return;
            }

            await next();
        }

        public static async Task<string> GetRawBodyAsync(HttpRequest request, Encoding encoding = null) {
            if (!request.Body.CanSeek) {
                // We only do this if the stream isn't *already* seekable,
                // as EnableBuffering will create a new stream instance
                // each time it's called
                request.EnableBuffering();
            }

            request.Body.Position = 0;

            var reader = new StreamReader(request.Body, encoding ?? Encoding.UTF8);

            var body = await reader.ReadToEndAsync().ConfigureAwait(false);

            request.Body.Position = 0;

            return body;
        }
    }
}
