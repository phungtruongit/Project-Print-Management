using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using PrintManagement.ApiIntegration.Common;
using Microsoft.Extensions.Options;

namespace PrintManagement.ApiIntegration {
    public class SystemInfoApiClient : ISystemInfoApiClient {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppSettings _appSettings;

        public SystemInfoApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                   IOptionsMonitor<AppSettings> optionsMonitor) {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _appSettings = optionsMonitor.CurrentValue;
        }

        public async Task<ApiResultDTO<string>> Authenticate(LoginRequest request) {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_appSettings.BaseAddress);
            var response = await client.PostAsync("/api/user/login", httpContent);
            if (response.IsSuccessStatusCode) {
                return JsonConvert.DeserializeObject<ApiResultDTO<string>>(await response.Content.ReadAsStringAsync());
            }

            return JsonConvert.DeserializeObject<ApiResultDTO<string>>(await response.Content.ReadAsStringAsync());
        }

        public async Task<ApiResultDTO<bool>> CreateSystemInfo(SystemInfoDTO systemInfo) {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            client.BaseAddress = new Uri(_appSettings.BaseAddress);
            var json = JsonConvert.SerializeObject(systemInfo);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/systeminfo", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(result);

            return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(result);
        }

        public async Task<ApiResultDTO<bool>> DeleteSystemInfo(Guid id) {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_appSettings.BaseAddress);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.DeleteAsync($"/api/systeminfo/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(body);

            return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(body);
        }

        public async Task<ApiResultDTO<List<SystemInfoDTO>>> GetAll() {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.BaseAddress = new Uri(_appSettings.BaseAddress);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/systeminfo");
            var body = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ApiResultDTO<List<SystemInfoDTO>>>(body);
            return data;
        }

        public async Task<ApiResultDTO<SystemInfoDTO>> GetSystemInfoById(Guid id) {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_appSettings.BaseAddress);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/systeminfo/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiResultDTO<SystemInfoDTO>>(body);

            return JsonConvert.DeserializeObject<ApiResultDTO<SystemInfoDTO>>(body);
        }

        public async Task<ApiResultDTO<SystemInfoDTO>> GetSystemInfoMain() {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.BaseAddress = new Uri(_appSettings.BaseAddress);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/systeminfo/main");
            var body = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ApiResultDTO<SystemInfoDTO>>(body);
            return data;
        }

        public async Task<ApiResultDTO<bool>> UpdateSystemInfo(Guid id, SystemInfoDTO systemInfo) {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_appSettings.BaseAddress);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(systemInfo);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/systeminfo/{id}", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(result);

            return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(result);
        }
    }
}