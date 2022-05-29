using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using PrintManagement.ApiIntegration.Common;

namespace PrintManagement.ApiIntegration {
    public class UserApiClient : IUserApiClient {
        private readonly AppSettings _appSettings;

        public UserApiClient(AppSettings appSettings) {
            _appSettings = appSettings;
        }

        public async Task<ApiResultDTO<string>> AuthenticateAsync(LoginRequest request) {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            client.BaseAddress = new Uri(_appSettings.BaseAddress);
            var response = await client.PostAsync("/api/user/login", httpContent);
            if (response.IsSuccessStatusCode) {
                return JsonConvert.DeserializeObject<ApiResultDTO<string>>(await response.Content.ReadAsStringAsync());
            }

            return JsonConvert.DeserializeObject<ApiResultDTO<string>>(await response.Content.ReadAsStringAsync());
        }
               
        public async Task<bool> CheckPermissionAsync(Guid id, int totalPage, string printerName, string token) {
            var client = new HttpClient();

            client.BaseAddress = new Uri(_appSettings.BaseAddress);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"/api/User/CheckPrintPermission?id={id}&totalPage={totalPage}&printerName={printerName}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<bool>(body);
            return false;
        }
    }
}