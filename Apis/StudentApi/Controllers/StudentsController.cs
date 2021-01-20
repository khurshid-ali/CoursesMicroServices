using System.Collections.Generic;
using System.Text.Json;
using Courses.Common;
using Microsoft.AspNetCore.Mvc;
using StudentApi.Services;
using StudentApi.Entities;

namespace StudentApi.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {

        private readonly IQueuePublisherService _publisher;
        public IStudentService StudentService { get; init; }

        public StudentsController(IStudentService studentService, IQueuePublisherService publisher)
        {
            StudentService = studentService;
            _publisher = publisher;
        }


        [HttpGet]
        public ActionResult<List<StudentEntity>> Get() => StudentService.Get();

        [HttpGet("{id:length(24)}", Name = "GetStudent")]
        public ActionResult<StudentEntity> Get(string id)
        {
            var student = StudentService.Get(id);

            if (student == null)
                return NotFound();

            return student;
        }

        [HttpPost]
        public ActionResult<StudentEntity> Create(StudentEntity student)
        {
            StudentService.Create(student);

            return CreatedAtAction("Get", new { id = student.Id.ToString() }, student);
        }

        [HttpGet]
        [Route("register/{studentId:length(24)}/{courseCode}")]
        public ActionResult<StudentEntity> RegisterCourse(string studentId, string courseCode)
        {
            var student = StudentService.Get(studentId);
            if (student == null) return NotFound();
            
            //send message to Courses to verify 


            var msg = new ServiceMessage
            {
                ReplyRoutingKey = "student.registration",
                Body = "",
                Parameters = new Dictionary<string, string>
                {
                    {"StudentId", studentId},
                    {"StudentName", $"{student.FirstName}, {student.LastName}"},
                    {"CourseId", courseCode}
                }
            };

            var msgJsonString = JsonSerializer.Serialize(msg, new JsonSerializerOptions
            {
                Converters = { new ServiceMessageJsonConverter()}
            });
            
            _publisher.PublishMessage("course.registration", msgJsonString);

            return student;
        }

        [HttpPut]
        public ActionResult<StudentEntity> Update(string id, StudentEntity studentIn)
        {
            var studentFound = StudentService.Get(id);
            if (studentFound == null)
                return NotFound();

            StudentService.Update(id, studentIn);

            return NoContent();
        }

        [HttpDelete]
        public ActionResult<StudentEntity> Delete(string id)
        {
            var studentFound = StudentService.Get(id);
            if (studentFound == null)
                return NotFound();

            StudentService.Remove(id);
            return NoContent();
        }
    }
}