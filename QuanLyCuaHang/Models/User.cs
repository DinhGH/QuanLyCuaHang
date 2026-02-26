using System;

namespace QuanLyCuaHang.Models
{
    public class User
    {
        public int id { get; set; }
        public string full_name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public decimal salary { get; set; }
        public DateTime created_at { get; set; }
        public string password { get; set; }
    }
}