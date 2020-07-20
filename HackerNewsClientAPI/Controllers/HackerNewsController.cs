using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HackerNewsClientAPI.Data;
using HackerNewsClientAPI.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HackerNewsClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HackerNewsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly INewsRepository _newsRepository;
        private readonly IMapper _mapper;

        public HackerNewsController(ILogger<HackerNewsController> logger, INewsRepository newsRepository, IMapper mapper)
        {
            _logger = logger;
            _newsRepository = newsRepository;
            _mapper = mapper;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> GetTop20NewsAsync()
        {
            _logger.LogInformation("Request Started");
            var newsFromRepo = await _newsRepository.GetNews(20);
            _logger.LogInformation("Request Completed");

            var news = _mapper.Map<IEnumerable<NewsDto>>(newsFromRepo);
            return Ok(news);
        }
    }
}
