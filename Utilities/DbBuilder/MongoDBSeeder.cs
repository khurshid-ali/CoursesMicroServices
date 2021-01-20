using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using StudentApi.Entities;
using CoursesApi.Entities;

namespace DbBuilder
{
    public class MongoDBSeeder
    {
        private readonly MongoClient _client;
        public MongoDBSeeder(string connectionString)
        {
            _client = new MongoClient(connectionString);

        }

        public void SeedDb()
        {
            SeedStudents();
            SeedCourses();
        }


        public void SeedStudents()
        {
            Console.Write("Seeding Students......................");
            var db = _client.GetDatabase("Microservice_Students");
            var studentCol = db.GetCollection<StudentEntity>("Students");
            studentCol.DeleteMany(student => true);
            studentCol.InsertMany(new List<StudentEntity>{
                new StudentEntity
                {
                    FirstName = "John",
                    MiddleName = "M",
                    LastName = "Doe",
                    DateOfBirth = GetDate("1979/03/06")
                },
                new StudentEntity
                {
                    FirstName = "Jane",
                    MiddleName = "F",
                    LastName = "Doe",
                    DateOfBirth = GetDate("1983/06/12")
                },
                new StudentEntity
                {
                    FirstName = "Amy",
                    MiddleName = "F",
                    LastName = "Cooper",
                    DateOfBirth = GetDate("1997/06/12")
                },
                new StudentEntity
                {
                    FirstName = "Tom",
                    MiddleName = "M",
                    LastName = "Adam",
                    DateOfBirth = GetDate("1963/06/12")
                }
            });
            Console.WriteLine("[OK]");
        }

        public void SeedCourses()
        {
            Console.Write("Seeding Courses.......................");
            var db = _client.GetDatabase("Microservice_Courses");
            var courses = db.GetCollection<CourseEntity>("Courses");
            courses.DeleteMany(courses => true);
            courses.InsertMany(new List<CourseEntity>
            {
                new CourseEntity
                {
                    Name = "Java I",
                    Code = "CS101",
                    Capacity = 2,
                    Registered = 0
                },
                new CourseEntity
                {
                    Name = "Java II",
                    Code = "CS102",
                    Capacity = 2,
                    Registered = 0
                }
            });
            Console.WriteLine("[OK]");
        }

        private DateTime GetDate(string date)
        {
            return DateTime.ParseExact(date, "yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}