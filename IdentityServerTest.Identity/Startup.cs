using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace IdentityServerTest.Identity
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddIdentityServer()
			        .AddInMemoryApiResources(Configs.GetApiResources())
					.AddInMemoryClients(Configs.GetClients())
			        .AddInMemoryIdentityResources(Configs.GetIdentityResources())
                    .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                    .AddProfileService<ProfileService>()
			        .AddTemporarySigningCredential();
		}

		public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
		{
            // Removed the default console logger, and added serilog.
            //
            // Libraries:
            // Serilog: Main library
            // Serilog.Extensions.Logging: Adds the extension to ".AddSerilog" to logger factory
            // Serilog.Sinks.Literate: Adds the Sink provider "LiterateConsole" which can be used with "WriteTo.XXX"
            //
            // Output template dictates the format of the printed logs.
            //
            loggerFactory.AddSerilog(
                new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.LiterateConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}{Exception}{NewLine}")
                    .CreateLogger()
            );

            app.UseDeveloperExceptionPage();
			app.UseIdentityServer();
		}
	}
}
