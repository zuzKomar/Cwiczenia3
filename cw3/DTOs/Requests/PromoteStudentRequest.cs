using System.ComponentModel.DataAnnotations;

namespace cw3.DTOs.Requests
{
    public class PromoteStudentRequest
    {
        [Required] 
        public string StudiesName { get; set; }
        
        [Required] 
        public int Semester { get; set; }
        
    }
}