using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CoursesApi.Entities;
using CoursesApi.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using MongoDB.Bson.Serialization.Serializers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CoursesApi.BgWorkers
{
    public class CoursesQueueConsumer : BackgroundService
    {
        private readonly IRabbitMqConfiguration _rabbitMqConfig;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private readonly ICourseService _courseService;

        public string Url => _rabbitMqConfig.Url;
        public string Exchange => _rabbitMqConfig.Exchange;
        public string QueueName => _rabbitMqConfig.QueueName;
        public string RoutingKey => _rabbitMqConfig.RoutingKey;
        
        

        public CoursesQueueConsumer(IRabbitMqConfiguration rabbitMqConfig, ICourseService courseService)
        {
            _rabbitMqConfig = rabbitMqConfig;
            _courseService = courseService;
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
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (sender, e) =>
            {
                var msgBody = Encoding.UTF8.GetString(e.Body.ToArray());
                var jsonBody = JsonDocument.Parse(msgBody).RootElement;
                var studentName = "";
                var studentId = "";
                var courseId = "";
                
                if (jsonBody.TryGetProperty("StudentName", out var studentNameElement))
                {
                    studentName = studentNameElement.GetString();
                }

                if (jsonBody.TryGetProperty("StudentId", out var studentIdElement))
                {
                    studentId = studentIdElement.GetString();
                }

                if (jsonBody.TryGetProperty("CourseId", out var courseIdElement))
                {
                    courseId = courseIdElement.GetString();
                }

                var IsRegistrationSuccess = await _courseService.RegisterStudentForCourse(studentId, studentName, courseId);

                if (IsRegistrationSuccess)
                {
                    
                }
                else
                {
                    
                }
                
                
                Console.WriteLine($"Message received =>{msgBody}");
            };

            _channel.BasicConsume(QueueName, true, consumer);

            await Task.CompletedTask;
            
        }
        
    }
}