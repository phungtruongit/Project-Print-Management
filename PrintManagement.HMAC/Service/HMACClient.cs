using System.Net.Http;

namespace VCSLib.HMAC {
    public class HMACClient : HttpClient {
        public HMACClient() {
        }

        public HMACClient(HttpMessageHandler handler) : base(handler) {
        }

        public HMACClient(HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler) {
        }
    }
}
