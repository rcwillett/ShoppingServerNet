using Microsoft.AspNetCore.Authentication.Cookies;
using ShoppingApplicationAPINET.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Forbidden/";
    });

string CORSPolicy = "CORSPolicy";
string[] Origins = builder.Configuration.GetSection("AllowedHosts").GetChildren().ToArray().Select(c => c.Value).ToArray();
builder.Services.AddCors(options =>
{
    options.AddPolicy(CORSPolicy, policy => {
        policy.WithOrigins(Origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ShoppingContext>(options => {
    string? connectionString = builder.Configuration.GetConnectionString("MySQLConnection");
    if (String.IsNullOrEmpty(connectionString))
    {
        throw new Exception("Failed to retrieve connection string!");
    }
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthentication();

//app.UseHttpsRedirection();

app.UseCors(CORSPolicy);

app.UseAuthorization();

app.UseCookiePolicy(
    new CookiePolicyOptions {
        Secure = app.Environment.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.Always,
        MinimumSameSitePolicy = app.Environment.IsDevelopment() ? SameSiteMode.Strict : SameSiteMode.None,
    }
);;

app.MapControllers();

app.Run();

