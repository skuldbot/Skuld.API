using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Skuld.API.Models;
using Skuld.Core.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Skuld.Models
{
	public class SkuldAPIDbContext : DbContext
	{
		public SkuldAPIDbContext(DbContextOptions<SkuldAPIDbContext> options)
			: base(options)
		{
		}

		public DbSet<APIToken> Tokens { get; set; }

		public bool IsConnected
		{
			get
			{
				return Database.CanConnect();
			}
		}

		public async Task ApplyPendingMigrations()
		{
			Database.EnsureCreated();

			bool hasMigrated = false;

			var pendingMigrations = Database.GetPendingMigrations().ToList();

			if (pendingMigrations.Any())
			{
				var migrator = Database.GetService<IMigrator>();

				foreach (var targetMigration in pendingMigrations)
				{
					migrator.Migrate(targetMigration);
					Log.Info(
						"DatabaseContext",
						$"Successfully migrated to: {targetMigration}"
					);
					hasMigrated = true;
				}
			}

			if (hasMigrated)
			{
				var migrations = await Database
					.GetAppliedMigrationsAsync()
				.ConfigureAwait(false);

				Log.Info(
					"DatabaseContext",
					"Migrated successfully, latest applied: " +
					$"{migrations.LastOrDefault()}"
				);
			}
			else
			{
				Log.Info(
					"DatabaseContext",
					$"No Migrations applied, database is fully synced"
				);
			}
		}
	}
}