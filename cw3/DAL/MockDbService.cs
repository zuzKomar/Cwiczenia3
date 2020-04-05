using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using cw3.DAL;
using cw3.Models;

namespace cw3.Services
{
    public class MockDbService : IDbService
    {
        private static ICollection<Student> _students;
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17301;Integrated Security=True";

        static MockDbService()
        {
            _students = new List<Student>();
        }
        public IEnumerable<Student> GetStudents()
        {
            using(var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText =
                    "SELECT FirstName, LastName, IndexNumber, BirthDate, Student.IdEnrollment, Studies.Name, Enrollment.Semester FROM Student INNER JOIN Enrollment ON Student.IdEnrollment = Enrollment.IdEnrollment INNER JOIN Studies ON Enrollment.IdStudy = Studies.IdStudy";
                con.Open();

                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    _students.Add(new Student
                    {
                        IndexNumber = dr["IndexNumber"].ToString(),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = dr["BirthDate"].ToString(),
                        StudiesName = dr["StudiesName"].ToString(),
                        Semester = dr["Semester"].ToString()
                    });
                }
            }

            return _students;
        }
        

        public Student getStudent(string id)
        {
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText="Select FirstName, LastName, IndexNumber, BirthDate, Student.IdEnrollment, Studies.Name, Enrollment.Semester FROM Student INNER JOIN Enrollment ON Student.IdEnrollment = Enrollment.IdEnrollment INNER JOIN Studies ON Enrollment.IdStudy = Studies.IdStudy WHERE Student.IndexNumber=@id";
                com.Parameters.AddWithValue("id", id);

                Student s = null;
                con.Open();
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    s = new Student
                    {
                        IndexNumber = dr["IndexNumber"].ToString(),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = dr["BirthDate"].ToString(),
                        StudiesName = dr["StudiesName"].ToString(),
                        Semester = dr["Semester"].ToString()
                    };
                }

                return s;
            }
        }
    }
}