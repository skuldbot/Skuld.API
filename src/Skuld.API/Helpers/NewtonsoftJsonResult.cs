using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Skuld.API.Helpers
{
	public class NewtonsoftJsonResult : ActionResult, IStatusCodeActionResult, IActionResult
	{
		public string ContentType { get; set; }
		public int? StatusCode { get; set; }
		public object Value { get; set; }
		public JsonSerializerSettings SerializerSettings { get; set; }

		public NewtonsoftJsonResult(object data, JsonSerializerSettings settings)
		{
			Value = data;
			SerializerSettings = settings;
		}

		public override void ExecuteResult(ActionContext context)
			=> ExecuteResultAsync(context).ConfigureAwait(false);

		public override async Task ExecuteResultAsync(ActionContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			var response = context.HttpContext.Response;

			response.StatusCode = StatusCode ?? 200;

			response.ContentType = !string.IsNullOrEmpty(ContentType)
				? ContentType
				: "application/json";

			var serializedObject = JsonConvert.SerializeObject(Value, SerializerSettings);
			await response.WriteAsync(serializedObject);
		}
	}
}
