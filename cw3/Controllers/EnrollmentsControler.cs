using System;
using System.Data.SqlClient;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]

    public class EnrollmentsControler : ControllerBase
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17301;Integrated Security=True";

        [HttpPost]
        public IActionResult createEnrollment(Student student)
        {
            using (var con = new SqlConnection(ConString)) //polacznie do bazy
            using (var com = new SqlCommand()) //zapytanie SQL wysylane do serwera
            {
                com.Connection = con;
                com.CommandText = "";


                    con.Open(); //otwarcie połączenia do bazy danych
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    var st = new Student();
                    if (student.IndexNumber.Contains(null))
                    {
                        return NotFound();
                    }
                    st.IndexNumber = student.IndexNumber;
                    st.FirstName = student.FirstName;
                    st.LastName = student.LastName;
                    st.BirthDate = student.BirthDate;
                    st.StudiesName = student.StudiesName;
                    return Ok(st);
                }
            }

            return NotFound();
        }
    

}