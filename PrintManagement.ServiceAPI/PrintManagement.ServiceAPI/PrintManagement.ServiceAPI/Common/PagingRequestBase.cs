﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PrintManagement.ServiceAPI.Common
{
    public class PagingRequestBase
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}