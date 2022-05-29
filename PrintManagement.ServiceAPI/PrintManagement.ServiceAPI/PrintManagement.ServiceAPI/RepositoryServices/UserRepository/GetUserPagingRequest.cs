using PrintManagement.ServiceAPI.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrintManagement.ServiceAPI.RepositoryServices {
    public class GetUserPagingRequest : PagingRequestBase {
        public string Keyword { get; set; }
    }
}