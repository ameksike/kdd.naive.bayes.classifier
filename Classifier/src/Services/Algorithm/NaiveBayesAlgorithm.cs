using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Classifier.Models.DTO;
using Classifier.Models.DAO;
using Classifier.Models.ORM;

namespace Classifier.Services.Algorithm
{
    public class NaiveBayesAlgorithm : IClassifierAlgorithm
    {
        public char[] _delimiter;
        protected string _scheme;
        protected readonly AppDbContext _db;
        private readonly ILogger _logger;

        public NaiveBayesAlgorithm(AppDbContext context, ILogger<NaiveBayesAlgorithm> logger)
        {
            _delimiter = new char[] { ' ', ',', '.', ';', '!', '?', ':', '\t', '\n', '"' };
            _scheme = "_TOTAL_";
            _db = context;
            _logger = logger;
        }

        /// <summary>
        /// Get a list of Words from content
        /// </summary>
        /// <param name="content">content of topics</param>
        public List<string> getWords(string content)
        {
            return content.Split(_delimiter).Select(item => item.ToLower()).Distinct().ToList();
        }

        /// <summary>
        /// Run the training process base on TrainingRequest
        /// </summary>
        /// <param name="entity">a TrainingRequest instance</param>
        public async Task<bool> training(TrainingRequest entity)
        {
            try
            {
                var words = this.getWords(entity.text);
                foreach (var word in words)
                {
                    this.trackWord(word, entity.topic);
                }
                this.trackDoc(entity.topic);
                this.trackDoc(_scheme);
                await _db.SaveChangesAsync();
                return await Task.FromResult(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return await Task.FromResult(false);
            }
        }

        /// <summary>
        /// track each Word by topic
        /// </summary>
        /// <param name="topic">name of topic</param>
        /// <param name="word">word string</param>
        protected void trackWord(string word, string topic)
        {
            try
            {
                if (String.IsNullOrEmpty(word) || String.IsNullOrEmpty(topic))
                    return;

                var entity = _db.TopicWord.FirstOrDefault(item => item.topic == topic && item.word == word);
                if (entity != null)
                {
                    entity.count = entity.count + 1;
                    _db.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    _db.TopicWord.Add(new TopicWord { topic = topic, word = word, count = 1 });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// track each document by topic
        /// </summary>
        /// <param name="topic">name of topic</param>
        protected void trackDoc(string topic)
        {
            try
            {
                if (String.IsNullOrEmpty(topic))
                    return;

                var entity = _db.TopicDoc.FirstOrDefault(item => item.topic == topic);
                if (entity != null)
                {
                    entity.docs = entity.docs + 1;
                    _db.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    _db.TopicDoc.Add(new TopicDoc { topic = topic, docs = 1 });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Classify documents by content
        /// </summary>
        /// <param name="entity">a TestRequest instance</param>
        public async Task<Score> test(TestRequest entity)
        {
            if (entity == null)
                return null;

            if (String.IsNullOrEmpty(entity.text))
                return null;

            var scores = new List<Score>();
            var list = await _db.TopicDoc.ToListAsync();
            var total = this.getTopicsTotal(list);
            var topics = this.getTopics(list);

            foreach (var item in topics)
            {
                if (String.IsNullOrEmpty(item.topic) == false && item.docs > 0 && total > 0)
                {
                    var score = this.classify(item.topic, item.docs, total, entity.text);
                    scores.Add(score);
                }
            }

            Score best = scores.Max();
            if (best != null)
            {
                _logger.LogInformation($" ** TOPIC: {best.topic} [{best.value}] ");
            }
            else
            {
                _logger.LogWarning($" ** there is no data to display, you must train your model to execute this operation ");
            }

            return best;
        }

        /// <summary>
        /// Get Score for classify documents by content
        /// </summary>
        /// <param name="name">The name of topic</param>
        /// <param name="count">Count of documents by topic</param>
        /// <param name="total">Total of documents</param>
        /// <param name="content">A document content</param>
        protected Score classify(string name, int count, int total, string content)
        {
            var words = this.getWords(content);
            var list = _db.TopicWord.Where(row => words.Contains(row.word) && row.topic == name).ToList();

            double scoreTopic = (double)count / total;
            double scoreWord = list.Sum(row => (double)row.count / count);
            double scoreTotal = scoreTopic + scoreWord;

            /*
                Note: When we use the multiplication of the probabilities, we obtain very small decimal numbers 
                      less than 0 and when treating them in a double data type they are rounded to 0 or comparison 
                      errors are introduced, similar happens when adding the logarithm of the probabilities 
                      for a small training set, for this type of problem it's solved much simpler with a sum. 

                double scoreWord = list.Aggregate((double) 1, (value, row) => (double) value * ((double) row.count / count));
                double scoreTotal = (double) scoreTopic * (double) scoreWord;
            */

            _logger.LogInformation($" -- NAME: {name} [{scoreTotal}]  >>  COUNT: {count} | DOCs: {total} | WORDs: {list.Count()} | sum(p(w_i|t)): {scoreWord} | p(t): {scoreTopic} ");
            return new Score { topic = name, value = scoreTotal };
        }

        /// <summary>
        /// Get a list of Topic without general _scheme
        /// </summary>
        /// <param name="list">a list of topics</param>
        protected List<TopicDoc> getTopics(List<TopicDoc> list)
        {
            return list.Where(item => item.topic != _scheme).ToList();
        }

        /// <summary>
        /// Get an _scheme Topic  
        /// </summary>
        /// <param name="list">a list of topics</param>
        protected int getTopicsTotal(List<TopicDoc> list)
        {
            var entity = list.Find(item => item.topic == _scheme);
            return entity == null ? 0 : entity.docs;
        }
    }
}
