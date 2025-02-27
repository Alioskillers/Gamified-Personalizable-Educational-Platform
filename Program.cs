using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Milestone3WebApp.Models;
using Milestone3WebApp.Data;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env
Env.Load();

// Retrieve the connection string from the environment variable
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("DATABASE_URL is not set in the environment variables.");
}

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Use the environment variable connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5201); // HTTP
    options.ListenAnyIP(7167, listenOptions => listenOptions.UseHttps()); // HTTPS
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseHsts();
app.UseStaticFiles();
app.UseRouting();

// Enable session BEFORE authentication/authorization
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();