using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServerTest.Identity
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
			services.AddIdentityServer()
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
