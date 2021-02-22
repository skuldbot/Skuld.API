using dotenv.net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Skuld.Core;
using System;
using System.IO;

namespace Skuld.Models
{
	public class SkuldAPIDbContextFactory : IDesignTimeDbContextFactory<SkuldAPIDbContext>
	{
		public SkuldAPIDbContext CreateDbContext(string[] args = null)
		{
			DotEnv.Config(filePath: Path.Combine(SkuldAppContext.BaseDirectory, ".env"));

			var connStr = Environment.GetEnvironmentVariable("SKULDAPI_CONNSTR");

			var optionsBuilder = new DbContextOptionsBuilder<SkuldAPIDbContext>();

			var serverVersion = ServerVersion.AutoDetect(connStr);

			optionsBuilder.UseMySql(
				connStr,
				serverVersion,
				x =>
				{
					x.EnableRetryOnFailure();
					x.CharSet(CharSet.Utf8Mb4);
				});

			return new SkuldAPIDbContext(optionsBuilder.Options);
		}
	}
}