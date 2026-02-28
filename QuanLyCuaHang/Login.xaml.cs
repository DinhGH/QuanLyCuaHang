using System;
using System.Windows;
using System.Windows.Input;

namespace QuanLyCuaHang
{
    public partial class Login : Window
    {
        private bool isPasswordVisible = false;

        public Login()
        {
            InitializeComponent();
            txtUsername.Focus();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            PerformLogin();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformLogin();
            }
        }

        private void txtPasswordVisible_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformLogin();
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

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            Register registerWindow = new Register();
            registerWindow.Show();
            this.Close();
        }

        private void PerformLogin()
        {
            string username = txtUsername.Text.Trim();
            string password = isPasswordVisible ? txtPasswordVisible.Text : txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Please enter email and password!");
                return;
            }

            try
            {
                // Validate login from database
                var employee = DatabaseHelper.ValidateLogin(username, password);

                if (employee != null)
                {
                    // Login successful
                    MainWindow mainWindow = new MainWindow(employee);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    ShowError("Invalid email or password!");
                    if (isPasswordVisible)
                    {
                        txtPasswordVisible.Clear();
                    }
                    else
                    {
                        txtPassword.Clear();
                    }
                    txtUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError("Login error: " + ex.Message);
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visibility = Visibility.Visible;
        }
    }
}