using System.Collections.Generic;
using cw3.Models;

namespace cw3.Services
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();
    }
}