using Discord.Rest;
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
	[Route("money")]
	[ApiController]
	public class MoneyController : ControllerBase
	{
		private readonly SkuldDbContext Database;
		private readonly RequestManager Request;
		private readonly int PageAmount;
		private readonly DiscordRestClient discordClient;

		public MoneyController(
			SkuldDbContext database,
			RequestManager manager,
			IConfiguration configuration,
			DiscordRestClient discord
		)
		{
			this.Database = database;
			this.Request = manager;
			PageAmount = configuration.GetValue<int>("pageAmount");
			discordClient = discord;
		}

		// GET api/money/123/123
		[HttpGet("{guildId}/{page}")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		[RequireToken(true, true)]
		public async Task<object> GetMoneyAsync(ulong guildId, int page = 0)
		{
			if (guildId == 0) return GetMoney(page);

			return Response.Send(EventResult.FromFailure("Not Implemented"), System.Net.HttpStatusCode.NotImplemented);

			var moneyLeaderboard = await Database.GetOrderedGuildMoneyLeaderboardAsync(await discordClient.GetGuildAsync(guildId));

			if (moneyLeaderboard == null)
			{
				return Response.Send(EventResult.FromFailure($"Couldn't find guild with Id '{guildId}'"), System.Net.HttpStatusCode.NotFound);
			}

			moneyLeaderboard = moneyLeaderboard.Skip(page * PageAmount).Take(PageAmount).ToList();

			return ProcessResult(moneyLeaderboard);
		}

		private object GetMoney(int page)
		{
			var moneyLeaderboard = Database.GetOrderedGlobalMoneyLeaderboard();

			moneyLeaderboard = moneyLeaderboard.Skip(page * PageAmount).Take(PageAmount).ToList();

			return ProcessResult(moneyLeaderboard);
		}

		private object ProcessResult(IEnumerable<User> leaderboard)
		{
			if (!Request.IsValidAuthorization && Request.RequiresAuthorization)
			{
				return Response.Send(EventResult<List<User>>.FromSuccess(leaderboard.Select(ResponseHelper.GetUnAuthedUser)));
			}

			if (Request.IsValidAuthorization && Request.RequiresAuthorization)
			{
				return Response.Send(EventResult<List<User>>.FromSuccess(leaderboard));
			}

			return Response.Send(EventResult.FromFailure("Can't parse result"), System.Net.HttpStatusCode.InternalServerError);
		}
	}
}
