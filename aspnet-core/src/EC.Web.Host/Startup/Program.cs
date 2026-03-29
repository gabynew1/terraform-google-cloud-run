using Abp.AspNetCore.Dependency;
using Abp.Dependency;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace EC.Web.Host.Startup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        internal static IHostBuilder CreateHostBuilder(string[] args)
        {
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            // Cloud Run injects $PORT; fall back to 8080 for local/Docker usage
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            var url = $"http://0.0.0.0:{port}";

            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(url);
                })
                .UseCastleWindsor(IocManager.Instance.IocContainer);
        }
    }
}
