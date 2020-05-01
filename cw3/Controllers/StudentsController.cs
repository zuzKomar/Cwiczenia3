using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;


namespace cw3.Controllers
{
    [Route("api/students")]     //jesli otrzyma jakies żądanie http na tą sciezke, to bedzie przekierowany do tego kontrolera
    [ApiController]
    public class StudentsController : ControllerBase
    {

        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17301;Integrated Security=True";
        
         [HttpPost]
         public IActionResult CreateStudent(Student student)
         {
             student.IndexNumber = $"s{new Random().Next(1, 20000)}"; //generuje numer indeksu
             return Ok(student);
         }

         [HttpPut]
         public IActionResult UpdateInfo(int id)
         {
             return Ok("Zaktualizowano dane studenta");
         }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Pomyślnie usunieto studenta o podanym numerze id");
        }

        [HttpGet]
        //[Authorize] //metoda dostepna tylko dla zalogowanych userów 
        public IActionResult GetStudents()
        {
            
            var result = new List<Student>();
            using (var con = new SqlConnection(ConString)) //polacznie do bazy
            using (var com = new SqlCommand()) //zapytanie SQL wysylane do serwera
            { 
                com.Connection = con;
                com.CommandText =
                   "select s.IndexNumber ,s.FirstName, s.LastName, s.BirthDate, e.Semester, st.Name from Student s, Enrollment e, Studies st where s.IdEnrollment=e.IdEnrollment AND e.IdStudy=st.IdStudy;";

                con.Open();                     //otwarcie połączenia do bazy danych
              
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString(); 
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = dr["BirthDate"].ToString();
                    st.StudiesName = dr["Name"].ToString();
                    st.Semester = dr["SemestrNumber"].ToString();
                    result.Add(st);
                }
                con.Close();
            }
            
            return Ok(result);
        }

        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)
        {
            using (var con = new SqlConnection(ConString))      //polacznie do bazy
            using (var com = new SqlCommand())                  //zapytanie SQL wysylane do serwera
            {
                com.Connection = con;
                com.CommandText =
                    "select s.IndexNumber, s.FirstName, s.LastName, s.Name, convert(varchar, e.StartDate, 105) as e.StartDate, e.Semester from dbo.Student s inner join Enrollment e on e.IdEnrollment = s.IdEnrollment inner join Studies st on st.IdStudy = e.IdStudy where s.IndexNumber = @index";
                com.Parameters.AddWithValue("index", indexNumber);       
                
                con.Open();
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    var st = new Student { 
                    IndexNumber = dr["IndexNuber"].ToString(),
                    FirstName = dr["FirstName"].ToString(),
                    LastName = dr["LastName"].ToString(),
                    BirthDate = dr["BirthDate"].ToString(),
                    StudiesName = dr["Name"].ToString(),
                    Semester = dr["SemestrNumber"].ToString(),
                    };

                    con.Close();
                return Ok(st);
                }
            }
            return NotFound("Nie znaleziono studenta o podanym numerze indeksu.");
        }
    }
}
