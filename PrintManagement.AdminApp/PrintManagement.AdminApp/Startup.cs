using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using PrintManagement.AdminApp.Services;
using PrintManagement.ApiIntegration;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using System.Configuration;
using PrintManagement.MailHub.EmailProvider;

namespace PrintManagement.AdminApp {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            NLog.LogManager.LoadConfiguration(Path.Combine(Directory.GetCurrentDirectory(), "nlog.config"));
            
            // add Service to the container
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            services.AddHttpClient();
            services.AddSingleton<ILogger, LoggerManager>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => {
                    options.LoginPath = "/Login/Index";
                    options.AccessDeniedPath = "/User/Forbidden/";
                });
            services.AddControllersWithViews()
                     .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<LoginRequestValidator>());
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // add Service Api Client
            services.AddSingleton<BaseApiClient>();
            services.AddTransient<IUserApiClient, UserApiClient>();
            services.AddTransient<IDepartmentApiClient, DepartmentApiClient>();
            services.AddTransient<ISystemInfoApiClient, SystemInfoApiClient>();
            services.AddTransient<IPrinterUsageLogApiClient, PrinterUsageLogApiClient>();
            services.AddTransient<IPrinterApiClient, PrinterApiClient>();
            services.AddTransient<IDocumentApiClient, DocumentApiClient>();
            services.AddTransient<IEmailConfigApiClient, EmailConfigApiClient>();
            services.AddTransient<IBackupConfigApiClient, BackupConfigApiClient>();
            services.AddScoped<IUserConfigApiClient, UserConfigApiClient>();
            services.AddScoped<IFinancialConfigApiClient, FinancialConfigApiClient>();
            services.AddScoped<IWatermarkConfigApiClient, WatermarkConfigApiClient>();

            // add Service DevExpress
            services.AddControllersWithViews();
            services.AddDevExpressControls();
            services
                .AddMvc()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
            services.ConfigureReportingServices(configurator => {
                configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
                    viewerConfigurator.UseCachedReportSourceBuilder();
                });
            });
            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest);
            services.ConfigureReportingServices(configurator => {
                configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
                    viewerConfigurator.UseCachedReportSourceBuilder();
                });
            });

            // add Service MailSMTP
            var emailConfig = Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddScoped<IEmailSender, EmailSender>();

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseDevExpressControls();
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}