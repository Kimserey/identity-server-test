using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Filter;
using Serilog;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;

namespace IdentityServerTest.Identity
{
    public class Startup
	{
        private readonly IConfigurationRoot Configuration;

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Replace loggerFactory by filtered loggerFactory which skips messages from System and Microsoft libraries like:
            // [19:06:34 Debug] Microsoft.AspNetCore.Server.Kestrel: Connection id "0HL44N1HST45L" received FIN.
            //
            loggerFactory = loggerFactory.WithFilter(
                new FilterLoggerSettings {
                    { "IdentityServerTest", LogLevel.Debug },
                    { "System", LogLevel.Error },
                    { "Microsoft", LogLevel.Warning }
                }
            );

            // Removed the default console logger, and added serilog.
            //
            // Libraries:
            // Serilog: Main library
            // Serilog.Extensions.Logging: Adds the extension to ".AddSerilog" to logger factory
            // Serilog.Sinks.Literate: Adds the Sink provider "LiterateConsole" which can be used with "WriteTo.XXX"
            //
            // Output template dictates the format of the printed logs.
            //
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}: {Message}{Exception}{NewLine}")
                .CreateLogger();

            loggerFactory.AddSerilog(logger);

            // Configuration builder taking the appsettings
            // and parsing it into IConfigurationRoot.
            //
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
		{
            // Allows to inject IOptions<ArangoDBConfiguration> in classes.
            services.Configure<ArangoDBConfiguration>(Configuration.GetSection("ArangoDB"));

            services.AddTransient<ICryptography, Cryptography>();
            services.AddSingleton<IUserStore, UserStore>();

            // Binding services before adding Identity server is required
            // as Identity server uses "TryAdd" for default stores.
            services
                    //.AddSingleton<IPersistedGrantStore, ArangoPersistedGrantStore>()
                    .AddIdentityServer()
                    .AddInMemoryApiResources(Configs.GetApiResources())
					.AddInMemoryClients(Configs.GetClients())
			        .AddInMemoryIdentityResources(Configs.GetIdentityResources())
                    //.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                    .AddTestUsers(Configs.GetTestUsers())
                    .AddExtensionGrantValidator<CustomGrantValidator>()
                    .AddProfileService<ProfileService>()
			        .AddTemporarySigningCredential();
		}

		public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
		{
            app.UseDeveloperExceptionPage();
			app.UseIdentityServer();
		}
	}
}
