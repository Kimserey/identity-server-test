﻿using System;
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
			        .AddInMemoryApiResources(Configs.GetApiResources())
					.AddInMemoryClients(Configs.GetClients())
			        .AddInMemoryIdentityResources(Configs.GetIdentityResources())
			        //.AddTestUsers(Configs.GetTestUsers())
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
