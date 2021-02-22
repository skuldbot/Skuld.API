using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Skuld.API.Managers;
using Skuld.Core.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Skuld.API.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	public class RequireTokenAttribute : ActionFilterAttribute
	{
		public bool Required { get; set; }
		public bool LimitedData { get; set; }

		public RequireTokenAttribute(bool required, bool limitedData)
		{
			Required = required;
			LimitedData = limitedData;
		}

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var requestManager = context.HttpContext.RequestServices.GetRequiredService<RequestManager>();

			requestManager.RequiresAuthorization = Required;
			requestManager.CanReturnLimitedOnUnAuthorized = LimitedData;

			requestManager.IsValidAuthorization = await Helpers.RequestHelper.IsRequestAuthenticatedAsync(context.HttpContext.Request);

			if (Required && requestManager.IsValidAuthorization || LimitedData)
			{
				await next();
				return;
			}

			var invalidRequest = EventResult.FromFailure("Unauthorized request");
			context.HttpContext.Response.ContentType = "application/json";
			context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

			await context.HttpContext.Response.WriteAsync(invalidRequest.ToJson());

			context.Result = new StatusCodeResult((int)HttpStatusCode.Unauthorized);
			await next();
			return;
		}
	}
}
