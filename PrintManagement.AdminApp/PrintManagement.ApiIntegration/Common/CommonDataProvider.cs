using Newtonsoft.Json;
using PrintManagement.ApiIntegration.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PrintManagement.ApiIntegration.Common {
    public class CommonDataProvider {
        public static async Task<string> GetNameUserGroupByIdAsync(Guid? id, string token) {
            if (id == null)
                return String.Empty;
            var userGroup = await GetAsync<ApiResultDTO<UserGroupDTO>>($"/api/usergroup/{id}", token);
            return userGroup.ResultObj.Name;
        }

        public static async Task<string> GetNameDepartmentByIdAsync(Guid? id, string token) {
            if (id == null)
                return String.Empty;
            var department = await GetAsync<ApiResultDTO<DepartmentDTO>>($"/api/department/{id}", token);
            return department.ResultObj.Name;
        }

        public static async Task<string> GetNameUserByIdAsync(Guid? id, string token) {
            if (id == null)
                return String.Empty;
            var user = await GetAsync<ApiResultDTO<UserDTO>>($"/api/user/{id}", token);
            return user.ResultObj.Name;
        }

        public static async Task<string> GetNamePrinterByIdAsync(Guid? id, string token) {
            if (id == null)
                return String.Empty;
            var printer = await GetAsync<ApiResultDTO<PrinterDTO>>($"/api/printer/{id}", token);
            return printer.ResultObj.Name;
        }
        public static async Task<string> GetNameDocumentByIdAsync(Guid? id, string token) {
            if (id == null)
                return String.Empty;
            var document = await GetAsync<ApiResultDTO<DocumentDTO>>($"/api/document/{id}", token);
            return document.ResultObj.Name;
        }

        public static async Task<TResponse> GetAsync<TResponse>(string url, string token) {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7201");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode) {
                TResponse myDeserializedObjList = (TResponse)JsonConvert.DeserializeObject(body,
                    typeof(TResponse));

                return myDeserializedObjList;
            }
            return JsonConvert.DeserializeObject<TResponse>(body);
        }
    }
}
