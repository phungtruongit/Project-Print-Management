using PrintManagement.ApiIntegration.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrintManagement.ApiIntegration {
    public class GetUserPagingRequest : PagingRequestBase {
        public string Keyword { get; set; }
    }
}