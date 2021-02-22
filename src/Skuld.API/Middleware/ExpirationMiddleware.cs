using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Skuld.API.Helpers;
using Skuld.Core.Extensions;
using Skuld.Core.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Skuld.API.Middleware
{
	public class ExpirationMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IConfiguration Configuration;
		private readonly string NewDomain;
		private readonly DateTime Expires;

		public ExpirationMiddleware(RequestDelegate next, IConfiguration config)
		{
			_next = next;
			Configuration = config;
			NewDomain = Configuration.GetValue<string>("currentDomainName");
			Expires = Configuration.GetValue<ulong>("oldDomainExpires").FromEpoch();
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (!context.GetUrlHostname().Contains(NewDomain) && DateTime.UtcNow > Expires)
			{
				context.Response.ContentType = "application/json";
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

				await context.Response.WriteAsync(EventResult.FromFailure($"You have called the legacy domain, please upgrade your requests to https://{NewDomain}").ToJson());

				return;
			}

			await _next(context);
		}
	}
}
