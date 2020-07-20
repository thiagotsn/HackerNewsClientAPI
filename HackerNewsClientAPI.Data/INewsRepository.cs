using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HackerNewsClientAPI.Domain;

namespace HackerNewsClientAPI.Data
{
    public interface INewsRepository 
    {
        Task<IEnumerable<News>> GetNews(int newsCount);
    }
}
