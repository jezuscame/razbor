using IntegrationTests.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace IntegrationTests
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public void Configure(IServiceProvider provider)
        {
        }

        public void ConfigureHost(IHostBuilder hostBuilder) =>
            hostBuilder.ConfigureAppConfiguration(lb => lb.AddJsonFile("appsettings.json", false, true));

        public void ConfigureServices(IServiceCollection services, HostBuilderContext context)
        {
            Configuration = context.Configuration;

            services.AddHttpClient("", client =>
            {
                client.BaseAddress = new Uri(Configuration.GetRequiredSection("ServerBaseAddress").Value!);
            });
            services.AddSingleton<TestManager>();
        }
    }
}
