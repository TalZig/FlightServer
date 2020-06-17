using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FlightServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            /*string Ip = configuration.GetSection("Host").GetSection("Ip").Value;
            string Port = configuration.GetSection("Host").GetSection("Port").Value;
            string HttpAddress = configuration.GetSection("Host").GetSection("HttpAddress").Value;
            Models.Host data = new Models.Host(Ip, Port, HttpAddress);*/
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DataOfServer>(Configuration.GetSection("DataOfServer"));
            services.AddOptions();
            services.AddControllers();
            services.AddSingleton<ITCPClient, ClientTcp>();
            services.AddSingleton<FlightGearClient, FlightGearClient>();
            services.AddSingleton<Screenshot, Screenshot>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            //Server server = new Server();
            //server.Start();
        }
    }
}