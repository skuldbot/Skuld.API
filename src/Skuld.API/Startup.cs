using Discord.Rest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Skuld.API.Managers;
using Skuld.API.Middleware;
using Skuld.Models;
using System;

namespace Skuld.API
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
			services.AddMvc(mvcOptions =>
			{
				mvcOptions.EnableEndpointRouting = false;
			});

			services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.TryAddSingleton(new SkuldDbContextFactory().CreateDbContext());

			services.TryAddScoped<RequestManager>();

			string token = Environment.GetEnvironmentVariable("SKULDAPI_BOT_TOKEN");

			if (!string.IsNullOrEmpty(token))
			{
				var restClient = new DiscordRestClient();

				restClient.LoginAsync(Discord.TokenType.Bot, token);

				services.TryAddSingleton(restClient);
			}

			services.AddControllers();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMiddleware<ExpirationMiddleware>();

			app.UseMvc();

			app.UseRouting();

			app.UseAuthorization();

			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.All
			});

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
