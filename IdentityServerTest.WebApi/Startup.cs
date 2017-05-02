using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace IdentityServerTest.WebApi
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
            // See https://leastprivilege.com/2015/12/28/validating-scopes-in-asp-net-4-and-5/ for scope and policy example
            // Setup a policy which check for the scope claim and allow access.
            services.AddAuthorization(options =>
            {
                options.AddPolicy("api.call", policy => policy.RequireClaim("scope", "api.call"));
            });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                // Sets the authority with the api and secret to exchange reference token for identity.
                Authority = "http://localhost:5000",
                ApiName = "api",
                ApiSecret = "secret",
                AutomaticAuthenticate = true,
                RequireHttpsMetadata = false,
                AllowedScopes = { "api", "api.call" }
            });

            app.Map("/communication", mainApp =>
            {
                mainApp.Map("/scoped", scopedApp =>
                {
                    scopedApp.AllowScopes("api", "api.call");
                    scopedApp.Run(async context =>
                    {
                        await context.Response.WriteAsync("CALL access");
                    });
                });

                mainApp.AllowScopes("api");
                mainApp.Run(async context =>
                {
                    await context.Response.WriteAsync("API access");
                });
            });
        }
    }
}
