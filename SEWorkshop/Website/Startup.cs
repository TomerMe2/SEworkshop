using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SEWorkshop.ServiceLayer;
using Website.Hubs;

namespace Website
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
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(15);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddRazorPages();
            services.AddSingleton<IUserManager, UserManager>();
            services.AddSingleton(typeof(MessagesObserver), typeof(MessagesObserver));
            services.AddSingleton(typeof(PurchaseObserver), typeof(PurchaseObserver));
            services.AddSingleton(typeof(OwnershipRequestObserver), typeof(OwnershipRequestObserver));
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.Use((httpContext, nextMiddleware) =>
            {
                httpContext.Session.Set("JustHoldSomething", new byte[] { 0, 2, 3});

                return nextMiddleware();
            });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<MessagesNotificationsHub>("/notificationshub");
                endpoints.MapHub<PurchasesNotificationsHub>("/purchasenotificationshub");
                endpoints.MapHub<OwnershipRequestHub>("/ownershiprequesthub");
            });
        }
    }
}
