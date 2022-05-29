using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PrintManagement.Infrastructure;
using PrintManagement.ServiceAPI.RepositoryServices;
using PrintManagement.ServiceAPI.Services;
using System.Text;
using VCSLib.HMAC;

var builder = WebApplication.CreateBuilder(args);
NLog.LogManager.LoadConfiguration(Path.Combine(Directory.GetCurrentDirectory(), "nlog.config"));
ConfigurationManager Configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddSingleton(option => builder.Configuration);
builder.Services.AddSingleton<ILogger, LoggerManager>();
builder.Services.AddDbContext<PrintManagementContext>(options => 
        options
        //.UseLazyLoadingProxies()
        .UseSqlServer(Configuration.GetConnectionString("PrintManagement")));

builder.Services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(opt =>
       {
           opt.TokenValidationParameters = new TokenValidationParameters
           {
               //tự cấp token
               ValidateIssuer = false,
               ValidateAudience = false,

               //ký vào token
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AppSettings:SecretKey"])),

               ClockSkew = TimeSpan.Zero
           };
       });

// Auto Scan & Add Mapping in project
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add services Repository
builder.Services.AddScoped<IBackupConfigRepository, BackupConfigRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IEmailConfigRepository, EmailConfigRepository>();
builder.Services.AddScoped<IPrinterRepository, PrinterRepository>();
builder.Services.AddScoped<IPrintUsageLogRepository, PrintUsageLogRepository>();
builder.Services.AddScoped<ISystemConfigOptionRepository, SystemConfigOptionRepository>();
builder.Services.AddScoped<ISystemInfoRepository, SystemInfoRepository>();
builder.Services.AddScoped<IUserGroupRepository, UserGroupRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserConfigRepository, UserConfigRepository>();
builder.Services.AddScoped<IFinancialConfigRepository, FinancialConfigRepository>();
builder.Services.AddScoped<IWatermarkConfigRepository, WatermarkConfigRepository>();

// HashcodeHMAC
builder.Services.AddTransient<IHashCodeHMAC, HashCodeHMAC>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PrintManagement.ServiceAPI", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JSON Web Token based security",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                   },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.Use(async (context, next) => {
    context.Request.EnableBuffering();
    await next();
});

app.MapControllers();

app.Run();
