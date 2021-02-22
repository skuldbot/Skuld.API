using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Skuld.API.Models;
using Skuld.Core.Converters;
using Skuld.Core.Models;
using Skuld.Models;
using System.Collections.Generic;
using System.Net;

namespace Skuld.API.Helpers
{
	public static class ResponseHelper
	{
		static readonly JsonSerializerSettings SerializerOptions = new()
		{
			Formatting = Formatting.Indented,
			DefaultValueHandling = DefaultValueHandling.Ignore,
			NullValueHandling = NullValueHandling.Ignore,
			Converters = new List<JsonConverter>
			{
				new LongToStringConverter()
			}
		};

		public static object Send(this HttpResponse Response, EventResult result, HttpStatusCode status = HttpStatusCode.OK)
		{
			Response.ContentType = "application/json";
			Response.StatusCode = (int)status;
			return new NewtonsoftJsonResult(result, SerializerOptions);
		}
		public static object Send<T>(this HttpResponse Response, EventResult<T> result, HttpStatusCode status = HttpStatusCode.OK)
		{
			Response.ContentType = "application/json";
			Response.StatusCode = (int)status;

			var config = Response.HttpContext.RequestServices.GetService<IConfiguration>();

			string newDomain = config.GetValue<string>("currentDomainName");
			string currentDomain = Response.HttpContext.GetUrlHostname();

			if (config != null && !currentDomain.Contains(newDomain))
			{
				return new NewtonsoftJsonResult(result.WithDomainWarning(newDomain), SerializerOptions);
			}

			return new NewtonsoftJsonResult(result, SerializerOptions);
		}

		public static ResponseEventResult<T> WithDomainWarning<T>(this EventResult<T> eventResult, string newDomain)
			=> new ResponseEventResult<T>()
				.WithWarning($"You are currently using the legacy domain name; Please update your API calls to 'https://{newDomain}'")
				.WithData(eventResult.Data)
				.WithError(eventResult.Error)
				.WithException(eventResult.Exception);

		public static User GetUnAuthedUser(User fullUser)
			=> new() { Id = fullUser.Id, Language = null, TimeZone = null, Background = null };

		public static Guild GetUnAuthedGuild(Guild fullGuild)
			=> new() { Id = fullGuild.Id };
	}
}
