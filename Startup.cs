﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneTestApi.Models;
using OneTestApi.Services;
using OneTestApi.Utils;

namespace OneTestApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(
                    options => options.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            services.AddDbContext<OneTestDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("mysql")));

            services.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<TestCaseExecutionType, string>().ConvertUsing(s => s.ToString().ToLower());
                cfg.CreateMap<TestCaseImportance, string>().ConvertUsing(s => s.ToString().ToLower());
                cfg.CreateMap<string, TestCaseExecutionType>().ConvertUsing(s => s.ToEnum<TestCaseExecutionType>());
                cfg.CreateMap<string, TestCaseImportance>().ConvertUsing(s => s.ToEnum<TestCaseImportance>())
            });

            services.AddTransient<ITestProjectService, TestProjectService>();
            services.AddTransient<ITestSuiteService, TestSuiteService>();
            services.AddTransient<ITestCaseService, TestCaseService>();
            services.AddTransient<ITestCaseTagService, TestCaseTagService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseDeveloperExceptionPage();

            app.UseMvc();
        }
    }
}