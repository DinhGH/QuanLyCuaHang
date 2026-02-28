using System;
using System.Collections.Generic;
using System.Data;
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

        // Get all products
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
                    throw new Exception("Error loading products: " + ex.Message);
                }
            }
            
            return dsSanPham;
        }

        // Save bill
        public static int LuuHoaDon(HoaDon hoaDon)
        {
            int maHD = 0;
            
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    
                    // Find or create customer
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
                                // Create new customer
                                string insertCustomer = @"INSERT INTO customers (full_name, phone, email, address, created_at) 
                                                        VALUES (@FullName, @Phone, @Email, @Address, @CreatedAt);
                                                        SELECT CAST(SCOPE_IDENTITY() AS INT)";
                                using (SqlCommand cmdInsertCustomer = new SqlCommand(insertCustomer, conn))
                                {
                                    cmdInsertCustomer.Parameters.AddWithValue("@FullName", hoaDon.KhachHang ?? "Guest");
                                    cmdInsertCustomer.Parameters.AddWithValue("@Phone", hoaDon.SoDienThoai ?? "");
                                    cmdInsertCustomer.Parameters.AddWithValue("@Email", "");
                                    cmdInsertCustomer.Parameters.AddWithValue("@Address", hoaDon.DiaChi ?? "");
                                    cmdInsertCustomer.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                                    customerId = (int)cmdInsertCustomer.ExecuteScalar();
                                }
                            }
                        }
                    }
                    
                    // Save bill
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
                    throw new Exception("Error saving bill: " + ex.Message);
                }
            }
            
            return maHD;
        }

        // Save bill details
        public static void LuuChiTietHoaDon(int maHD, List<ChiTietHoaDon> dsChiTiet)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    
                    foreach (var item in dsChiTiet)
                    {
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
                        
                        // Update stock
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
                    throw new Exception("Error saving bill details: " + ex.Message);
                }
            }
        }

        // Generate bill code
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

        // Check if email exists
        public static bool EmailExists(string email)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM employees WHERE email = @Email";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error checking email: " + ex.Message);
                }
            }
        }

        // Register new employee
        public static bool RegisterEmployee(string fullName, string email, string password)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"INSERT INTO employees (full_name, phone, email, role, salary, created_at) 
                           VALUES (@FullName, @Password, @Email, @Role, @Salary, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Role", "Employee");
                        cmd.Parameters.AddWithValue("@Salary", 7000.00);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Registration error: " + ex.Message);
                }
            }
        }

        // Validate login - CH? GI? L?I 1 PH??NG TH?C
        public static Employee ValidateLogin(string username, string password)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT id, full_name, phone, email, role, salary, created_at 
                           FROM employees 
                           WHERE email = @Email AND phone = @Password";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", username);
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
                                    Role = reader.IsDBNull(4) ? "Employee" : reader.GetString(4),
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

        // THÊM VÀO CLASS DatabaseHelper

        // ============ BILL MANAGEMENT ============

        public static List<BillView> GetAllBills()
        {
            List<BillView> bills = new List<BillView>();

            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT b.id, 
                                   ISNULL(c.full_name, 'Guest') AS customer_name,
                                   e.full_name AS employee_name,
                                   b.total_amount,
                                   b.bill_date
                            FROM bills b
                            LEFT JOIN customers c ON b.customer_id = c.id
                            LEFT JOIN employees e ON b.employee_id = e.id
                            ORDER BY b.bill_date DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bills.Add(new BillView
                                {
                                    Id = reader.GetInt32(0),
                                    CustomerName = reader.GetString(1),
                                    EmployeeName = reader.GetString(2),
                                    TotalAmount = reader.GetDecimal(3),
                                    OrderDate = reader.GetDateTime(4)
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error loading bills: " + ex.Message);
                }
            }

            return bills;
        }

        public static BillView GetBillInfo(int billId)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT b.id, 
                                   ISNULL(c.full_name, 'Guest') AS customer_name,
                                   e.full_name AS employee_name,
                                   b.total_amount,
                                   b.bill_date
                            FROM bills b
                            LEFT JOIN customers c ON b.customer_id = c.id
                            LEFT JOIN employees e ON b.employee_id = e.id
                            WHERE b.id = @BillId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BillId", billId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new BillView
                                {
                                    Id = reader.GetInt32(0),
                                    CustomerName = reader.GetString(1),
                                    EmployeeName = reader.GetString(2),
                                    TotalAmount = reader.GetDecimal(3),
                                    OrderDate = reader.GetDateTime(4)
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error loading bill info: " + ex.Message);
                }
            }

            return null;
        }

        public static List<BillItemView> GetBillItems(int billId)
        {
            List<BillItemView> items = new List<BillItemView>();

            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT p.name, bi.quantity, bi.price
                            FROM bill_items bi
                            JOIN products p ON bi.product_id = p.id
                            WHERE bi.bill_id = @BillId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BillId", billId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                items.Add(new BillItemView
                                {
                                    ProductName = reader.GetString(0),
                                    Quantity = reader.GetInt32(1),
                                    Price = reader.GetDecimal(2)
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error loading bill items: " + ex.Message);
                }
            }

            return items;
        }

        public static void DeleteBill(int billId)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();

                    // Delete bill items first
                    string deleteItems = "DELETE FROM bill_items WHERE bill_id = @BillId";
                    using (SqlCommand cmd = new SqlCommand(deleteItems, conn))
                    {
                        cmd.Parameters.AddWithValue("@BillId", billId);
                        cmd.ExecuteNonQuery();
                    }

                    // Delete bill
                    string deleteBill = "DELETE FROM bills WHERE id = @BillId";
                    using (SqlCommand cmd = new SqlCommand(deleteBill, conn))
                    {
                        cmd.Parameters.AddWithValue("@BillId", billId);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error deleting bill: " + ex.Message);
                }
            }
        }

        // ============ PRODUCT MANAGEMENT ============

        public static List<ProductView> GetAllProducts()
        {
            List<ProductView> products = new List<ProductView>();

            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id, name, price, quantity, unit, description FROM products ORDER BY name";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(new ProductView
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Price = reader.GetDecimal(2),
                                    Quantity = reader.GetInt32(3),
                                    Unit = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    Description = reader.IsDBNull(5) ? "" : reader.GetString(5)
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error loading products: " + ex.Message);
                }
            }

            return products;
        }

        public static void AddProduct(string name, decimal price, int quantity, string unit, string description)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"INSERT INTO products (name, price, quantity, unit, description, created_at) 
                           VALUES (@Name, @Price, @Quantity, @Unit, @Description, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.Parameters.AddWithValue("@Unit", unit);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error adding product: " + ex.Message);
                }
            }
        }

        public static void UpdateProduct(int id, string name, decimal price, int quantity, string unit, string description)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"UPDATE products 
                           SET name = @Name, 
                               price = @Price, 
                               quantity = @Quantity, 
                               unit = @Unit, 
                               description = @Description
                           WHERE id = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.Parameters.AddWithValue("@Unit", unit);
                        cmd.Parameters.AddWithValue("@Description", description);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error updating product: " + ex.Message);
                }
            }
        }

        public static void DeleteProduct(int id)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM products WHERE id = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error deleting product: " + ex.Message);
                }
            }
        }
    }
}