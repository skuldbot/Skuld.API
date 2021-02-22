using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Skuld.Core.Extensions;
using Skuld.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Skuld.API.Helpers
{
	public static class RequestHelper
	{
		public static string GetUrlHostname(this HttpContext context)
		{
			var origin = context.Request.Headers["X-Original-Host"];

			return origin.Count > 0 ? origin[0] : context.Request.Host.Host;
		}

		public static async Task<bool> IsRequestAuthenticatedAsync(this HttpRequest request)
		{
			return await IsValidKeyAsync(request);
		}

		static async Task<bool> IsValidKeyAsync(HttpRequest request)
		{
			if (!request.Headers.TryGetValue("Authorization", out StringValues authKey))
			{
				return false;
			}

			if (!authKey.ToArray().AnyStartWith("Bearer", out string key))
			{
				return false;
			}

			using var Database = new SkuldAPIDbContextFactory().CreateDbContext();

			key = key.Replace("Bearer ", "");

			var tokenEntry = Database.Tokens.FirstOrDefault(token => token.Token.Equals(key));

			if (tokenEntry == null || !tokenEntry.IsValid)
			{
				return false;
			}

			tokenEntry.TotalAPICalls += 1;

			await Database.SaveChangesAsync();

			return true;
		}
	}
}
