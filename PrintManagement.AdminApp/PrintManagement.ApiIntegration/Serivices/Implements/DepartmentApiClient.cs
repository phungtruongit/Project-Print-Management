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
    public class DepartmentApiClient : IDepartmentApiClient {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppSettings _appSettings;

        public DepartmentApiClient(IHttpClientFactory httpClientFactory,
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

        public async Task<ApiResultDTO<bool>> CreateDepartment(DepartmentDTO department) {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            client.BaseAddress = new Uri(_appSettings.BaseAddress);
            var json = JsonConvert.SerializeObject(department);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/department", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(result);

            return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(result);
        }

        public async Task<ApiResultDTO<bool>> DeleteDepartment(Guid id) {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_appSettings.BaseAddress);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.DeleteAsync($"/api/department/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(body);

            return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(body);
        }

        public async Task<ApiResultDTO<PagedResult<DepartmentDTO>>> GetAllDepartment(string keyword, int pageSize, int pageIndex) {
            var client = _httpClientFactory.CreateClient();
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.BaseAddress = new Uri(_appSettings.BaseAddress);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/department/paging?keyword={keyword}&pageSize={pageSize}&pageIndex={pageIndex}");
            var body = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ApiResultDTO<PagedResult<DepartmentDTO>>>(body);
            return data;
        }

        public async Task<ApiResultDTO<DepartmentDTO>> GetDepartmentById(Guid id) {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_appSettings.BaseAddress);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync($"/api/department/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiResultDTO<DepartmentDTO>>(body);

            return JsonConvert.DeserializeObject<ApiResultDTO<DepartmentDTO>>(body);
        }

        public async Task<ApiResultDTO<bool>> UpdateDepartment(Guid id, DepartmentDTO department) {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_appSettings.BaseAddress);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(department);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/department/{id}", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(result);

            return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(result);
        }
    }
}