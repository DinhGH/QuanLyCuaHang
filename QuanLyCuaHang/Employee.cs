using System;

namespace QuanLyCuaHang
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public decimal Salary { get; set; }
        public DateTime CreatedAt { get; set; }

        public Employee()
        {
        }

        public Employee(int id, string fullName, string role)
        {
            Id = id;
            FullName = fullName;
            Role = role;
        }
    }
}