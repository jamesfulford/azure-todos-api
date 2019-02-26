using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TodosAPI.Data;

namespace TodosAPI
{
    /// <summary>
    /// Program to run Todo API
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the main entry point for the ASP.NET Core Web API application
        /// </summary>
        /// <param name="args">CLI Arguments.</param>
        public static void Main(string[] args)
        {
            IWebHost host = BuildWebHost(args);
            
            // Seed the database
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    Context context = services.GetRequiredService<Context>();
                    Data.Initializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "*** ERROR *** An error occurred while seeding the database. *** ERROR ***");
                }
            }
            host.Run();
        }

        /// <summary>
        /// Builds a new WebHost to serve the API
        /// </summary>
        /// <param name="args">Args from command line.</param>
        /// <returns>A spun-up Todo API webserver host instance.</returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
