using System.Collections.Generic;
using cw3.Models;

namespace cw3.Services
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students = new List<Student>
        {
            new Student{IdStudent=1, FirstName = "Jan", LastName="Kowalski", IndexNumber="s17302"},
            new Student{IdStudent=2, FirstName="Jerzy", LastName="Kocot", IndexNumber="s17304"},
            new Student{IdStudent=3, FirstName="Krzysztof", LastName="Markowski", IndexNumber="s17300"}
        };

        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }
    }
}