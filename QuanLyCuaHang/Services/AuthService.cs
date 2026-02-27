using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyCuaHang.Models;

namespace QuanLyCuaHang.Services
{
    public class AuthService
    {
        private const string ConnectionString =@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\QLCHDB.mdf;Integrated Security=True;Connect Timeout=30";

        /// <summary>
        /// Xác th?c ??ng nh?p b?ng SQL tr?c ti?p
        /// </summary>
        public static User Login(string email, string password)
        {   
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return null;
                }

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    
                    string query = "SELECT id, full_name, phone, email, role, salary, password, created_at FROM employees WHERE email = @email AND password = @password";
                    
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                User user = new User
                                {
                                    id = (int)reader["id"],
                                    full_name = reader["full_name"].ToString(),
                                    phone = reader["phone"].ToString(),
                                    email = reader["email"].ToString(),
                                    role = reader["role"].ToString(),
                                    salary = reader["salary"] != DBNull.Value ? (decimal)reader["salary"] : 0,
                                    password = reader["password"].ToString(),
                                    created_at = (DateTime)reader["created_at"]
                                };

                                System.Diagnostics.Debug.WriteLine($"??ng nh?p thành công: {user.full_name}");
                                return user;
                            }
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Email ho?c password không ?úng");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"L?i ??ng nh?p: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// L?y thông tin nhân viên theo ID
        /// </summary>
        public static User GetUserById(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    
                    string query = "SELECT id, full_name, phone, email, role, salary, password, created_at FROM employees WHERE id = @id";
                    
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    id = (int)reader["id"],
                                    full_name = reader["full_name"].ToString(),
                                    phone = reader["phone"].ToString(),
                                    email = reader["email"].ToString(),
                                    role = reader["role"].ToString(),
                                    salary = reader["salary"] != DBNull.Value ? (decimal)reader["salary"] : 0,
                                    password = reader["password"].ToString(),
                                    created_at = (DateTime)reader["created_at"]
                                };
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"L?i l?y nhân viên: {ex.Message}");
                return null;
            }
        }
    }
}