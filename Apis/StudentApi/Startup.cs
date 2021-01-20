using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using StudentApi.BgWorkers;
using StudentApi.Entities;
using StudentApi.Services;
using StudentApi.JsonConverters;
using Courses.Common;

namespace StudentApi
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

            services.Configure<StudentDatabaseSettings>(
                Configuration.GetSection(nameof(StudentDatabaseSettings))
            );

            //students database settings is injected into the DI
            services.AddSingleton<IStudentDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<StudentDatabaseSettings>>().Value
            );

            services.Configure<RabbitMqConfiguration>(
                Configuration.GetSection(nameof(RabbitMqConfiguration))
            );
            services.AddSingleton<IRabbitMqConfiguration>(sp =>
                sp.GetRequiredService<IOptions<RabbitMqConfiguration>>().Value);

            services.AddSingleton<IStudentService, StudentService>();

            services.AddControllers()
                .AddJsonOptions(cfg =>
                    cfg.JsonSerializerOptions.Converters.Add(new DateTimeConverter())
                );
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StudentApi", Version = "v1" });
            });

            services.AddSingleton<IQueuePublisherService,QueuePublisherService>();

            services.AddHostedService<StudentQueueConsumer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StudentApi v1"));
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
