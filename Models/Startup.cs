using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Milestone3WebApp.Data;
using Microsoft.AspNetCore.Identity;
using Milestone3WebApp.Models;

namespace Milestone3WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();
    services.AddDistributedMemoryCache();
    
    services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    services.AddAuthentication()
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login"; // Redirect to login page
        });
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseSession(); // Ensure this is BEFORE authentication
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    });
}
    }
}