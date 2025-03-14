using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoConsumerLibary.KafkaConsumer;

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
            KafkaSettings kafkaSettings =  Configuration.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>();
            services.AddSingleton(kafkaSettings);
            services.AddSingleton<KafkaConnection>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
            });
        }
    }
}
