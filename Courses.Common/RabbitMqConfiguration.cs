namespace Courses.Common
{
    public interface IRabbitMqConfiguration
    {
        string Url { get; set; }
        string Exchange { get; set; }
        string RoutingKey { get; set; }
        string QueueName { get; set; }
    }

    public class RabbitMqConfiguration : IRabbitMqConfiguration
    {
        public string Url { get; set; }
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
        public string QueueName { get; set; }
    }
}