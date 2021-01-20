using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StudentApi.Entities;
using Courses.Common;
using StudentApi.Services;

namespace StudentApi.BgWorkers
{
    public class StudentQueueConsumer : BaseQueueConsumerBackgroundService
    {
        private readonly IStudentService _service;
        
        public StudentQueueConsumer(IRabbitMqConfiguration rabbitConfig, IStudentService studentService):base(rabbitConfig)
        {
            _service = studentService;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += (sender, e) =>
            {
                var msgString = Encoding.UTF8.GetString(e.Body.ToArray());

                var msg = JsonSerializer.Deserialize<ServiceMessage>(msgString, new JsonSerializerOptions
                {
                    Converters = { new ServiceMessageJsonConverter()}
                });

                if (msg.Parameters.ContainsKey("Error"))
                {
                    Console.WriteLine($"Error occured: {msg.Parameters["Error"]}");
                }
                else
                {
                    var studentId = msg.Parameters["StudentId"];
                    var courseId = msg.Parameters["CourseId"];
                    var courseName = msg.Parameters["CourseName"];
                    var courseCode = msg.Parameters["CourseCode"];

                    var student = _service.Get(studentId);

                    if (student != null)
                    {
                        if (student.Courses == null) student.Courses = new List<RegisteredCourse>();
                        
                        student.Courses.Add(new RegisteredCourse
                        {
                            CourseId = courseId,
                            Code = courseCode,
                            Name = courseName
                        });
                        
                        _service.Update(studentId, student);
                    }
                    else
                    {
                        Console.WriteLine("Could not find student");
                    }
                }
            };

            Channel.BasicConsume(QueueName, true, consumer);

            await Task.CompletedTask;
        }
    }
}