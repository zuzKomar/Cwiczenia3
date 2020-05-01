using System;

namespace cw3.DTOs.Responses
{
    public class EnrollStudentResponse
    {

        public string IndexNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string  BirthDate { get; set; }
        public int Semester { get; set; }
        public string Study { get; set; }
    }
}