using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    [Route("api/students")]      //jesli otrzyma jakies żądanie http na tą sciezke, to bedzie przekierowany do tego kontrolera
    [ApiController]
    public class StudentsController : ControllerBase
    {
        [HttpGet] //tylko rządania http get beda realizowane przez ta metode
        
        public string GetStudents()
        {
            return "Anna, Krzysztof i Basia";
        }
    }
}