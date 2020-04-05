using cw3.DTOs.Requests;

namespace cw3.Services
{
    public interface IStudentDbService
    {
        void EnrollStudent(EnrollStudentRequest request);
        void PromoteStudents(PromoteStudentRequest request);
    }
}