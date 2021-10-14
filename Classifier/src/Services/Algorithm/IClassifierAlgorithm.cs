using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Classifier.Models.DTO;

namespace Classifier.Services.Algorithm
{
    public interface IClassifierAlgorithm
    {
        List<string> getWords(string content);

        Task<Score> test(TestRequest entity);

        Task<bool> training(TrainingRequest entity);
    }
}