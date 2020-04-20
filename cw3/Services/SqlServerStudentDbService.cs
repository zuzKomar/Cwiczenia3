using System;
using System.Data.SqlClient;
using cw3.DTOs.Requests;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Services
{
    public class SqlServerStudentDbService : IStudentDbService
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17301;Integrated Security=True";
        public void EnrollStudent(EnrollStudentRequest request)
        {
            var st = new Student();
            st.IndexNumber = request.IndexNumber;
            st.FirstName = request.FirstName;
            st.LastName = request.LastName;
            st.BirthDate = request.BirthDate.ToString();
            st.StudiesName = request.StudiesName;
            st.Semester = 1.ToString();
           // st.StartDate = (DateTime.Now).ToString();
           
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
                        com.Transaction.Rollback();
                    }

                    var idStudies = (int) dr["IdStudies"];

                    com.CommandText = "SELECT IDENROLLMENT FROM ENROLLMENTS WHERE IDSTUDY = @IDSTUDY AND SEMESTER =1";
                    com.Parameters.AddWithValue("idStudy", idStudies);
                    dr = com.ExecuteReader();

                    if (!dr.Read())
                    {
                        com.CommandText =
                            "INSERT INTO ENROLLMENT(IDENROLLMENT, SEMESTER, IDSTUDY, STARTDATE) VALUES ((SELECT MAX(IDENROLLMENT))+1, 1, @IDSTUDY, @DATE)";
                        com.Parameters.AddWithValue("idStudy", idStudies);
                        com.Parameters.AddWithValue("date", DateTime.Now.ToString("yyyy-MM-dd"));
                        com.ExecuteNonQuery();
                        com.CommandText =
                            "SELECT IDENROLLMENT FROM ENROLLMENT WHERE IDSTUDY = @IDSTUDY AND SEMESTER = 1";
                        dr = com.ExecuteReader();
                    }

                    var idEnrollment = (int) dr["IdEnrollment"];

                    com.CommandText = "SELECT 1 FROM STUDENT WHERE INDEXNUMBER = @INDEXNUMBER";
                    com.Parameters.AddWithValue("indexNumber", request.IndexNumber);
                    dr = com.ExecuteReader();
                    if(dr.Read())
                        com.Transaction.Rollback();
                    
                    dr.Close();
                    //dodanie studenta
                    com.CommandText = "INSERT INTO STUDENT(INDEXNUMBER, FISTNAME, LASTNAME, BIRTHDATE, IDENROLLMENT) VALUES(@INDEXNUMBER, @FIRSTNAME, @LASTNAME, @BIRTHDATE, @IDENROLLMENT)";
                    com.Parameters.AddWithValue("IndexNumber", request.IndexNumber);
                    com.Parameters.AddWithValue("FirstName", request.FirstName);
                    com.Parameters.AddWithValue("LastName", request.LastName);
                    com.Parameters.AddWithValue("BirthDate", request.BirthDate);
                    com.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                    com.ExecuteNonQuery();

                    com.Transaction.Commit();
                    
                }
                catch (SqlException ex)
                {
                    com.Transaction.Rollback();
                }
            }
        }
        
        public void PromoteStudents(PromoteStudentRequest request)
        {
            using (var con = new SqlConnection())
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                com.Transaction = con.BeginTransaction();
                try
                {
                    com.CommandText =
                        "SELECT IDENROLMENT FROM ENROLLMENT E, STUDIES S WHERE E.IDSTUDIES=S.IDSTUDIES AND S.NAME=@STUDIESNAME AND E.SEMESTER=@SEMESTER";
                    com.Parameters.AddWithValue("studiesName", request.StudiesName);
                    com.Parameters.AddWithValue("semester", request.Semester);

                    var reader = com.ExecuteReader();
                    if (!reader.Read())
                    {
                        com.Transaction.Rollback();
                    }
                    
                }
                catch (SqlException s)
                {
                    com.Transaction.Rollback();
                }
                com.Transaction.Commit();
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
    }

       
    
}