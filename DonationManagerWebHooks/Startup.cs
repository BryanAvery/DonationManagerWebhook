using DonationManagerWebHooks.Logger;
using DonationManagerWebHooks.WordPressService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace DonationManagerWebHooks
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddTransient(_ => new MySqlDatabase(Configuration["MySqlDatabase"]));
            services.AddTransient<IDonationManagerService, DonationManagerService>();

            Log.Logger = new LoggerConfiguration()
                .Enrich.With(new LogEnricher())
                .WriteTo.Console()
                .WriteTo.Seq(Configuration["seq"])
                .CreateLogger();

            Log.Information("DonationManager Starting!", Configuration.GetSection("Logging"));

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            services.AddMvc()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problems = new CustomBadRequest(context);
                foreach (var error in problems?.Errors)
                {
                    Log.Warning(error.Value[0]);
                }
                return new BadRequestObjectResult(problems);
            };
        });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}