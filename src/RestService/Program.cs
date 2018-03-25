using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace RestService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .UseUrls("http://localhost:5002/")
                          .ConfigureLogging((context, logging) => { logging.ClearProviders(); })
                          .UseStartup<Startup>()
                          .Build();
        }
    }
}
