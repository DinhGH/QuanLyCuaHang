using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHang
{
    public class DatabaseHelper
    {
        private static string GetConnectionString()
        {
            // L?y connection string t? App.config
            string connStr = ConfigurationManager.ConnectionStrings["QuanLyCuaHangEntities1"]?.ConnectionString;
            
            // Parse Entity Framework connection string ?? l?y provider connection string
            if (!string.IsNullOrEmpty(connStr) && connStr.Contains("provider connection string"))
            {
                int startIndex = connStr.IndexOf("provider connection string=\"") + 28;
                int endIndex = connStr.LastIndexOf("\"");
                connStr = connStr.Substring(startIndex, endIndex - startIndex);
                connStr = connStr.Replace("&quot;", "\"");
            }
            else
            {
                // Fallback connection string
                connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\QLCHDB.mdf;Integrated Security=True;Connect Timeout=30";
            }
            
            return connStr;
        }

        public static SqlConnection GetConnection()
        {
            string connectionString = GetConnectionString();
            return new SqlConnection(connectionString);
        }

        // L?y danh sách s?n ph?m
        public static List<SanPham> GetAllSanPham()
        {
            List<SanPham> dsSanPham = new List<SanPham>();
            
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT MaSP, TenSP, GiaBan, SoLuongTon, DonViTinh FROM SanPham WHERE SoLuongTon > 0";
                    
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SanPham sp = new SanPham
                                {
                                    MaSP = reader.GetInt32(0),
                                    TenSP = reader.GetString(1),
                                    GiaBan = reader.GetDecimal(2),
                                    SoLuongTon = reader.GetInt32(3),
                                    DonViTinh = reader.IsDBNull(4) ? "" : reader.GetString(4)
                                };
                                dsSanPham.Add(sp);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("L?i khi l?y danh sách s?n ph?m: " + ex.Message);
                }
            }
            
            return dsSanPham;
        }

        // L?u hóa ??n
        public static int LuuHoaDon(HoaDon hoaDon)
        {
            int maHD = 0;
            
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"INSERT INTO HoaDon (MaHoaDon, KhachHang, SoDienThoai, DiaChi, NgayLap, TongTien, HinhThucThanhToan, TienKhachDua, TienThua) 
                                   VALUES (@MaHoaDon, @KhachHang, @SoDienThoai, @DiaChi, @NgayLap, @TongTien, @HinhThucThanhToan, @TienKhachDua, @TienThua);
                                   SELECT CAST(SCOPE_IDENTITY() AS INT)";
                    
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaHoaDon", hoaDon.MaHoaDon);
                        cmd.Parameters.AddWithValue("@KhachHang", hoaDon.KhachHang ?? "Khách l?");
                        cmd.Parameters.AddWithValue("@SoDienThoai", hoaDon.SoDienThoai ?? "");
                        cmd.Parameters.AddWithValue("@DiaChi", hoaDon.DiaChi ?? "");
                        cmd.Parameters.AddWithValue("@NgayLap", hoaDon.NgayLap);
                        cmd.Parameters.AddWithValue("@TongTien", hoaDon.TongTien);
                        cmd.Parameters.AddWithValue("@HinhThucThanhToan", hoaDon.HinhThucThanhToan ?? "Ti?n m?t");
                        cmd.Parameters.AddWithValue("@TienKhachDua", hoaDon.TienKhachDua);
                        cmd.Parameters.AddWithValue("@TienThua", hoaDon.TienThua);
                        
                        maHD = (int)cmd.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("L?i khi l?u hóa ??n: " + ex.Message);
                }
            }
            
            return maHD;
        }

        // L?u chi ti?t hóa ??n
        public static void LuuChiTietHoaDon(int maHD, List<ChiTietHoaDon> dsChiTiet)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    
                    foreach (var item in dsChiTiet)
                    {
                        string query = @"INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia, ThanhTien) 
                                       VALUES (@MaHD, @MaSP, @SoLuong, @DonGia, @ThanhTien)";
                        
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@MaHD", maHD);
                            cmd.Parameters.AddWithValue("@MaSP", item.MaSP);
                            cmd.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                            cmd.Parameters.AddWithValue("@DonGia", item.DonGia);
                            cmd.Parameters.AddWithValue("@ThanhTien", item.ThanhTien);
                            
                            cmd.ExecuteNonQuery();
                        }
                        
                        // C?p nh?t s? l??ng t?n kho
                        string updateQuery = "UPDATE SanPham SET SoLuongTon = SoLuongTon - @SoLuong WHERE MaSP = @MaSP";
                        using (SqlCommand cmdUpdate = new SqlCommand(updateQuery, conn))
                        {
                            cmdUpdate.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                            cmdUpdate.Parameters.AddWithValue("@MaSP", item.MaSP);
                            cmdUpdate.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("L?i khi l?u chi ti?t hóa ??n: " + ex.Message);
                }
            }
        }

        // T?o mã hóa ??n t? ??ng
        public static string TaoMaHoaDon()
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM HoaDon";
                    
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        int count = (int)cmd.ExecuteScalar();
                        return "HD" + (count + 1).ToString("D4");
                    }
                }
                catch
                {
                    return "HD" + DateTime.Now.ToString("yyyyMMddHHmmss");
                }
            }
        }

        // Them vao class DatabaseHelper
        public static Employee ValidateLogin(string username, string password)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    // Simple validation - trong thuc te nen ma hoa password
                    string query = @"SELECT id, full_name, phone, email, role, salary, created_at 
                           FROM employees 
                           WHERE email = @Username AND phone = @Password";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Employee
                                {
                                    Id = reader.GetInt32(0),
                                    FullName = reader.GetString(1),
                                    Phone = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    Role = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    Salary = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5),
                                    CreatedAt = reader.IsDBNull(6) ? DateTime.Now : reader.GetDateTime(6)
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Login validation error: " + ex.Message);
                }
            }

            return null;
        }
    }
}