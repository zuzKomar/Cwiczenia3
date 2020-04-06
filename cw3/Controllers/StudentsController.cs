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
        public IActionResult GetStudents()
        {
            var result = new List<Student>();
            using (var con = new SqlConnection(ConString)) //polacznie do bazy
            using (var com = new SqlCommand()) //zapytanie SQL wysylane do serwera
            { 
                com.Connection = con;
                com.CommandText =
                   "select s.FirstName, s.LastName, s.BirthDate, e.Semester, st.Name from Student s, Enrollment e, Studies st where s.IdEnrollment=e.IdEnrollment AND e.IdStudy=st.IdStudy;";

                con.Open();                     //otwarcie połączenia do bazy danych
              
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString(); //wczytanie imienia
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = dr["BirthDate"].ToString();
                    st.StudiesName = dr["Name"].ToString();
                    st.Semester = dr["SemestrNumber"].ToString();
                    result.Add(st);
                }
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
                    "select Student.IndexNumber, Student.FirstName, Student.LastName, Studies.Name, convert(varchar, Enrollment.StartDate, 105) as StartDate, Enrollment.Semester from dbo.Student inner join Enrollment on Enrollment.IdEnrollment = Student.IdEnrollment inner join Studies on Studies.IdStudy = Enrollment.IdStudy where Student.IndexNumber = @index";
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

                return Ok(st);
                }
            }
            return NotFound("Nie znaleziono studenta o podanym numerze indeksu.");
        }
    }
}
// if (dr["IndexNumber"] == DBNull.Value) //NULL bazodanowy

                //com.ExecuteScalar();             odczytanie pojedynczej wartosci
                //com.ExecuteReader();             pozwala na uzyskanie strumienia i odczytanie danych z bazy danych
                //com.ExecuteNonQuery();           uruchmonienie zapytania na ktore nie oczekujemy odpowiedzi<inert,update,delete>, zwraca liczbe zmodyfikowanych rekordow
                //con.Dispose();                   zamkniecie polaczenia z bazą
                
                
//[HttpGet]
// public IActionResult GetStudent(string orderBy)
// {
//     if (orderBy == "lastname")
//     {
//         return Ok(_dbService.GetStudents().OrderBy(s => s.LastName));
//     }
//
//     return Ok(_dbService.GetStudents());
// }


// //1. URL segment
// [HttpGet("{id}")]
// public IActionResult GetStudent(int id)
// {
//     if (id == 1)
//     {
//         return Ok("Kowalski");
//     }
//     else if (id == 2)
//     {
//         return Ok("Malewski");
//     }
//
//     return NotFound("Nie znaleziono studenta");
// }
