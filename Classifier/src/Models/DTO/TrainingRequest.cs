using System;

namespace Classifier.Models.DTO
{
    public class TrainingRequest
    {
        public string text { get; set; }
        public string topic { get; set; } 
    }
}