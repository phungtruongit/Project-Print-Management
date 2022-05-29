using System;
using System.Collections.Generic;
using System.Text;

namespace PrintManagement.ApiIntegration.Common {
    public class ApiResultDTO<T> {
        public ApiResultDTO() {
        }
        public ApiResultDTO(bool isSuccess, string message, T dataObj) {
            IsSuccessed = isSuccess;
            Message = message;
            ResultObj = dataObj;
        }
        public ApiResultDTO(bool isSuccess, string message) {
            IsSuccessed = isSuccess;
            Message = message;
        }
        public bool IsSuccessed { get; set; }

        public string Message { get; set; }

        public T ResultObj { get; set; }
    }
}