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
            var usergroupName = await GetAsync<ApiResultDTO<UserGroupDTO>>($"/api/usergroup/{id}", token);
            return usergroupName.ResultObj.Name;
        }

        public static async Task<string> GetNameDepartmentByIdAsync(Guid? id, string token) {
            if (id == null)
                return String.Empty;
            var departmentName = await GetAsync<ApiResultDTO<DepartmentDTO>>($"/api/department/{id}", token);
            return departmentName.ResultObj.Name;
        }

        public static async Task<string> GetNameUserByIdAsync(Guid? id, string token) {
            if (id == null)
                return String.Empty;
            var userName = await GetAsync<ApiResultDTO<UserDTO>>($"/api/user/{id}", token);
            return userName.ResultObj.Name;
        }

        public static async Task<string> GetNamePrinterByIdAsync(Guid? id, string token) {
            if (id == null)
                return String.Empty;
            var printerName = await GetAsync<ApiResultDTO<PrinterDTO>>($"/api/printer/{id}", token);
            return printerName.ResultObj.Name;
        }
        public static async Task<string> GetNameDocumentByIdAsync(Guid? id, string token) {
            if (id == null)
                return String.Empty;
            var documentName = await GetAsync<ApiResultDTO<DocumentDTO>>($"/api/document/{id}", token);
            return documentName.ResultObj.Name;
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
