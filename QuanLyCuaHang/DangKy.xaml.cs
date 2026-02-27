using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using QuanLyCuaHang.Models;

namespace QuanLyCuaHang
{
    public partial class DangKy : Window
    {
        public DangKy()
        {
            InitializeComponent();
        }

        // ===== CLICK ĐĂNG KÝ =====
        private void BtnDangKy_Click(object sender, RoutedEventArgs e)
        {
            string email = txtUsername.Text.Trim();
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;
            string role = (cbRole.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();

            // 1️⃣ Validate
            if (string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirmPassword) ||
                string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new QLCHDBEntities()) // TÊN CONTEXT EF
                {
                    // 2️⃣ Check trùng email
                    bool exists = db.Users.Any(u => u.email == email);
                    if (exists)
                    {
                        MessageBox.Show("Email đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // 3️⃣ Tạo user mới
                    User user = new User
                    {
                        email = email,
                        password = HashPassword(password),
                        role = role,
                        full_name = email, // tạm, có thể chỉnh sau
                        salary = 0,
                        created_at = DateTime.Now
                    };

                    db.Users.Add(user);
                    db.SaveChanges();
                }

                MessageBox.Show("Đăng ký thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // 4️⃣ Quay về Login
                Login login = new Login();
                login.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ===== HASH PASSWORD =====
        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        // ===== BACK TO LOGIN =====
        private void TxtBackToLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }


    }
}