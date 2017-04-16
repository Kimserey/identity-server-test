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
        public Startup(ILoggerFactory loggerFactory, IHostingEnvironment environment)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.LiterateConsole()
                .CreateLogger();

            loggerFactory.AddSerilog(logger);
        }

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
			loggerFactory.AddConsole();
			app.UseDeveloperExceptionPage();

			app.UseIdentityServer();
		}
	}
}
