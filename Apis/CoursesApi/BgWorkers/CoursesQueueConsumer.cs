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
using Courses.Common;

namespace CoursesApi.BgWorkers
{
    public class CoursesQueueConsumer : BaseQueueConsumerBackgroundService
    {
        private readonly ICourseService _courseService;
        
        public CoursesQueueConsumer(IRabbitMqConfiguration rabbitMqConfig, ICourseService courseService) : base(rabbitMqConfig)
        {
            _courseService = courseService;
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

        //         public override async Task ProcessMessageAsync(string jsonString)
//         {
//             await base.ProcessMessageAsync(jsonString);
//             
//             Console.WriteLine($"Message received =>{jsonString}");
//
//             /*var jsonBody = JsonDocument.Parse(jsonString).RootElement;
//             var studentName = "";
//             var studentId = "";
//             var courseId = "";
//                 
//             if (jsonBody.TryGetProperty("StudentName", out var studentNameElement))
//             {
//                 studentName = studentNameElement.GetString();
//             }
//
//             if (jsonBody.TryGetProperty("StudentId", out var studentIdElement))
//             {
//                 studentId = studentIdElement.GetString();
//             }
//
//             if (jsonBody.TryGetProperty("CourseId", out var courseIdElement))
//             {
//                 courseId = courseIdElement.GetString();
//             }
//
//             var IsRegistrationSuccess = await _courseService.RegisterStudentForCourse(studentId, studentName, courseId);
//
//             if (IsRegistrationSuccess)
//             {
//                     
//             }
//             else
//             {
//                     
//             }*/
//
//             
//         }

        

        
    }
}