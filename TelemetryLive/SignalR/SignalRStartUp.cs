﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace TelemetryLive.SignalR
{
    class SignalRStartUp
    {
        public IConfiguration Configuration { get; }
        public SignalRStartUp(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<StatisticsHub>("/statisticsHub");
            });
        }
    }
}
