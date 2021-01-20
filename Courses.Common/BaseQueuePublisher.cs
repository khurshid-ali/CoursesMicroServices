using System;
using System.Text;
using RabbitMQ.Client;

namespace Courses.Common
{
    public abstract class BaseQueuePublisher
    {
        
        private readonly IRabbitMqConfiguration _rabbitMqConfig;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        
        public string Url => _rabbitMqConfig.Url;
        public string Exchange => _rabbitMqConfig.Exchange;
        public string QueueName => _rabbitMqConfig.QueueName;
        public string RoutingKey => _rabbitMqConfig.RoutingKey;
        public IModel Channel => _channel;

        public BaseQueuePublisher(IRabbitMqConfiguration rabbitConf)
        {
            _rabbitMqConfig = rabbitConf;
            
        }


        public virtual void PublishMessage(string routingKey, string msg)
        {
            _connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(Url)
            };

            var body = Encoding.UTF8.GetBytes(msg);

            using (var connection = _connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(Exchange, ExchangeType.Topic);
                    channel.BasicPublish(Exchange, routingKey, null, body);
                    ;
                }
            }
        }
    }
}