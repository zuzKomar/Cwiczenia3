using System;
using System.ComponentModel.DataAnnotations;

namespace cw3.DTOs.Requests
{
    public class EnrollStudentRequest
    {
        [Required(ErrorMessage = "Musisz podać numer indeksu")]
        [RegularExpression("^s[0-9]+$")]
        public string IndexNumber { get; set; }

        [Required(ErrorMessage = "Musisz podać imię")]
        [MaxLength(10)]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Musisz podać nazwisko")]
        [MaxLength(255)]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Musisz podać datę urodzenia!")]
        public DateTime BirthDate { get; set; }
        
        [Required(ErrorMessage = "Musisz podać nazwę kierunku studiów")]
        public string StudiesName { get; set; }
        
        
        
    }
}