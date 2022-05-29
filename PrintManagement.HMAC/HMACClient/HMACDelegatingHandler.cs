using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VCSLib.HMAC {
    public class HMACHttpClientHandler : HttpClientHandler {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                                     CancellationToken cancellationToken) {
            Console.WriteLine("Bất đầu kết nối " + request.RequestUri.ToString());
            // Thực hiện truy vấn đến Server
            var response = await base.SendAsync(request, cancellationToken);
            Console.WriteLine("Hoàn thành tải dữ liệu");
            return response;
        }
    }

    public class HMACDelegatingHandler : DelegatingHandler {
        public readonly IHashCodeHMAC _hashCodeHMAC;
        public HMACDelegatingHandler(HttpMessageHandler innerHandler, IHashCodeHMAC hashCodeHMAC) : base(innerHandler) {
            _hashCodeHMAC = hashCodeHMAC;
        }
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            string requestContentBase64String = string.Empty;
            //Calculate UNIX time
            DateTime epochStart = new (1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();
            //Create the random nonce for each request
            string nonce = Guid.NewGuid().ToString("N");
            //Checking if the request contains body, usually will be null wiht HTTP GET and DELETE
            if (request.Content != null) {
                // Hashing the request body, so any change in request body will result a different hash
                // we will achieve message integrity
                var content = await request.Content.ReadAsByteArrayAsync();
                requestContentBase64String = Convert.ToBase64String(content);
                //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(GetDataFromStream(content));
                //requestContentBase64String = Convert.ToBase64String(plainTextBytes);
            }

            var hashCodeHMACSHA1 = _hashCodeHMAC.ComputeHashCodeHMACSHA256(requestContentBase64String, nonce, requestTimeStamp);
            //Setting the values in the Authorization header using custom scheme (hmacauth)
            //request.Headers.Authorization = new AuthenticationHeaderValue("HMACAuthorize", string.Format("{0}:{1}:{2}", hashCodeHMACSHA1, nonce, requestTimeStamp));
            request.Headers.Add("HMACAuthorize", string.Format("{0}:{1}:{2}", hashCodeHMACSHA1, nonce, requestTimeStamp));

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }

        private string GetDataFromStream(Stream stream) {
            try {
                using StreamReader reader = new (stream);
                return reader.ReadToEnd();
            }
            catch (Exception) {
                throw;
            }
        }

    }
}
