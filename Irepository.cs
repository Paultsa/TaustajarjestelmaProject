using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
namespace Project
{
    public interface IRepository
    {
        Task<Map> CreateMap(int size, string map);
    }
}