using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductManagementMT20.Interfaces;
using ProductManagementMT20.Models;
using ProductManagementMT20.Models.Entities;
using ProductManagementMT20.Policies.Handlers;
using ProductManagementMT20.Policies.Requirements;
using ProductManagementMT20.Services;

namespace ProductManagementMT20
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //builder.Services.AddDefaultIdentity<ApplicationUser>().AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("FiveYearsEmployee", policy =>
                policy.Requirements.Add(new FiveYearsRequirement(5)));

                options.AddPolicy("AdminClaimPolicy", policy =>
                policy.RequireClaim("AdminClaim"));
            });

            builder.Services.AddScoped<IAuthorizationHandler, FiveYearsHandler>();

            builder.Services.ConfigureApplicationCookie(option =>
            {
                option.AccessDeniedPath = "/Auth/AccessDenied";
            });


            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Create an async scope to ensure roles exist
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Instead of 'await', use GetAwaiter().GetResult() to force synchronous execution
                CreateRoles(roleManager).GetAwaiter().GetResult();
            }

            // Async method to create roles
            async Task CreateRoles(RoleManager<IdentityRole> roleManager)
            {
                string[] roleNames = { "Admin", "User" };

                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
