using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StudentApi.Entities
{
    public class StudentEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime DateOfBirth { get; set; }
        
        public List<RegisteredCourse> Courses { get; set; }

        public StudentEntity()
        {
            Courses = new List<RegisteredCourse>();
        }
    }
}