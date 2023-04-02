using E_Project_.Data;
using E_Project_.Models;
using E_Project_.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Project_
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
            services.AddControllersWithViews();

            // DbContext => Database Configuration 
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("DefaultConnection")
                ));


            //  configure Identity
            //services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<ApplicationDbContext>();


            //services.AddIdentity<IdentityUser , IdentityRole>()

            //    .AddDefaultTokenProviders()
            //    .AddDefaultUI()
            //    .AddEntityFrameworkStores<ApplicationDbContext>();


            //  Here force confirmation mail in register
            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                 .AddDefaultTokenProviders();


            // configure Session 
            services.AddHttpContextAccessor();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            // Rgester Email Service 
            services.AddTransient<IEmailSender, EmailSender>();

            // 1-RazorPages
            services.AddRazorPages();


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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            //configure auth
            app.UseAuthentication();

            app.UseAuthorization();

            // Congigure Session In MiddleWare
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                // 2-RazorPages

                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
