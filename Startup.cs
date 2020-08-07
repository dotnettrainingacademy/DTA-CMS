using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using dtacms.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace dtacms
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
           services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            //CreateRolesUsers(serviceProvider).Wait();
        }

        // Add couple of user - Admin ,Guest
        private async Task CreateRolesUsers(IServiceProvider serviceProvider)
        {
            // UserManager, Rolemanager
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Intitalize roles
            string[] roleNames = {"Admin", "Guest"};

            IdentityResult roleResult;

            foreach (var rolename in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(rolename);
                if(!roleExist){
                    // Create the role
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(rolename));
                }
            }

            // Create User
            IdentityUser user = await UserManager.FindByEmailAsync("admin@dta.com");

            if(user == null)
            {
                user = new IdentityUser(){
                    UserName = "Admin",
                    Email = "admin@dta.com"
                };
                await UserManager.CreateAsync(user, "Admin@123");
            }
            await UserManager.AddToRoleAsync(user, roleNames[0]);

            // Guest User
            IdentityUser guest = await UserManager.FindByEmailAsync("guest@dta.com");

            if(guest == null)
            {
                guest = new IdentityUser(){
                    UserName = "Guest",
                    Email = "guest@dta.com"
                };
                await UserManager.CreateAsync(guest, "Guest@123");
            }
            await UserManager.AddToRoleAsync(guest, roleNames[1]);
        }    
    }
}
