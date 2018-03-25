﻿using System;
using Amba.ImageTools.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationBot.Infrastructure
{
    public class ApplicationBuilder<T> 
        where T : class, IConsoleApplication
    {
        public T Build()
        {
            ServiceProvider = ServiceCollection.BuildServiceProvider();
            var application = ServiceProvider.GetService<IConsoleApplication>() as T;
            return application;
        }

        private IConfiguration Configuration { get; set; }
        private IServiceCollection ServiceCollection { get; set; } = new ServiceCollection();
        public ServiceProvider ServiceProvider { get; private set; }

        public virtual ApplicationBuilder<T> ReadConfiguration()
        {
            var environmentName = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";            
            Configuration = new ConfigurationBuilder() 
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .Build();
            return this;
        }

        public virtual ApplicationBuilder<T> RegisterServices(params Action<IServiceCollection, IConfiguration>[] registers)            
        {
            if (Configuration != null)
                ServiceCollection.AddSingleton<IConfiguration>(Configuration);   
            if (registers != null)
            {
                foreach (var register in registers)
                {
                    register(ServiceCollection, Configuration);
                }
            }
            ServiceCollection.AddSingleton<IConsoleApplication, T>();
            ServiceProvider = ServiceCollection.BuildServiceProvider();
            return this;
        }
         
    }
}