using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace QuanLyCuaHang
{
    public partial class Register : Window
    {
        private bool isPasswordVisible = false;
        private bool isRePasswordVisible = false;

        public Register()
        {
            InitializeComponent();
            txtFullName.Focus();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            PerformRegister();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (isRePasswordVisible)
                    txtRePasswordVisible.Focus();
                else
                    txtRePassword.Focus();
            }
        }

        private void txtPasswordVisible_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (isRePasswordVisible)
                    txtRePasswordVisible.Focus();
                else
                    txtRePassword.Focus();
            }
        }

        private void txtRePassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformRegister();
            }
        }

        private void txtRePasswordVisible_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformRegister();
            }
        }

        private void btnTogglePassword_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                // Show password - icon shows "hide" state
                txtPasswordVisible.Text = txtPassword.Password;
                txtPasswordVisible.Visibility = Visibility.Visible;
                txtPassword.Visibility = Visibility.Collapsed;
                txtToggleIcon.Text = "🙈"; // Eye closed - click to hide
                txtPasswordVisible.Focus();
                txtPasswordVisible.SelectionStart = txtPasswordVisible.Text.Length;
            }
            else
            {
                // Hide password - icon shows "show" state
                txtPassword.Password = txtPasswordVisible.Text;
                txtPassword.Visibility = Visibility.Visible;
                txtPasswordVisible.Visibility = Visibility.Collapsed;
                txtToggleIcon.Text = "👁"; // Eye open - click to show
                txtPassword.Focus();
            }
        }

        private void btnToggleRePassword_Click(object sender, RoutedEventArgs e)
        {
            isRePasswordVisible = !isRePasswordVisible;

            if (isRePasswordVisible)
            {
                // Show password - icon shows "hide" state
                txtRePasswordVisible.Text = txtRePassword.Password;
                txtRePasswordVisible.Visibility = Visibility.Visible;
                txtRePassword.Visibility = Visibility.Collapsed;
                txtToggleReIcon.Text = "🙈"; // Eye closed - click to hide
                txtRePasswordVisible.Focus();
                txtRePasswordVisible.SelectionStart = txtRePasswordVisible.Text.Length;
            }
            else
            {
                // Hide password - icon shows "show" state
                txtRePassword.Password = txtRePasswordVisible.Text;
                txtRePassword.Visibility = Visibility.Visible;
                txtRePasswordVisible.Visibility = Visibility.Collapsed;
                txtToggleReIcon.Text = "👁"; // Eye open - click to show
                txtRePassword.Focus();
            }
        }

        private void btnBackToLogin_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private void PerformRegister()
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = isPasswordVisible ? txtPasswordVisible.Text : txtPassword.Password;
            string rePassword = isRePasswordVisible ? txtRePasswordVisible.Text : txtRePassword.Password;

            // Validation
            if (string.IsNullOrEmpty(fullName))
            {
                ShowError("Please enter your full name!");
                txtFullName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(email))
            {
                ShowError("Please enter your email!");
                txtEmail.Focus();
                return;
            }

            if (!IsValidEmail(email))
            {
                ShowError("Please enter a valid email address!");
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Please enter password!");
                if (isPasswordVisible)
                    txtPasswordVisible.Focus();
                else
                    txtPassword.Focus();
                return;
            }

            if (password.Length < 6)
            {
                ShowError("Password must be at least 6 characters!");
                if (isPasswordVisible)
                    txtPasswordVisible.Focus();
                else
                    txtPassword.Focus();
                return;
            }

            if (string.IsNullOrEmpty(rePassword))
            {
                ShowError("Please confirm your password!");
                if (isRePasswordVisible)
                    txtRePasswordVisible.Focus();
                else
                    txtRePassword.Focus();
                return;
            }

            if (password != rePassword)
            {
                ShowError("Passwords do not match!");
                if (isRePasswordVisible)
                {
                    txtRePasswordVisible.Clear();
                    txtRePasswordVisible.Focus();
                }
                else
                {
                    txtRePassword.Clear();
                    txtRePassword.Focus();
                }
                return;
            }

            try
            {
                // Check if email already exists
                if (DatabaseHelper.EmailExists(email))
                {
                    ShowError("This email is already registered!");
                    txtEmail.Focus();
                    return;
                }

                // Register new employee
                bool success = DatabaseHelper.RegisterEmployee(fullName, email, password);

                if (success)
                {
                    MessageBox.Show(
                        "Registration successful!\nYou can now login with your email and password.",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Redirect to login
                    Login loginWindow = new Login();
                    loginWindow.Show();
                    this.Close();
                }
                else
                {
                    ShowError("Registration failed! Please try again.");
                }
            }
            catch (Exception ex)
            {
                ShowError("Registration error: " + ex.Message);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visibility = Visibility.Visible;
        }
    }
}