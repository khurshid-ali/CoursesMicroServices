using Courses.Common;

namespace StudentApi.Services
{
    
    public interface IQueuePublisherService
    {
        void PublishMessage(string routingKey, string msg);
    }
    public class QueuePublisherService : BaseQueuePublisher, IQueuePublisherService
    {
        public QueuePublisherService(IRabbitMqConfiguration rabbitConf) : base(rabbitConf)
        {
        }

        public override void PublishMessage(string routingKey, string msg)
        {
            base.PublishMessage(routingKey, msg);
        }
    }
}