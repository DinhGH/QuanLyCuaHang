using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using QuanLyCuaHang.Models;

namespace QuanLyCuaHang
{
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        // ===== REGISTER CLICK =====
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            // Default role (UI does not have role selection)
            string role = "Staff";

            // 1️⃣ Validation
            if (string.IsNullOrEmpty(fullName) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Invalid email format.", "Validation Error",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new QLCHDBEntities())
                {
                    // 2️⃣ Check duplicate email
                    bool emailExists = db.Users.Any(u => u.email == email);
                    if (emailExists)
                    {
                        MessageBox.Show("This email is already registered.", "Error",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // 3️⃣ Create new user
                    User newUser = new User
                    {
                        full_name = fullName,
                        email = email,
                        password = HashPassword(password),
                        role = role,
                        salary = 0,
                        created_at = DateTime.Now
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();
                }

                MessageBox.Show("Registration successful!", "Success",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                // 4️⃣ Back to Login
                Login login = new Login();
                login.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("System error:\n" + ex.Message, "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ===== HASH PASSWORD =====
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
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