using System;
using System.Collections.Generic;
using System.Text;

namespace PrintManagement.ServiceAPI.Common {
    public class ApiSuccessResult<T> : ApiResultBase<T> {
        public ApiSuccessResult(T resultObj) {
            IsSuccessed = true;
            ResultObj = resultObj;
        }

        public ApiSuccessResult() {
            IsSuccessed = true;
        }
    }
}