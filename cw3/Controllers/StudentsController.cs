using System;
using System.Linq;
using cw3.Models;
using cw3.Services;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    [Route("api/students")]      //jesli otrzyma jakies żądanie http na tą sciezke, to bedzie przekierowany do tego kontrolera
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public StudentsController(IDbService service)
        {
            _dbService = service;
        }
        [HttpGet]
        public IActionResult GetStudent(string orderBy)
        {
            if (orderBy == "lastname")
            {
                return Ok(_dbService.GetStudents().OrderBy(s => s.LastName));
            }

            return Ok(_dbService.GetStudents());
        }

        
        //1. URL segment
           [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            if (id == 1)
            {
                return Ok("Kowalski");
            }else if (id == 2)
            {
                return Ok("Malewski");
            }

            return NotFound("Nie znaleziono studenta");
        }
        //3. żądanie POST
        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";//generuje numer indeksu
            return Ok(student);
        }

        [HttpPut]
        public IActionResult UpdateInfo(Student s)
        {
            if (s.LastName == "Kowalski")
            {
                s.IndexNumber = "s00001";
                return Ok(s);
            }
            else
            {
                return NotFound("Nie wprowadzono zmian");
            }
        }

        [HttpDelete]

        public IActionResult DeleteStudent(Student s)
        {
            if (s.IdStudent == 2)
            {
                DeleteStudent(s);
                return Ok();
            }
            else
            {
                return NotFound("Nie usunięto studenta");
            }
        }
     
    }
}