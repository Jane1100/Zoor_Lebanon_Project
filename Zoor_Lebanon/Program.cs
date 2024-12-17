using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Zoor_Lebanon.Models;
using Zoor_Lebanon.Models.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
var secretKey = builder.Configuration["JwtSettings:SecretKey"];
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton(new JwtHelper(secretKey));

// Configure DbContext with the connection string
builder.Services.AddDbContext<zoor_lebanonContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), 
       new MySqlServerVersion(new Version(8, 0, 26))));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthorization();



app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
