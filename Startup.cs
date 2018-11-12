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
using InfoScreenPi.Infrastructure.Repositories;
using InfoScreenPi.Infrastructure.Services;
using InfoScreenPi.Infrastructure.WebSocketManager;

using InfoScreenPi.Infrastructure;
using System.Reflection;

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

            services.AddDbContext<InfoScreenContext>(options => options.UseSqlite("Data Source=InfoScreenDB.db"));

            // Repositories
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IItemKindRepository, ItemKindRepository>();
            services.AddScoped<IBackgroundRepository, BackgroundRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ILoggingRepository, LoggingRepository>();
            services.AddScoped<IRssFeedRepository, RssFeedRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
 
            // Services
            services.AddScoped<IMembershipService, MembershipService>();
            services.AddScoped<IEncryptionService, EncryptionService>();

            services.AddDataProtection();
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/config/login");
                    options.LogoutPath = new PathString("/config/LogOut");
                });
 
            services.AddTransient<WebSocketConnectionManager>();

            foreach(var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if(type.GetTypeInfo().BaseType == typeof(WebSocketHandler))
                {
                    services.AddSingleton(type);
                }
            }

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

            // var webSocketOptions = new WebSocketOptions()
            // {
            //     KeepAliveInterval = TimeSpan.FromSeconds(120),
            //     ReceiveBufferSize = 4 * 1024
            // };
            // app.UseWebSockets(webSocketOptions);
            //IServiceProvider serviceProvider = Microsoft.Extensions.DependencyInjection.ServiceProvider;
            //app.MapWebSocketManager("/ws", serviceProvider.GetService<TestMessageHandler>());
            //WebSocketHandler handler = new TestMessageHandler();
            //app.Map("/ws", (_app) => _app.UseMiddleware<WebSocketManagerMiddleware>());

            app.UseSignalR(routes =>
            {
                routes.MapHub<SignalRHub>("/signalr");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Screen}/{action=Index}/{id?}");
            });
            
        }
    }
}
