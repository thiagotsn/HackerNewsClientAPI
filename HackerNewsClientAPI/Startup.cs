using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HackerNewsClientAPI.Data;
using HackerNewsClientAPI.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HackerNewsClientAPI
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var _baseUrl = Configuration.GetSection("BaseUrl").Value;
            var _requestCountLimit = int.Parse(Configuration.GetSection("RequestCountLimit").Value);

            var httpClient = new HttpClient(){ BaseAddress = new Uri(_baseUrl) };
            services.AddSingleton(httpClient);

            var semaphore = new SemaphoreSlim(_requestCountLimit);
            services.AddSingleton(semaphore);

            services.AddSingleton<INewsRepository, HttpClientNewsRepository>();

            var configMapper = new AutoMapper.MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<News, NewsDto>()
                        .ForMember(dest => dest.Uri, opt => opt.MapFrom(src => src.Url))
                        .ForMember(dest => dest.PostedBy, opt => opt.MapFrom(src => src.By))
                        .ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Time).DateTime.ToLocalTime()))
                        .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Descendants));
                });
            var mapper = configMapper.CreateMapper();
            services.AddSingleton(mapper);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
