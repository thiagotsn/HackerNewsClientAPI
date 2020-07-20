using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HackerNewsClientAPI.Data;
using HackerNewsClientAPI.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language;
using Moq.Protected;
using Xunit;

namespace HackerNewsClientAPI.Test
{
    public class HttpClientNewsRepositoryTest
    {
        [Fact]
        public async Task GetNews_ShouldReturnFilteredNewsByScoreDesc()
        {
            // Arrange
            var logger = GetLogger();
            var httpClient = GetHttpClient();
            var semaphore = new SemaphoreSlim(1);
            var repository = new HttpClientNewsRepository(logger, httpClient, semaphore);
            var numberOfNews = 2;

            // Act
            var response = await repository.GetNews(numberOfNews);

            // Assert
            response.Count().Should().Be(numberOfNews);
            response.First().Score.Should().Be(response.Max(n => n.Score));
        }

        private static ILogger<HttpClientNewsRepository> GetLogger()
        {
            var serviceProvider = new ServiceCollection()
                                        .AddLogging(builder => {
                                            builder.AddDebug();
                                        })
                                        .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();

            return factory.CreateLogger<HttpClientNewsRepository>();
        }


        private HttpClient GetHttpClient()
        {

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[23878508,23885684,23875692]", Encoding.UTF8, "application/json")
                })
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"by\":\"ismaildonmez\",\"descendants\":588,\"id\":21233041,\"kids\":[21233229,21233578],\"score\":975,\"time\":1570887781,\"title\":\"A uBlock Origin update was rejected from the Chrome Web Store\",\"type\":\"story\",\"url\":\"https://github.com/uBlockOrigin/uBlock-issues/issues/745\"}", Encoding.UTF8, "application/json")
                })
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"by\":\"ismaildonmez\",\"descendants\":588,\"id\":21233041,\"kids\":[21233229,21233578],\"score\":1757,\"time\":1570887781,\"title\":\"A uBlock Origin update was rejected from the Chrome Web Store\",\"type\":\"story\",\"url\":\"https://github.com/uBlockOrigin/uBlock-issues/issues/745\"}", Encoding.UTF8, "application/json")
                })
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"by\":\"ismaildonmez\",\"descendants\":588,\"id\":21233041,\"kids\":[21233229,21233578],\"score\":501,\"time\":1570887781,\"title\":\"A uBlock Origin update was rejected from the Chrome Web Store\",\"type\":\"story\",\"url\":\"https://github.com/uBlockOrigin/uBlock-issues/issues/745\"}", Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(@"https://hacker-news.firebaseio.com/v0/"),
            };

            return httpClient;
        }
    }
}
