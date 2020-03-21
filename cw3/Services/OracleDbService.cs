using System.Collections.Generic;
using cw3.Models;

namespace cw3.Services
{
    public class OracleDbService :IDbService
    {
        public IEnumerable<Student> GetStudents()
        {
            //real db connection
            throw new System.NotImplementedException();
        }
    }
}