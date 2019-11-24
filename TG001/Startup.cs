using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using TG001.Data;
using TG001.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace TG001
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
            services.AddDbContext<ContasAPagarContext>();
            services.AddScoped<DbContext, ContasAPagarContext>();

            services.AddDefaultIdentity<Usuario>().AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ContasAPagarContext>();
            services.AddIdentityCore<Colaborador>().AddRoles<IdentityRole>().AddEntityFrameworkStores<ContasAPagarContext>();
            services.AddIdentityServer().AddApiAuthorization<Usuario, ContasAPagarContext>();

            services.AddAuthentication().AddIdentityServerJwt();

            services.Configure<JwtBearerOptions>(IdentityServerJwtConstants.IdentityServerJwtBearerScheme, options =>
            {
                var onTokenValidated = options.Events.OnTokenValidated;
                options.Events.OnTokenValidated = async context =>
                {
                    await onTokenValidated(context);
                };
            });
            services.AddMvc().AddNewtonsoftJson(options=>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddRazorPages().AddNewtonsoftJson();
            services.Configure<Config.ConfiguracoesDB>(Configuration.GetSection("ConfiguracoesDB"));
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<Usuario> usuarioManager, UserManager<Colaborador> colaboradorManager, IServiceProvider serviceProvider, ContasAPagarContext contasAPagarContext)
        {
            ApplicationDBInitializer.SeedDatabase(usuarioManager, colaboradorManager, contasAPagarContext, serviceProvider);
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
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

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
        // To learn more about options for serving an Angular SPA from ASP.NET Core,
        // see https://go.microsoft.com/fwlink/?linkid=864501

        spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                    //spa.UseProxyToSpaDevelopmentServer("Http://localhost:4200/");
                }
            });
        }
    }   
}
