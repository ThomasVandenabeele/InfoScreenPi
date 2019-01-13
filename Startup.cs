using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;

using InfoScreenPi;
using InfoScreenPi.Hubs;
using InfoScreenPi.Infrastructure.Services;

using InfoScreenPi.Infrastructure;
using System.Reflection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace InfoScreenPi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<InfoScreenContext>(options => options.UseSqlite("Data Source=db/InfoScreenDB.db"));

            // Services
            services.AddScoped<IDataService, DataService>();
            services.AddScoped<IMembershipService, MembershipService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IRSSService, RSSService>();

            services.AddDataProtection();
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/config/login");
                    options.LogoutPath = new PathString("/config/LogOut");
                });

            //Polices
            services.AddAuthorization(options =>
            {
                // inline policies
                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.RequireRole("Admin");
                });

            });

            // services.AddSession(/* options go here */);
            services.AddSession();

            services.AddSignalR();

            //services.AddHostedService<RefreshRSSTimedHostedService>();
            //services.AddHostedService<CheckItemStateTimedHostedService>();
            //services.AddHostedService<ScreenTimedHostedService>();
            services.AddHostedService<ScreenBackgroundService>();
            services.Configure<FormOptions>(x => x.MultipartBodyLengthLimit = 1_074_790_400);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseSession();

            // var cookiePolicyOptions = new CookiePolicyOptions
            // {
            //     MinimumSameSitePolicy = SameSiteMode.Strict,
            // };
            // app.UseCookiePolicy(cookiePolicyOptions);
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseSession();

            app.UseSignalR(routes =>
            {
                routes.MapHub<WebSocketHub>("/signalr");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Config}/{action=Index}/{id?}");
            });

        }
    }
}
