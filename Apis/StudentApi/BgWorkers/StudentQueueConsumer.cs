using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StudentApi.Entities;

namespace StudentApi.BgWorkers
{
    public class StudentQueueConsumer : BackgroundService
    {
        
        private readonly IRabbitMqConfiguration _rabbitMqConfig;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;

        public string Url => _rabbitMqConfig.Url;
        public string Exchange => _rabbitMqConfig.Exchange;
        public string QueueName => _rabbitMqConfig.QueueName;
        public string RoutingKey => _rabbitMqConfig.RoutingKey;
        
        public StudentQueueConsumer(IRabbitMqConfiguration rabbitConfig)
        {
            _rabbitMqConfig = rabbitConfig;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(Url)
            };
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(Exchange, ExchangeType.Topic );
            _channel.QueueDeclare(QueueName,
                true,
                false,
                false,
                null);
            
            _channel.QueueBind(QueueName, Exchange, RoutingKey);
            
            return base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            _connection.Close();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, e) =>
            {
                var msgBody = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine($"Message received =>{msgBody}");
            };

            _channel.BasicConsume(QueueName, true, consumer);

            await Task.CompletedTask;
        }
    }
}