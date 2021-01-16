using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using StudentApi.Services;
using StudentApi.Entities;

namespace StudentApi.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {

        public IStudentService StudentService { get; init; }

        public StudentsController(IStudentService studentService)
        {
            StudentService = studentService;
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

            return CreatedAtAction("GetStudent", new { id = student.Id.ToString() }, student);
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