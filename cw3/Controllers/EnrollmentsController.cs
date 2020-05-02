
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using cw3.Handlers;
using cw3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace cw3.Controllers
{
    
    [ApiController] // implicit model validation
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentDbService _dbService;
        public IConfiguration Configuration { get; set; }

        public EnrollmentsController(IStudentDbService dbService, IConfiguration configuration)
        {
            Configuration = configuration;
            _dbService = dbService;
        }
        
        [HttpPost]
        [Authorize(Roles = "employee")]
        public async Task<IActionResult> EnrollStudent(EnrollStudentRequest newStudent)
        {
           await _dbService.EnrollStudent(newStudent);
           var response = new EnrollStudentResponse()
           {
               IndexNumber = newStudent.IndexNumber,
               FirstName = newStudent.FirstName,
               LastName = newStudent.LastName,
               BirthDate = newStudent.BirthDate.ToString(),
               Semester = 1,
               Study = newStudent.StudiesName
           };

            return CreatedAtAction("EnrollStudent", response);
        }

        [HttpPost("promotions")]
        [Authorize(Roles = "employee")]
        public async Task<IActionResult> PromoteStudents(PromoteStudentRequest request)
        {
            await _dbService.PromoteStudents(request);
            var response = new PromoteStudentResponse()
            {
                Semester = Convert.ToInt32(request.Semester) + 1,
                StudiesName = request.StudiesName
            };

            return Ok(response);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var authHandler = new BearerAuthHandler(_dbService);

            if (await authHandler.HandleAuthenticateAsync(request) != Accepted())
                return BadRequest("Błędne dane logowania!");
            
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, request.IndexNumber),
                new Claim(ClaimTypes.Name, "Pjatk"), 
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "student"), 
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token), //żyje 5-10 minut
                refreshToken = Guid.NewGuid() //losowy, dlugi ciąg znaków zapisywany w bazie danych, uzywany do wygenerowania nowego Tokena bez koniecznosci ponownego logowania się
            });
        }
    }
}
