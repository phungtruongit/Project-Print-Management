using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using PrintManagement.ApiIntegration;
using PrintManagement.ApiIntegration.Common;

namespace PrintManagement.AdminApp.Controllers {
    public class AboutController : BaseController {
        private readonly BaseApiClient _baseApiClient;
        private readonly ISystemInfoApiClient _systemInfoApiClient;
        private readonly ILogger _logger;

        public AboutController(ISystemInfoApiClient systemInfoApiClient,
            ILogger logger, BaseApiClient baseApiClient) {
            _logger = logger;
            _systemInfoApiClient = systemInfoApiClient;
            _baseApiClient = baseApiClient;
        }

        public async Task<IActionResult> Index() {
            var data = await _systemInfoApiClient.GetSystemInfoMain();
            return View(data.ResultObj);
        }
    }
}
