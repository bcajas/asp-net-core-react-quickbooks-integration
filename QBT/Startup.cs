using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using QBT.Data;
using QBT.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace QBT
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
            
            services.AddAuthentication(sharedOptions => { })
                .AddCookie()
                .AddOpenIdConnect("QuickBooks", "QuickBooks", openIdConnectOptions =>
                {
                    openIdConnectOptions.Authority = "QuickBooks";
                    openIdConnectOptions.UseTokenLifetime = true;
                    openIdConnectOptions.ClientId = "ABRTd3zJPhGcWjGKIyA9gtrKPSzWZQfWsXHRZU0IPMgt6xbMNB"; //client id & client secret need to be set w/ your app keys
                    openIdConnectOptions.ClientSecret = "ugWP7V6pdQJLBKPSkpcf44J6Uhdpp4Kg2lsz9E2B";
                    openIdConnectOptions.ResponseType = OpenIdConnectResponseType.Code;
                    openIdConnectOptions.MetadataAddress = "https://developer.api.intuit.com/.well-known/openid_sandbox_configuration/";    //development path
                    openIdConnectOptions.ProtocolValidator.RequireNonce = false;
                    openIdConnectOptions.SaveTokens = true;
                    openIdConnectOptions.GetClaimsFromUserInfoEndpoint = true;
                    openIdConnectOptions.Scope.Add("openid");
                    openIdConnectOptions.Scope.Add("phone");
                    openIdConnectOptions.Scope.Add("email");
                    openIdConnectOptions.Scope.Add("address");
                    openIdConnectOptions.Scope.Add("com.intuit.quickbooks.accounting");
                })
                .AddIdentityServerJwt();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
            
            services.AddControllersWithViews();
            services.AddRazorPages();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
