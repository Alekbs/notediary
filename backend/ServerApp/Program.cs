using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using Serilog;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ServerApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Security.Claims;
using ServerApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Настройка Serilog для логирования
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// Установите путь к wwwroot
builder.WebHost.UseWebRoot("wwwroot");

// Регистрация AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));



// Добавление остальных сервисов, если нужно
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Role.Admin.ToString(), policy => policy.RequireRole(Role.Admin.ToString()));
    options.AddPolicy(Role.Student.ToString(), policy => policy.RequireRole(Role.Student.ToString()));
    options.AddPolicy(Role.Teacher.ToString(), policy => policy.RequireRole(Role.Teacher.ToString()));
});
builder.Services.AddControllers(opts =>
{
    opts.Filters.Add(new AuthorizeFilter());
});



builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var key = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:SecretKey")!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
    options.Events = new JwtBearerEvents
    {

        OnTokenValidated = async (context) =>
        {
            Console.WriteLine("Token validated");
            var principal = context.Principal;

            // Получение userId из токена
            var userId = principal?.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
            Console.WriteLine(userId); // Логируем, чтобы убедиться, что токен передает корректный ID

            // Если userId отсутствует или не удается преобразовать в int, возвращаем
            if (userId == null || !int.TryParse(userId, out var userIdInt))
            {
                return;
            }

            var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

            // Получаем пользователя из базы данных
            var user = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userIdInt);

            if (user == null)
            {
                return;
            }

            var claimsIdentity = new ClaimsIdentity();

            // Добавляем роль пользователя в ClaimsIdentity
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Role));

            // Применяем обновлённый ClaimsIdentity к principal
            principal?.AddIdentity(claimsIdentity);
        }

    };

    
});


var app = builder.Build();

// Применение миграций при запуске
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();  // Применяем миграции
}

// Регистрация маршрутов
app.MapGet("/sign", async context =>
{
    context.Response.Redirect("wwwroot/pages/sign.html");
});
app.MapGet("/", async context =>
{
    context.Response.Redirect("wwwroot/pages/users.html");
});
app.MapGet("/users", async context =>
{
    context.Response.Redirect("wwwroot/pages/users.html");
});
app.MapGet("user-details", async context =>
{
    context.Response.Redirect("wwwroot/pages/user-details.html");
});
app.MapGet("/test", async context =>
{
    context.Response.Redirect("wwwroot/pages/test.html");
});
app.MapGet("/fronttest", async context =>
{
    context.Response.Redirect("wwwroot/pages/frontTest.html");
});

if (!app.Environment.IsProduction())
{
    app.UseStaticFiles(new StaticFileOptions
    {
        RequestPath = "/wwwroot",
        FileProvider = new PhysicalFileProvider("C:/Users/aleks/OneDrive/Документы/UA/ServerApp/wwwroot"),
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Append(
                "Cache-Control", "no-store, no-cache, must-revalidate, proxy-revalidate");
        }
    });
}



app.UseCors("AllowAll");
app.UseAuthentication(); 
app.UseAuthorization();
app.MapControllers();
app.Urls.Add("http://0.0.0.0:5000"); // Разрешаем подключения со всех интерфейсов

app.Run();
