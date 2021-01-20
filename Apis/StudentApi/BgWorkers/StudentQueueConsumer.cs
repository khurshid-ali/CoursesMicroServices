using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StudentApi.Entities;
using Courses.Common;

namespace StudentApi.BgWorkers
{
    public class StudentQueueConsumer : BaseQueueConsumerBackgroundService
    {
        
        
        public StudentQueueConsumer(IRabbitMqConfiguration rabbitConfig):base(rabbitConfig)
        {
           
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += (sender, e) =>
            {
                Console.WriteLine("message received");
            };

            Channel.BasicConsume(QueueName, true, consumer);

            await Task.CompletedTask;
        }
    }
}