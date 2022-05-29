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
using VCSLib.HMAC;

namespace PrintManagement.ApiIntegration {
    public class PrinterUsageLogApiClient : IPrinterUsageLogApiClient {
        private readonly AppSettings _appSettings;

        public PrinterUsageLogApiClient(AppSettings appSettings) {
            _appSettings = appSettings;
        }

        //public async Task<ApiResultDTO<string>> Authenticate(LoginRequest request) {
        //    var json = JsonConvert.SerializeObject(request);
        //    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        //    var client = new HttpClient();
        //    client.BaseAddress = new Uri(_appSettings.BaseAddress);
        //    var response = await client.PostAsync("/api/user/login", httpContent);
        //    if (response.IsSuccessStatusCode) {
        //        return JsonConvert.DeserializeObject<ApiResultDTO<string>>(await response.Content.ReadAsStringAsync());
        //    }

        //    return JsonConvert.DeserializeObject<ApiResultDTO<string>>(await response.Content.ReadAsStringAsync());
        //}

        public async Task<ApiResultDTO<bool>> CreatePrinterUsageLog(PrinterUsageLogDTO printerUsageLog, string token) {

            var client = new HMACClient(new HMACDelegatingHandler(new HMACHttpClientHandler(), new HashCodeHMAC()));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.BaseAddress = new Uri(_appSettings.BaseAddress);
            var json = JsonConvert.SerializeObject(printerUsageLog);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/printusagelog", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(result);

            return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(result);
        }

        //public async Task<ApiResultDTO<bool>> DeletePrinterUsageLog(Guid id) {
        //    var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
        //    var client = new HttpClient();
        //    client.BaseAddress = new Uri(_appSettings.BaseAddress);
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
        //    var response = await client.DeleteAsync($"/api/printusagelog/{id}");
        //    var body = await response.Content.ReadAsStringAsync();
        //    if (response.IsSuccessStatusCode)
        //        return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(body);

        //    return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(body);
        //}
        //public async Task<ApiResultDTO<List<PrinterUsageLogDTO>>> GetAllPrinterUsageLog() {
        //    var client = new HttpClient();
        //    var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

        //    client.BaseAddress = new Uri(_appSettings.BaseAddress);
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
        //    var response = await client.GetAsync($"/api/printusagelog/");
        //    var body = await response.Content.ReadAsStringAsync();
        //    var users = JsonConvert.DeserializeObject<ApiResultDTO<List<PrinterUsageLogDTO>>>(body);
        //    return users;
        //}

        //public async Task<ApiResultDTO<PagedResult<PrinterUsageLogDTO>>> GetAllPrinterUsageLog(string keyword, int pageSize, int pageIndex) {
        //    var client = new HttpClient();
        //    var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

        //    client.BaseAddress = new Uri(_appSettings.BaseAddress);
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
        //    var response = await client.GetAsync($"/api/printusagelog/paging?keyword={keyword}&pageSize={pageSize}&pageIndex={pageIndex}");
        //    var body = await response.Content.ReadAsStringAsync();
        //    var data = JsonConvert.DeserializeObject<ApiResultDTO<PagedResult<PrinterUsageLogDTO>>>(body);
        //    return data;
        //}

        //public async Task<ApiResultDTO<PrinterUsageLogDTO>> GetPrinterUsageLogById(Guid id) {
        //    var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
        //    var client = new HttpClient();
        //    client.BaseAddress = new Uri(_appSettings.BaseAddress);
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
        //    var response = await client.GetAsync($"/api/printusagelog/{id}");
        //    var body = await response.Content.ReadAsStringAsync();
        //    if (response.IsSuccessStatusCode)
        //        return JsonConvert.DeserializeObject<ApiResultDTO<PrinterUsageLogDTO>>(body);

        //    return JsonConvert.DeserializeObject<ApiResultDTO<PrinterUsageLogDTO>>(body);
        //}

        //public async Task<ApiResultDTO<bool>> UpdatePrinterUsageLog(Guid id, PrinterUsageLogDTO printerUsageLog) {
        //    var client = new HttpClient();
        //    client.BaseAddress = new Uri(_appSettings.BaseAddress);
        //    var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

        //    var json = JsonConvert.SerializeObject(printerUsageLog);
        //    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        //    var response = await client.PutAsync($"/api/printusagelog/{id}", httpContent);
        //    var result = await response.Content.ReadAsStringAsync();
        //    if (response.IsSuccessStatusCode)
        //        return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(result);

        //    return JsonConvert.DeserializeObject<ApiResultDTO<bool>>(result);
        //}
    }
}