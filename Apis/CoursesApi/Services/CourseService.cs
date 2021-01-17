using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CoursesApi.Entities;
using MongoDB.Driver;

namespace CoursesApi.Services
{
    public interface ICourseService
    {
        Task<CourseEntity> CreateAsync(CourseEntity courseIn);
        Task DeleteAsync(string id);
        Task<List<CourseEntity>> GetAsync();
        Task<CourseEntity> GetAsync(string id);
        Task UpdateAsync(string id, CourseEntity courseIn);
    }

    public class CourseService : ICourseService
    {
        private readonly IMongoCollection<CourseEntity> _entities;

        public CourseService(ICoursesDatabaseSettings dbSetting)
        {
            var client = new MongoClient(dbSetting.ConnectionString);
            var database = client.GetDatabase(dbSetting.DatabaseName);
            _entities = database.GetCollection<CourseEntity>(dbSetting.CourseCollectionName);
        }

        public async Task<List<CourseEntity>> GetAsync() => (await _entities.FindAsync<CourseEntity>(course => true)).ToList();

        public async Task<CourseEntity> GetAsync(string id)
        {

            return (await _entities.FindAsync<CourseEntity>(course => course.Id == id)).FirstOrDefault();
        }

        public async Task<CourseEntity> CreateAsync(CourseEntity courseIn)
        {
            await _entities.InsertOneAsync(courseIn);
            return courseIn;
        }


        public async Task UpdateAsync(string id, CourseEntity courseIn)
        {
            await _entities.ReplaceOneAsync(course => course.Id == id, courseIn);
        }

        public async Task DeleteAsync(string id)
        {
            await _entities.DeleteOneAsync(course => course.Id == id);
        }



    }
}