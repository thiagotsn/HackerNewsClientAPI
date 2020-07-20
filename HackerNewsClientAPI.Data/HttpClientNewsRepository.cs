using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HackerNewsClientAPI.Domain;
using Microsoft.Extensions.Logging;

namespace HackerNewsClientAPI.Data
{
    public class HttpClientNewsRepository : INewsRepository
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly SemaphoreSlim _semaphore;

        public HttpClientNewsRepository(ILogger<HttpClientNewsRepository> logger,
            HttpClient httpClient,
            SemaphoreSlim semaphore)
        {
            _logger = logger;
            _httpClient = httpClient;
            _semaphore = semaphore;
        }

        public async Task<IEnumerable<News>> GetNews(int newsCount)
        {
            await _semaphore.WaitAsync();
            try
            {
                _logger.LogInformation("Call Started");

                var bestNewsIds = await GetNewsIds();
                var tasks = bestNewsIds.Select(id => GetNewsById(id));
                var bestNews = await Task.WhenAll(tasks);

                _logger.LogInformation("Call Ended");

                return bestNews.OrderByDescending(n => n.Score)
                    .Take(newsCount);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<IEnumerable<int>> GetNewsIds()
        {
            var responseBestNewsIds = await _httpClient.GetStringAsync($"{_httpClient.BaseAddress}/beststories.json");
            var bestNewsIds = JsonSerializer.Deserialize<IEnumerable<int>>(responseBestNewsIds);

            return bestNewsIds;
        }

        private async Task<News> GetNewsById(int id)
        {
            var responseNews = await _httpClient.GetStringAsync($"{_httpClient.BaseAddress}/item/{id}.json");
            var news = JsonSerializer.Deserialize<News>(responseNews);

            return news;
        }
    }
}
