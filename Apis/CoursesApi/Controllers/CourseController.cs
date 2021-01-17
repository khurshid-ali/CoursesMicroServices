using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CoursesApi.Services;
using CoursesApi.Entities;
using System.Threading.Tasks;

namespace CoursesApi.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _service;

        public CourseController(ICourseService courseSerice)
        {
            _service = courseSerice;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseEntity>>> GetAsync()
        {
            return await _service.GetAsync();
        }

        [HttpGet("{id:length(24)}", Name = "GetCourse")]
        public async Task<ActionResult<CourseEntity>> GetCourseAsync(string id)
        {
            var course = await _service.GetAsync(id);

            if (course == null) return NotFound();

            return course;
        }

        [HttpPost]
        public async Task<ActionResult<CourseEntity>> CreateAsync(CourseEntity courseIn)
        {
            await _service.CreateAsync(courseIn);

            return CreatedAtAction("GetCourse", new { id = courseIn.Id.ToString() }, courseIn);
        }


        [HttpPut]
        public async Task<ActionResult<CourseEntity>> UpdateAync(string id, CourseEntity courseIn)
        {
            var courseFound = await _service.GetAsync(id);

            if (courseFound == null) return NotFound();

            await _service.UpdateAsync(id, courseIn);

            return NoContent();
        }


        [HttpDelete]
        public async Task<ActionResult<CourseEntity>> DeleteAsync(string id)
        {
            var courseFound = await _service.GetAsync(id);

            if (courseFound == null) return NotFound();

            await _service.DeleteAsync(id);

            return NoContent();
        }



    }
}