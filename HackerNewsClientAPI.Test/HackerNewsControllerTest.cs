using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using HackerNewsClientAPI.Controllers;
using HackerNewsClientAPI.Data;
using HackerNewsClientAPI.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HackerNewsClientAPI.Test
{
    public class HackerNewsControllerTest
    {
        [Fact]
        public async Task GetTop20NewsAsync_ShouldReturnAllNews()
        {
            // Arrange
            var logger = GetLogger();

            var mockRepo = new Mock<INewsRepository>();
            mockRepo.Setup(repo => repo
                           .GetNews(20))
                           .Returns(Task.FromResult(GetTestNews()));

            var mapper = GetMapper();

            var controller = new HackerNewsController(logger, mockRepo.Object, mapper);

            // Act
            var actionResponse = await controller.GetTop20NewsAsync();

            // Assert
            var actionResult = actionResponse.Should().BeOfType<OkObjectResult>().Subject;
            var newsResponse = actionResult.Value.Should().BeAssignableTo<List<NewsDto>>().Subject;

            newsResponse.Count.Should().Be(20);
        }

        private IEnumerable<News> GetTestNews()
        {
            var newsList = new List<News>();

            for (int i = 0; i < 20; i++)
            {
                newsList.Add(new News());
            }

            return newsList;
        }

        private static IMapper GetMapper()
        {
            var configMapper = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<News, NewsDto>()
                    .ForMember(dest => dest.Uri, opt => opt.MapFrom(src => src.Url))
                    .ForMember(dest => dest.PostedBy, opt => opt.MapFrom(src => src.By))
                    .ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Time).DateTime.ToLocalTime()))
                    .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Descendants));
            });
            var mapper = configMapper.CreateMapper();

            return mapper;
        }

        private static ILogger<HackerNewsController> GetLogger()
        {
            var serviceProvider = new ServiceCollection()
                                        .AddLogging(builder => {
                                            builder.AddDebug();
                                        })
                                        .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();

            return factory.CreateLogger<HackerNewsController>();
        }
    }
}
