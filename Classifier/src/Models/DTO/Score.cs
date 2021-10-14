using System;
using System.Linq;

namespace Classifier.Models.DTO
{
    public class Score : IComparable<Score>
    {
        public double value { get; set; }
        public string topic { get; set; }

        int IComparable<Score>.CompareTo(Score other)
        {
            if (other.value > this.value)
                return -1;
            else if (other.value == this.value)
                return 0;
            else
                return 1;
        }
    }
}