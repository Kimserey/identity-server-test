﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServerTest.WebSite
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

			app.UseCookieAuthentication(new CookieAuthenticationOptions {
				AuthenticationScheme = "Cookies"
			});

            // Turn off the JWT claim type mapping to allow well-known claims (e.g. ‘sub’ and ‘idp’) 
            // to flow through unmolested.
            // Otherwise claim names are changed. (e.g 'sub' becomes 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier')
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

			app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions {
				SignInScheme = "Cookies",
				AuthenticationScheme = "odic",
				Authority = "http://localhost:5000",
				RequireHttpsMetadata = false,
				SaveTokens = true,
				ClientId = "website_2",
                GetClaimsFromUserInfoEndpoint = true
			});

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
