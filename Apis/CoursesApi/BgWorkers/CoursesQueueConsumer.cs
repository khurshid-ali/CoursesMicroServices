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

        public CoursesQueueConsumer(IRabbitMqConfiguration rabbitMqConfig, ICourseService courseService) : base(
            rabbitMqConfig)
        {
            _courseService = courseService;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += async (sender, e) =>
            {
                Console.WriteLine("message received");
                var body = Encoding.UTF8.GetString(e.Body.ToArray());
                var receivedMsg = JsonSerializer.Deserialize<ServiceMessage>(body, new JsonSerializerOptions
                {
                    Converters = {new ServiceMessageJsonConverter()}
                });

                var courseId = receivedMsg.Parameters.ContainsKey("CourseId") ? receivedMsg.Parameters["CourseId"] : "";
                var studentId = receivedMsg.Parameters.ContainsKey("StudentId")
                    ? receivedMsg.Parameters["StudentId"]
                    : "";
                var studentName = receivedMsg.Parameters.ContainsKey("StudentName")
                    ? receivedMsg.Parameters["StudentName"]
                    : "";

                var course = await _courseService.GetAsync(courseId);
                var seatAvailable = (course.Capacity - course.Registered) > 0;
                var isRegistrationGood = false;
                var replyMessage = new ServiceMessage();
                replyMessage.Parameters.Add("Type", "RegistrationResponse");

                if (seatAvailable)
                {
                    isRegistrationGood =
                        await _courseService.RegisterStudentForCourse(studentId, studentName, courseId);

                    if (isRegistrationGood)
                    {
                        replyMessage.Parameters.Add("CourseId", course.Id);
                        replyMessage.Parameters.Add("CourseName", course.Name);
                        replyMessage.Parameters.Add("CourseCode", course.Code);
                        replyMessage.Parameters.Add("StudentId", studentId);
                    }
                    else
                    {
                        replyMessage.Parameters.Add("Error", "Registration Failed.");
                    }
                }
                else
                {
                    replyMessage.Parameters.Add("Error", "No Seat Available");
                }

                var replyJson = JsonSerializer.Serialize(replyMessage, new JsonSerializerOptions
                {
                    Converters = {new ServiceMessageJsonConverter()}
                });
                PublishMessage("student.registration", replyJson);
            };

            Channel.BasicConsume(QueueName, true, consumer);

            await Task.CompletedTask;
        }
    }
}