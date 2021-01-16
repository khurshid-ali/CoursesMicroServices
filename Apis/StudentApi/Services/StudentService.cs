using System;
using System.Collections.Generic;
using MongoDB.Driver;
using StudentApi.Entities;

namespace StudentApi.Services
{
    public interface IStudentService
    {
        StudentEntity Create(StudentEntity student);
        List<StudentEntity> Get();
        StudentEntity Get(string id);
        void Remove(StudentEntity studentIn);
        void Remove(string id);
        void Update(string id, StudentEntity studentIn);
    }

    public class StudentService : IStudentService
    {
        private readonly IMongoCollection<StudentEntity> _students;

        public StudentService(IStudentDatabaseSettings dbSettings)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            var database = client.GetDatabase(dbSettings.DatabaseName);
            _students = database.GetCollection<StudentEntity>(dbSettings.StudentCollectionName);
        }

        public List<StudentEntity> Get() =>
            _students.Find(student => true).ToList();

        public StudentEntity Get(string id) =>
            _students.Find<StudentEntity>(student => student.Id == id).FirstOrDefault();

        public StudentEntity Create(StudentEntity student)
        {
            _students.InsertOne(student);
            return student;
        }

        public void Update(string id, StudentEntity studentIn) =>
            _students.ReplaceOne(student => student.Id == id, studentIn);

        public void Remove(StudentEntity studentIn) =>
            _students.DeleteOne(student => student.Id == studentIn.Id);

        public void Remove(string id) =>
            _students.DeleteOne(student => student.Id == id);


    }
}