
using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using cw3.Services;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    
    [ApiController] // implicit model validation
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentDbService _dbService;

        public EnrollmentsController(IStudentDbService dbService)
        {
            _dbService = dbService;
        }
        
        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest newStudent)
        {
            _dbService.EnrollStudent(newStudent);
            var response = new EnrollStudentResponse();

            return CreatedAtAction("EnrollStudent", response);
        }

        [HttpPost("promotions")]
        public IActionResult PromoteStudents(PromoteStudentRequest request)
        {
            _dbService.PromoteStudents(request);
            var response = new PromoteStudentResponse();

            return Ok(response);
        }
    }
}
