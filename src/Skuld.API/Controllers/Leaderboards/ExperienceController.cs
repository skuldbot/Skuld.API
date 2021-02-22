using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Skuld.API.Attributes;
using Skuld.API.Helpers;
using Skuld.API.Managers;
using Skuld.Core.Models;
using Skuld.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Skuld.API.Controllers
{
	[Route("experience")]
	[ApiController]
	public class ExperienceController : ControllerBase
	{
		private readonly SkuldDbContext Database;
		private readonly RequestManager Request;
		private readonly int PageAmount;

		public ExperienceController(
			SkuldDbContext database,
			RequestManager manager,
			IConfiguration configuration
		)
		{
			this.Database = database;
			this.Request = manager;
			PageAmount = configuration.GetValue<int>("pageAmount");
		}

		// GET api/leaderboard/1234/123
		[HttpGet("{guildId}/{page}")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		[RequireToken(true, true)]
		public async Task<object> GetExperienceAsync(ulong guildId, int page = 0)
		{
			if (guildId == 0)
			{
				return GetExperience(page);
			}

			var experienceLeaderboard = await Database.GetOrderedGuildExperienceLeaderboardAsync(guildId);

			if (experienceLeaderboard == null)
			{
				return Response.Send(EventResult.FromFailure($"Couldn't find guild with Id '{guildId}'"), System.Net.HttpStatusCode.NotFound);
			}

			if (experienceLeaderboard.Count == 0)
			{
				return Response.Send(EventResult.FromFailure($"Guild '{guildId}' hasn't opted into the experience module, or have just enabled it"), System.Net.HttpStatusCode.NotFound);
			}

			experienceLeaderboard = experienceLeaderboard.Skip(page * PageAmount).Take(PageAmount).ToList();

			return ProcessResult(experienceLeaderboard);
		}

		private object GetExperience(int page)
		{
			var experienceLeaderboard = Database.GetOrderedGlobalExperienceLeaderboard();

			if (experienceLeaderboard == null || experienceLeaderboard.Count == 0)
			{
				return Response.Send(EventResult.FromFailure("No one has opted into the experience module"), System.Net.HttpStatusCode.NotFound);
			}

			experienceLeaderboard = experienceLeaderboard.Skip(page * PageAmount).Take(PageAmount).ToList();

			return ProcessResult(experienceLeaderboard);
		}

		private object ProcessResult(IEnumerable<UserExperience> leaderboard)
		{
			if (!Request.IsValidAuthorization && Request.RequiresAuthorization)
			{
				return Response.Send(EventResult<List<UserExperience>>.FromSuccess(leaderboard.Select(entry => new UserExperience { UserId = entry.Id })));
			}

			if (Request.IsValidAuthorization && Request.RequiresAuthorization)
			{
				return Response.Send(EventResult<List<UserExperience>>.FromSuccess(leaderboard));
			}

			return Response.Send(EventResult.FromFailure("Can't parse result"), System.Net.HttpStatusCode.InternalServerError);
		}
	}
}
