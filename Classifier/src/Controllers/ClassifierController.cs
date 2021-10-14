using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Classifier.Services;
using Classifier.Models.DTO;

namespace Classifier.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClassifierController : ControllerBase
    {
        private readonly ClassifierService _srv;
        private readonly ILogger<ClassifierController> _logger;
        private readonly string _homeUrl;
        public ClassifierController(ILogger<ClassifierController> logger, ClassifierService srv)
        {
            _srv = srv;
            _logger = logger;
            _homeUrl = "/swagger";
        }

        /// <summary>
        /// Documentation
        /// </summary>
        [Route("/")]
        [HttpGet]
        public IActionResult Get()
        {
            return LocalRedirect(_homeUrl);
        }

        /// <summary>
        /// Training system
        /// </summary>
        /// <param name="entity">a request object to training the system</param>
        [Route("/api/training/document")]
        [HttpPost]
        public async Task<IActionResult> Training(TrainingRequest entity)
        {
            try
            {
                var result = await _srv.Training(entity);
                return result ? Ok() : StatusCode(500, "Error");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Processing request from {ex}");
                return StatusCode(500, $"Internal server error");
            }
        }

        /// <summary>
        /// Classify documents
        /// </summary>
        /// <param name="entity">a test request object to classify a document</param>
        [Route("/api/test/document")]
        [HttpPost]
        public async Task<IActionResult> Test(TestRequest entity)
        {
            try
            {
                return Ok(await _srv.Test(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Processing request from {ex}");
                return StatusCode(500, $"Internal server error");
            }
        }
    }
}
