using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using cw3.DTOs.Requests;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace cw3.Services
{
    public class SqlServerStudentDbService : IStudentDbService
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17301;Integrated Security=True";
        public IConfiguration _Configuration { get; set; }

        public SqlServerStudentDbService(IConfiguration configuration)
        {
            _Configuration = configuration;

        }
        public async  Task<IActionResult> EnrollStudent(EnrollStudentRequest request)
        {
            var st = new Student();
            st.IndexNumber = request.IndexNumber;
            st.FirstName = request.FirstName;
            st.LastName = request.LastName;
            st.BirthDate = request.BirthDate.ToString();
            st.StudiesName = request.StudiesName;
            st.Semester = 1.ToString();
            
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection =con;
                con.Open();
                com.Transaction = con.BeginTransaction();

                try
                {
                    //1. czy studia istnieją? 
                    com.CommandText = "SELECT IDSTUDY FROM STUDIES WHERE NAME=@STUDIESNAME";
                    com.Parameters.AddWithValue("studiesName", request.StudiesName);

                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        com.Transaction.Rollback();
                        return new BadRequestResult();
                    }

                    var idStudies = (int) dr["IdStudies"];
                    dr.Close();

                    //2. czy istnieje wpis na pierwszy semestr
                    com.CommandText = "SELECT IDENROLLMENT FROM ENROLLMENT WHERE IDSTUDY = @IDSTUDY AND SEMESTER =1";
                    com.Parameters.AddWithValue("idStudy", idStudies);
                    
                    dr = com.ExecuteReader();
                    if (!dr.Read())   //tworzenie nowego wpisu na pierwszy semestr
                    {
                        dr.Close();
                        com.CommandText =
                            "INSERT INTO ENROLLMENT(IDENROLLMENT, SEMESTER, IDSTUDY, STARTDATE) VALUES ((SELECT MAX(IDENROLLMENT))+1 FROM ENROLLMENT), 1, @IDSTUDY, @DATE)";
                        com.Parameters.AddWithValue("idStudy", idStudies);
                        com.Parameters.AddWithValue("date", DateTime.Now);
                        com.ExecuteNonQuery();
                        com.CommandText =
                            "SELECT IDENROLLMENT FROM ENROLLMENT WHERE IDSTUDY = @IDSTUDY AND SEMESTER = 1";
                        com.Parameters.AddWithValue("IdStudy", idStudies);
                        dr = com.ExecuteReader();
                        dr.Read();
                    }

                    var idEnrollment = (int) dr["IdEnrollment"];
                    dr.Close();

                    // czy student istnieje
                    string indexNumb = request.IndexNumber.Substring(1);
                    
                    com.CommandText = "SELECT 1 FROM STUDENT WHERE INDEXNUMBER = @INDEXNUMBER";
                    com.Parameters.AddWithValue("indexNumber", indexNumb);
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        dr.Close();
                        com.Transaction.Rollback();
                        return new BadRequestResult();
                    }

                    dr.Close();
                    //dodanie studenta
                    com.CommandText = "INSERT INTO STUDENT(INDEXNUMBER, FIRSTNAME, LASTNAME, BIRTHDATE, IDENROLLMENT) VALUES(@INDEXNUMBER, @FIRSTNAME, @LASTNAME, @BIRTHDATE, @IDENROLLMENT)";
                    com.Parameters.AddWithValue("IndexNumber", indexNumb);
                    com.Parameters.AddWithValue("FirstName", request.FirstName);
                    com.Parameters.AddWithValue("LastName", request.LastName);
                    com.Parameters.AddWithValue("BirthDate", request.BirthDate);
                    com.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                    com.ExecuteNonQuery();

                    //odpowiedz
                    com.CommandText =
                        "Select IdEnrollment, Semester, IdStudy, ,StartDate from Enrollment where IdEnrollment = @IdEnrollment";
                    com.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                    dr = com.ExecuteReader();
                    dr.Read();
                    
                    
                    dr.Close();
                    com.Transaction.Commit();
                    return new AcceptedResult();

                }
                catch (SqlException ex)
                {
                    Console.Write(ex);
                    com.Transaction.Rollback();
                    return new BadRequestResult();
                }
            }
        }
        
        public async Task<IActionResult> PromoteStudents(PromoteStudentRequest request)
        {
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                com.Transaction = con.BeginTransaction();
                try
                {
                    com.CommandText =
                        "SELECT IdEnrollment FROM Enrollment, Studies WHERE Enrollment.IdStudy = Studies.IdStudy AND Studies.Name = @studiesName AND Enrollment.Semester = @semester";
                    com.Parameters.AddWithValue("name", request.StudiesName);
                    com.Parameters.AddWithValue("semester", request.Semester);

                    var reader = com.ExecuteReader();
                    if (!reader.Read())
                    {
                        reader.Close();
                        com.Transaction.Rollback();
                        return new BadRequestResult();
                    }
                    
                    com.CommandText = "exec PromoteStudents @name, @semester";
                    com.Parameters.AddWithValue("semestetr", request.Semester);
                    com.Parameters.AddWithValue("name", request.StudiesName);
                    reader.Close();
                    com.ExecuteNonQuery();
                }
                catch (SqlException s)
                {
                    Console.Write(s);
                    com.Transaction.Rollback();
                    return new BadRequestResult();
                }
                com.Transaction.Commit();
                return new AcceptedResult();
            }
        }

        public Student GetStudent(string id)
        {
            using (var con= new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                try{ 
                    com.Connection = con;
                    con.Open();
                    com.CommandText = "select * from Student where IndexNumber=@index";
                    com.Parameters.AddWithValue("index", id);

                    var dr = com.ExecuteReader();
                    if (!dr.HasRows)
                    {
                        return null;
                    }
                    else
                    {
                        Student student = new Student();
                        while (dr.Read())
                        {
                            student.IndexNumber = dr["IndexNumber"].ToString();
                            student.FirstName = dr["FirstName"].ToString();
                            student.LastName = dr["LastName"].ToString();
                            student.BirthDate = dr["BirthDate"].ToString();
                        }

                    dr.Close();
                    return student;
                }
            }
            catch (SqlException e)
            {
                Console.Write(e);
            }
            }
        return null;
            }
        

        public bool CheckIndexNumber(string index)
        {
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                
                con.Open();
                com.CommandText = "Select 1 from Student where Student.IndexNumber = @index";
                com.Parameters.AddWithValue("index", index);

                var dr = com.ExecuteReader();
                return dr.Read();

            }
        }

        public bool CheckUserCredentials(string index, string password)
        {
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                
                con.Open();
                com.CommandText =
                    "Select 1 from Student where Student.IndexNumber = @index and Student.Password = @password";
                com.Parameters.AddWithValue("index", index);
                com.Parameters.AddWithValue("password", password);

                var dr = com.ExecuteReader();
                return dr.Read();

            }
        }
    }

       
    
}