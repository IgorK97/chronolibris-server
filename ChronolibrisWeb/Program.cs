using System.Reflection;
using System.Security.Claims;
using System.Text;
using Chronolibris.Application.Extensions;
using Chronolibris.Infrastructure.DataAccess.Hangfire;
using Chronolibris.Infrastructure.DatabaseChecker;
using Chronolibris.Infrastructure.DependencyInjection;
using ChronolibrisWeb.Middleware;
using ChronolibrisWeb.Middleware.Hangfire;
using ChronolibrisWeb.Utils;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Уровень логирования
    .WriteTo.Console()    // Оставляем вывод в консоль
    .WriteTo.File("logs/parsing_log_.txt",
        rollingInterval: RollingInterval.Day, // Новый файл каждый день
        retainedFileCountLimit: 30,            // Хранить логи за последние 7 дней
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// Очистка карты клеймов до настройки аутентификации
//JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// CORS
var allowAVDCORSPolicy = "_allowAVDCORSPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(allowAVDCORSPolicy,
        policy =>
        {
            policy.WithOrigins(/*"http://localhost:5173", "http://localhost:45457",*/ "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

// Настройка логирования и уровней
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Настройка уровней логирования
builder.Logging.AddFilter("Microsoft", LogLevel.Warning)
    .AddFilter("System", LogLevel.Warning)
    .AddFilter("Default", LogLevel.Information);

// Инфраструктурные сервисы
builder.Services.AddExceptionMapper();
builder.Services.AddDatabaseInfrastructure(builder.Configuration);
builder.Services.AddIdentityRealization(builder.Configuration);
//builder.Services.AddFileProviderInfrastructure(builder.Configuration);
builder.Services.AddFileServices(builder.Configuration);
builder.Services.AddFb2Converter(builder.Configuration);

//builder.Services.AddHangfireInfrastructure(builder.Configuration);
//GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 3 });

// Конфигурация аутентификации с использованием JWT-токенов
builder.Services.AddAuthentication(options =>
{
    // Устанавливаем JWT как схему по умолчанию для всего
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            RoleClaimType = ClaimsIdentity.DefaultRoleClaimType
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Cookies.Delete("token");
                }
                return Task.CompletedTask;
            }
            //TODO: Реализовать черный список токенов (например, при выходе пользователя из системы) и проверять его здесь
            //OnTokenValidated = context =>
            //{
            //    var jti = context.SecurityToken.Id;
            //    var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

            //    if (cache.TryGetValue(jti, out _))
            //    {
            //        context.Fail("Token is blacklisted");
            //    }
            //    return Task.CompletedTask;
            //}
        };
    });

//builder.Services.Configure<ForwardedHeadersOptions>(options =>
//{
//    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

//    options.KnownNetworks.Clear();
//    options.KnownProxies.Clear();
//});

// Авторизация
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("admin"));
});

builder.Services.AddApplicationServices();
builder.Services.AddRateLimiter(options =>
{
    options.AddChronolibrisRateLimiter();
});
builder.Services.AddRequestTimeouts(options =>
{
    options.DefaultPolicy = new RequestTimeoutPolicy
    {
        Timeout = TimeSpan.FromSeconds(60)
    };
});


builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = false; //автоматически прерывает выполнение при ошибке валидации
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values.SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage);

        var detail = string.Join("; ", errors);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Ошибка валидации",
            Detail = detail,
            Instance = context.HttpContext.Request.Path
        };
        return new BadRequestObjectResult(problemDetails);
    };
});

// NSwag (OpenAPI/Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(options =>
{
    options.Title = "My API";

    options.AddSecurity("Bearer", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Description = "Введите: Bearer {ваш_токен}"
    });

    options.OperationProcessors.Add(
        new NSwag.Generation.Processors.Security.OperationSecurityScopeProcessor("Bearer"));
});

// HTTP Logging
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    //время на получение заголовков запроса
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(15);

    //Минимальная скорость передачи тела запроса (байт в секунду)
    //Если клиент шлет данные медленнее 240 байт/сек дольше 5 секунд (GracePeriod),
    //соединение будет разорвано
    serverOptions.Limits.MinRequestBodyDataRate =
        new MinDataRate(bytesPerSecond: 240, gracePeriod: TimeSpan.FromSeconds(5));

    //То же самое для ответа сервера клиенту
    serverOptions.Limits.MinResponseDataRate =
        new MinDataRate(bytesPerSecond: 240, gracePeriod: TimeSpan.FromSeconds(5));

    //Максимальный размер заголовков (тоже защита от переполнения памяти)
    serverOptions.Limits.MaxRequestHeadersTotalSize = 32768; //32 KB
});

var app = builder.Build();
var configuration = app.Configuration;
app.UseMiddleware<ExceptionHandlingMiddleware>();

//app.UseForwardedHeaders(new ForwardedHeadersOptions //если бы я использовал нгинкс, это было бы критично
//{
//    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
//    options.KnownNetworks.Clear();
//    options.KnownProxies.Clear();
//});
//proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
//proxy_set_header X-Real-IP $remote_addr;

//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}

app.UseHttpsRedirection();
app.UseHttpLogging();
if(app.Environment.IsDevelopment())
    app.UseCors(allowAVDCORSPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.UseRequestTimeouts();
app.UseRateLimiter();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
//app.UseHangfireDashboard("/hangfire", new DashboardOptions
//{
//    Authorization = new[] { new HangfireAuthFilter() },
//    DashboardTitle = "My App Jobs"
//});

app.UseOpenApi(options =>
{
    options.PostProcess = (document, request) =>
    {
        document.Servers.Clear();
        document.Servers.Add(new NSwag.OpenApiServer
        {
            Url = "https://localhost:7016",
            Description = "Local HTTPS (dev)"
        });
    };
});
app.UseSwaggerUI();
app.MapControllers();

// Запуск проверки БД (миграций) при старте приложения
app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DatabaseChecker.CheckDatabase(app.Services, configuration);
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database seeding failed");
    }
});
app.Run();