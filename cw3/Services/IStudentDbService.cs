using cw3.DTOs.Requests;
using cw3.Models;

namespace cw3.Services
{
    public interface IStudentDbService
    {
        void EnrollStudent(EnrollStudentRequest request);
        void PromoteStudents(PromoteStudentRequest request);
        Student GetStudent(string id);
    }
}