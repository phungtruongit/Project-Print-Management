using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using PrintManagement.ApiIntegration;
using PrintManagement.ApiIntegration.Common;

namespace PrintManagement.AdminApp.Controllers {
    public class LoginController : Controller {
        private readonly IUserApiClient _userApiClient;
        private readonly AppSettings _appSettings;
        public LoginController(IUserApiClient userApiClient, IOptionsMonitor<AppSettings> optionsMonitor) {
            _userApiClient = userApiClient;
            _appSettings = optionsMonitor.CurrentValue;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginRequest request) {
            if (!ModelState.IsValid)
                return View(ModelState);

            var result = await _userApiClient.AuthenticateAsync(request);
            if (result.ResultObj == null) {
                ModelState.AddModelError("", result.Message);
                return View();
            }
            var userPrincipal = this.ValidateToken(result.ResultObj);

            var authProperties = new AuthenticationProperties {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                IsPersistent = false
            };

            // Set token into session
            HttpContext.Session.SetString(SystemConstants.AppSetting.Token, result.ResultObj);

            await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        userPrincipal,
                        authProperties);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ForgetPassword() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest request) {
            if (!ModelState.IsValid)
                return View(ModelState);

            var allUser = (await _userApiClient.GetAllUserAsync()).ResultObj;
            var user = allUser.FirstOrDefault(x => x.UserName == request.UserName && x.Email == request.Email);
            if (user != null) {

            }
            else {
                ModelState.AddModelError("", "Tài khoản không tồn tại");
                return View();
            }
            //var result = await _userApiClient.AuthenticateAsync(request);
            //if (result.ResultObj == null) {
            //    ModelState.AddModelError("", result.Message);
            //    return View();
            //}
            //var userPrincipal = this.ValidateToken(result.ResultObj);

            //var authProperties = new AuthenticationProperties {
            //    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
            //    IsPersistent = false
            //};

            //// Set token into session
            //HttpContext.Session.SetString(SystemConstants.AppSetting.Token, result.ResultObj);

            //await HttpContext.SignInAsync(
            //            CookieAuthenticationDefaults.AuthenticationScheme,
            //            userPrincipal,
            //            authProperties);

            return RedirectToAction("Index", "Home");
        }

        private ClaimsPrincipal ValidateToken(string jwtToken) {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;
            validationParameters.ValidateIssuer = false;
            validationParameters.ValidateAudience = false;
            validationParameters.IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secretkey));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }
    }
}