using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers
{
    [Route("api/{controller}")]
    [ApiController]
    public class UserController : ControllerBase
    {

        public UserController()
        {

        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return new JsonResult(new string[] { "value1", "value2" });
        }

    }
}