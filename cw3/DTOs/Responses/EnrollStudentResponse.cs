using System;

namespace cw3.DTOs.Responses
{
    public class EnrollStudentResponse
    {
        public int IdEnrollment { get; set; }
        public int Semester { get; set; }
        public string Study { get; set; }
        public string StartDate { get; set; }
    }
}