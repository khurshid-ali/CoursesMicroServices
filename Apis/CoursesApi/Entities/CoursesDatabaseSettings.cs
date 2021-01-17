namespace CoursesApi.Entities
{
    public interface ICoursesDatabaseSettings
    {
        string CourseCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }

    public class CoursesDatabaseSettings : ICoursesDatabaseSettings
    {
        public string CourseCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

    }
}