using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoConsumerLibary.KafkaConsumer;
using TelemetryLive.SignalR;

namespace TelemetryLive
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            KafkaSettings kafkaSettings =  Configuration.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>();
            services.AddSingleton(kafkaSettings);
            services.AddSingleton<KafkaConnection>();
            services.AddSingleton<WebSocketService>();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:4200") // Adjust for your frontend URL
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseCors();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<StatisticsHub>(Consts.SGINALR_URL_NAME);

            });
        }
    }
}
