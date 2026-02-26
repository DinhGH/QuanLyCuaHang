using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;

namespace QuanLyCuaHang
{
    public class DatabaseHelper
    {
        private static string GetConnectionString()
        {
            string connStr = ConfigurationManager.ConnectionStrings["QuanLyCuaHangEntities1"]?.ConnectionString;
            
            if (!string.IsNullOrEmpty(connStr) && connStr.Contains("provider connection string"))
            {
                int startIndex = connStr.IndexOf("provider connection string=\"") + 28;
                int endIndex = connStr.LastIndexOf("\"");
                connStr = connStr.Substring(startIndex, endIndex - startIndex);
                connStr = connStr.Replace("&quot;", "\"");
            }
            else
            {
                connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\QLCHDB.mdf;Integrated Security=True;Connect Timeout=30";
            }
            
            return connStr;
        }

        public static SqlConnection GetConnection()
        {
            string connectionString = GetConnectionString();
            return new SqlConnection(connectionString);
        }

        // Lay danh sach san pham
        public static List<SanPham> GetAllSanPham()
        {
            List<SanPham> dsSanPham = new List<SanPham>();
            
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id, name, price, quantity, unit FROM products WHERE quantity > 0 ORDER BY name";
                    
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
                    throw new Exception("Loi khi lay danh sach san pham: " + ex.Message + "\nConnectionString: " + GetConnectionString());
                }
            }
            
            return dsSanPham;
        }

        // Tao ma hoa don tu dong
        public static string TaoMaHoaDon()
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM bills";
                    
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

        // Luu hoa don
        public static int LuuHoaDon(HoaDon hoaDon)
        {
            int maHD = 0;
            
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    
                    // Tim hoac tao customer
                    int? customerId = null;
                    if (!string.IsNullOrEmpty(hoaDon.SoDienThoai))
                    {
                        string queryCustomer = "SELECT id FROM customers WHERE phone = @Phone";
                        using (SqlCommand cmdCustomer = new SqlCommand(queryCustomer, conn))
                        {
                            cmdCustomer.Parameters.AddWithValue("@Phone", hoaDon.SoDienThoai);
                            var result = cmdCustomer.ExecuteScalar();
                            if (result != null)
                            {
                                customerId = (int)result;
                            }
                            else
                            {
                                // Tao customer moi
                                string insertCustomer = @"INSERT INTO customers (full_name, phone, email, address, created_at) 
                                                        VALUES (@FullName, @Phone, @Email, @Address, @CreatedAt);
                                                        SELECT CAST(SCOPE_IDENTITY() AS INT)";
                                using (SqlCommand cmdInsertCustomer = new SqlCommand(insertCustomer, conn))
                                {
                                    cmdInsertCustomer.Parameters.AddWithValue("@FullName", hoaDon.KhachHang ?? "Khach le");
                                    cmdInsertCustomer.Parameters.AddWithValue("@Phone", hoaDon.SoDienThoai ?? "");
                                    cmdInsertCustomer.Parameters.AddWithValue("@Email", "");
                                    cmdInsertCustomer.Parameters.AddWithValue("@Address", hoaDon.DiaChi ?? "");
                                    cmdInsertCustomer.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                                    customerId = (int)cmdInsertCustomer.ExecuteScalar();
                                }
                            }
                        }
                    }
                    
                    // Luu hoa don
                    string query = @"INSERT INTO bills (customer_id, employee_id, total_amount, bill_date) 
                                   VALUES (@CustomerId, @EmployeeId, @TotalAmount, @BillDate);
                                   SELECT CAST(SCOPE_IDENTITY() AS INT)";
                    
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CustomerId", customerId.HasValue ? (object)customerId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@EmployeeId", 1);
                        cmd.Parameters.AddWithValue("@TotalAmount", hoaDon.TongTien);
                        cmd.Parameters.AddWithValue("@BillDate", hoaDon.NgayLap);
                        
                        maHD = (int)cmd.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Loi khi luu hoa don: " + ex.Message);
                }
            }
            
            return maHD;
        }

        // Luu chi tiet hoa don
        public static void LuuChiTietHoaDon(int maHD, List<ChiTietHoaDon> dsChiTiet)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    
                    foreach (var item in dsChiTiet)
                    {
                        // Luu chi tiet
                        string query = @"INSERT INTO bill_items (bill_id, product_id, quantity, price) 
                                       VALUES (@BillId, @ProductId, @Quantity, @Price)";
                        
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@BillId", maHD);
                            cmd.Parameters.AddWithValue("@ProductId", item.MaSP);
                            cmd.Parameters.AddWithValue("@Quantity", item.SoLuong);
                            cmd.Parameters.AddWithValue("@Price", item.DonGia);
                            
                            cmd.ExecuteNonQuery();
                        }
                        
                        // Cap nhat ton kho
                        string updateQuery = "UPDATE products SET quantity = quantity - @Quantity WHERE id = @ProductId";
                        using (SqlCommand cmdUpdate = new SqlCommand(updateQuery, conn))
                        {
                            cmdUpdate.Parameters.AddWithValue("@Quantity", item.SoLuong);
                            cmdUpdate.Parameters.AddWithValue("@ProductId", item.MaSP);
                            cmdUpdate.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Loi khi luu chi tiet hoa don: " + ex.Message);
                }
            }
        }
    }
}