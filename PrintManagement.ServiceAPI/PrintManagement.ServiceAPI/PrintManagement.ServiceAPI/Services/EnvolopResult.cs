using PrintManagement.ServiceAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintManagement.ServiceAPI.Services {
    public static class EnvolopResult {
        public static ApiResultDTO<T> Envelope<T>(this T resultObj) {
            return new ApiResultDTO<T>(true, string.Empty, resultObj);
        }
        public static ApiResultDTO<Exception> Envelope(this Exception ex) {
            return new ApiResultDTO<Exception>(false, ex.Message);
        }
    }
}
