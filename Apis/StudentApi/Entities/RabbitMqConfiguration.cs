namespace StudentApi.Entities
{
    public interface IRabbitMqConfiguration
    {
        string Exchange { get; set; }
        string Url { get; set; }
        string RoutingKey { get; set; }
        string QueueName { get; set; }
    }

    public class RabbitMqConfiguration : IRabbitMqConfiguration
    {
        public string Exchange { get; set; }
        public string Url { get; set; }
        public string RoutingKey { get; set; }
        public string QueueName { get; set; }
    }
}