using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using cw3.Models;
using cw3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.CompilerServices;

namespace cw3.Controllers
{
    [Route("api/students")] //jesli otrzyma jakies żądanie http na tą sciezke, to bedzie przekierowany do tego kontrolera
    [ApiController]
    
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17301;Integrated Security=True";

        public StudentsController(IDbService service)
        {
            _dbService = service;
        }

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

        //3. żądanie POST
         [HttpPost]
         public IActionResult CreateStudent(Student student)
         {
             student.IndexNumber = $"s{new Random().Next(1, 20000)}"; //generuje numer indeksu
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

        [HttpDelete("{id}")]

        public IActionResult DeleteStudent(int id)
        {
            return Ok("Pomyślnie usunieto Studenta o podanym numerze id");
        }
        

        [HttpGet]

        public IActionResult GetStudents()
        {
            var result = new List<Student>();
            using (var con = new SqlConnection(ConString)) //polacznie do bazy
            using (var com = new SqlCommand()) //zapytanie SQL wysylane do serwera
            {
                com.Connection = con;
               // com.CommandText = "select * from Student";
               com.CommandText =
                   "select s.FirstName, s.LastName, s.BirthDate, e.Semester, st.Name from Student s, Enrollment e, Studies st where s.IdEnrollment=e.IdEnrollment AND e.IdStudy=st.IdStudy;";

                con.Open();                 //otwarcie połączenia do bazy danych
               // com.ExecuteScalar();     odczytanie pojedynczej wartosci
               //com.ExecuteReader();    pozwala na uzyskanie strumienia i odczytanie danych z bazy danych
               // com.ExecuteNonQuery(); //uruchmonienie zapytania na ktore nie oczekujemy odpowiedzi<inert,update,delete>, zwraca liczbe zmodyfikowanych rekordow
               //   con.Dispose();            //zamkniecie polaczenia z bazą
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())//wczytuje kolejny wiersz i zwraca boolean sprawdzajac czy kolejny wiersz istnieje
                {
                    var st = new Student();
                    st.FirstName = dr["FirstName"].ToString(); //wczytanie imienia
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = dr["BirthDate"].ToString();
                    st.StudiesName = dr["Name"].ToString();
                    st.SemestrNumber = dr["Semester"].ToString();
                    result.Add(st);
                }
            }
            return Ok(result);
        }

        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)
        {
            using (var con = new SqlConnection(ConString)) //polacznie do bazy
            using (var com = new SqlCommand()) //zapytanie SQL wysylane do serwera
            {
                com.Connection = con;
                com.CommandText = "SELECT * FROM ENROLLMENT E, STUDENT S WHERE S.IDENROLLMENT = E.IDENROLLMENT AND S.INDEXNUMBER =@index";
                com.Parameters.AddWithValue("index", indexNumber);       
                
                con.Open();
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    var st = new Student();

                    // if (dr["IndexNumber"] == DBNull.Value) //NULL bazodanowy
                    //{ }

                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.SemestrNumber = dr["Semester"].ToString();
                    st.StartDate = dr["StartDate"].ToString();
                    return Ok(st);
                }
            }
            return NotFound();
        }
    }
}
