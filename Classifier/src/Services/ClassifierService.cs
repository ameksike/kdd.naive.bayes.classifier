using System;
using Classifier.Models.DTO;
using System.Threading.Tasks;
using Classifier.Services.Algorithm;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Classifier.Services
{
    public class ClassifierService
    {
        protected IClassifierAlgorithm Algorithm;
        public ClassifierService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            // load algorithm from configuration file: Classifier\appsettings.json
            var DefaultAlgorithm = configuration.GetValue<string>("Algorithm");
            switch (DefaultAlgorithm)
            {
                case "NaiveBayes":
                    Algorithm = serviceProvider.GetService<NaiveBayesAlgorithm>();
                    break;
            }
        }

        public async Task<bool> Training(TrainingRequest entity)
        {
            var result = await Algorithm.training(entity);
            return await Task.FromResult(result);
        }

        public async Task<TestResponse> Test(TestRequest entity)
        {
            var test = await Algorithm.test(entity);
            var result = test != null ? new TestResponse { topic = test.topic } : new TestResponse { topic = "undefined" };
            return await Task.FromResult(result);
        }

    }
}