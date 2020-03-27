using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Models
{
    public class Student
    {
        public string IndexNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string StudiesName { get; set; }
        public int SemestrNumber { get; set; }
        public string StartDate { get; set; }

    }
}