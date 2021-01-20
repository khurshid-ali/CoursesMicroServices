using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Courses.Common
{
    public abstract class BaseQueueConsumerBackgroundService : BackgroundService
    {
        
        private readonly IRabbitMqConfiguration _rabbitMqConfig;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private AsyncEventingBasicConsumer _consumer;

        public string Url => _rabbitMqConfig.Url;
        public string Exchange => _rabbitMqConfig.Exchange;
        public string QueueName => _rabbitMqConfig.QueueName;
        public string RoutingKey => _rabbitMqConfig.RoutingKey;

        public IModel Channel => _channel;
        
        

        public BaseQueueConsumerBackgroundService(IRabbitMqConfiguration rabbitMqConfig)
        {
            _rabbitMqConfig = rabbitMqConfig;
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
            _channel.BasicQos(0, 10, false);

            return base.StartAsync(cancellationToken);
        }
        
        
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            
            await base.StopAsync(cancellationToken);
            
            _connection.Close();

        }
        
        
        public void PublishMessage(string publishRoutingKey, string msgJson)
        {
            var message = Encoding.UTF8.GetBytes(msgJson);
            Channel.BasicPublish(Exchange, publishRoutingKey, null, message );
        }
        
        // protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        // {
        //     var consumer = new AsyncEventingBasicConsumer(_channel);
        //     consumer.Received += async (sender, e) =>
        //     {
        //         var content = Encoding.UTF8.GetString(e.Body.ToArray());
        //         Console.WriteLine("Base message received");
        //         await ProcessMessageAsync(content);
        //     };
        //
        //     _channel.BasicConsume(QueueName, true, consumer);
        //     
        //     await Task.CompletedTask;
        // }
        //
        // public virtual async Task ProcessMessageAsync(string jsonString)
        // {
        //     Console.WriteLine(jsonString);
        // }
        
        
        
        
    }
}