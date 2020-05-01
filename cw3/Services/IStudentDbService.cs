using System.Threading.Tasks;
using cw3.DTOs.Requests;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Services
{
    public interface IStudentDbService
    {
        Task<IActionResult> EnrollStudent(EnrollStudentRequest request);
        Task<IActionResult> PromoteStudents(PromoteStudentRequest request);
        Student GetStudent(string id);
        
        bool CheckIndexNumber(string index);

        bool CheckUserCredentials(string index, string password);
    }
}