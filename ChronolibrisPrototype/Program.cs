using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using Chronolibris.Application.Extensions;
using Chronolibris.Application.Fb2Converter.Interfaces;
using Chronolibris.Application.Handlers;
using Chronolibris.Infrastructure.DataAccess.DependencyInjection;
using Chronolibris.Infrastructure.DataAccess.Hangfire;
using Chronolibris.Infrastructure.DatabaseChecker;
using Chronolibris.Infrastructure.DependencyInjection;
using ChronolibrisWeb.Hangfire;
using ChronolibrisWeb.Middleware;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddDatabaseInfrastructure(builder.Configuration);
builder.Services.AddIdentityRealization(builder.Configuration);
//builder.Services.AddFileProviderInfrastructure(builder.Configuration);
builder.Services.AddFileServices(builder.Configuration);
builder.Services.AddFb2Converter(builder.Configuration);
builder.Services.AddHangfireInfrastructure(builder.Configuration);
builder.Services.AddJobs(builder.Configuration);

GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 3 });

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

// Авторизация
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("admin"));
});

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = false;
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

var app = builder.Build();
var configuration = app.Configuration;
app.UseMiddleware<ExceptionHandlingMiddleware>();



// Configure the HTTP request pipeline.
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

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthFilter() },
    DashboardTitle = "My App Jobs"
});


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


//static async Task TestConvertAsync(IServiceProvider services)
//{
//    var fb2Path = Path.Combine(AppContext.BaseDirectory, "test.fb2");

//    if (!File.Exists(fb2Path))
//    {
//        Console.WriteLine($"[TEST] Файл не найден: {fb2Path}");
//        Console.WriteLine($"[TEST] Ожидается по пути: {fb2Path}");
//        return;
//    }

//    using var scope = services.CreateScope();
//    var converter = scope.ServiceProvider.GetRequiredService<IFb2Converter>();

//    Console.WriteLine("[TEST] Начинаем конвертацию...");

//    try
//    {
//        await using var stream = File.OpenRead(fb2Path);
//        var result = await converter.ConvertAsync(stream);

//        Console.WriteLine($"[TEST] ✓ BookId:     {result.BookId}");
//        Console.WriteLine($"[TEST] ✓ Название:   {result.Meta.Title}");
//        Console.WriteLine($"[TEST] ✓ Язык:       {result.Meta.Lang}");
//        Console.WriteLine($"[TEST] ✓ Автор:      {result.Meta.Authors?.FirstOrDefault()?.Last}");
//        Console.WriteLine($"[TEST] ✓ Элементов:  {result.TotalElements}");
//        Console.WriteLine($"[TEST] ✓ Фрагментов: {result.PartFiles.Count}");
//        Console.WriteLine($"[TEST] ✓ toc.json:   {result.TocFile.SizeBytes} байт");
//        Console.WriteLine($"[TEST] ─────────────────────────────────────");

//        foreach (var part in result.PartFiles)
//        {
//            Console.WriteLine(
//                $"[TEST]   {part.FileName}  " +
//                $"s={part.GlobalStart,-5} e={part.GlobalEnd,-5}  " +
//                $"xps=[{string.Join(",", part.XpStart!)}]  " +
//                $"{part.SizeBytes} байт");
//        }
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"[TEST] ✗ Ошибка: {ex.Message}");
//        Console.WriteLine(ex.StackTrace);
//    }
//}