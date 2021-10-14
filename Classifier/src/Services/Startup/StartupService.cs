using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Classifier.Services;
using Classifier.Services.Algorithm;

namespace Classifier.Services.Startup
{
    public static class StartupService
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration Configuration)
        {
            // register all required services 
            services.AddScoped<ClassifierService>();
            services.AddScoped<NaiveBayesAlgorithm>();
        }
    }
}
