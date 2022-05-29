using System;
using System.Collections.Generic;
using System.Text;

namespace PrintManagement.ServiceAPI.Common
{
    public class ApiErrorResult<T> : ApiResultBase<T>
    {
        public string[] ValidationErrors { get; set; }

        public ApiErrorResult() {
        }

        public ApiErrorResult(string message)
        {
            IsSuccessed = false;
            Message = message;
        }

        public ApiErrorResult(string[] validationErrors)
        {
            IsSuccessed = false;
            ValidationErrors = validationErrors;
        }
    }
}