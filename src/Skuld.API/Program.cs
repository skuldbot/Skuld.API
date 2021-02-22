using dotenv.net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Skuld.Core;
using System.IO;
using System.Threading.Tasks;

namespace Skuld.API
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			DotEnv.Config(filePath: Path.Combine(SkuldAppContext.BaseDirectory, ".env"));

			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
