using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace CoursesApi.Entities
{
    public class CourseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

    }
}