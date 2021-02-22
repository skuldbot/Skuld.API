using Microsoft.AspNetCore.Mvc;
using Skuld.API.Attributes;
using Skuld.API.Helpers;
using Skuld.API.Managers;
using Skuld.Core.Models;
using Skuld.Models;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Skuld.API.Controllers
{
	[Route("/guild")]
	[ApiController]
	public class GuildController : ControllerBase
	{
		private readonly SkuldDbContext Database;
		private readonly RequestManager Request;

		public GuildController(
			SkuldDbContext database,
			RequestManager manager
		)
		{
			this.Database = database;
			this.Request = manager;
		}

		// GET api/guild/5
		[HttpGet("{id}")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		[RequireToken(true, true)]
		public async Task<object> GetAsync(ulong id)
		{
			try
			{
				var guild = await Database.Guilds.FindAsync(id);

				if (guild == null)
				{
					return Response.Send(EventResult.FromFailure($"Couldn't find guild with Id '{id}'"), System.Net.HttpStatusCode.NotFound);
				}

				if (Request.IsValidAuthorization && Request.RequiresAuthorization)
				{
					return Response.Send(EventResult<Guild>.FromSuccess(guild));
				}

				if (!Request.IsValidAuthorization && Request.RequiresAuthorization)
				{
					return Response.Send(EventResult<Guild>.FromSuccess(ResponseHelper.GetUnAuthedGuild(guild)));
				}

				return Response.Send(EventResult.FromFailure("Can't parse result"), System.Net.HttpStatusCode.InternalServerError);
			}
			catch (ArgumentException)
			{
				return Response.Send(EventResult.FromFailure($"Couldn't find guild with Id '{id}'"), System.Net.HttpStatusCode.NotFound);
			}
		}
	}
}
