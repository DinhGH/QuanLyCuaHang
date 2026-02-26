using System.Windows;
using QuanLyCuaHang.Models;
using QuanLyCuaHang.Services;

namespace QuanLyCuaHang
{
    public partial class Login : Window
    {
        private bool isPasswordVisible = false;

        public Login()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;
            btnTogglePassword.Click += BtnTogglePassword_Click;
            txtPasswordVisible.TextChanged += TxtPasswordVisible_TextChanged;
        }

        /// <summary>
        /// Xử lý nút toggle hiển thị/ẩn mật khẩu
        /// </summary>
        private void BtnTogglePassword_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                // Hiển thị mật khẩu
                txtPasswordVisible.Text = txtPassword.Password;
                txtPassword.Visibility = Visibility.Collapsed;
                txtPasswordVisible.Visibility = Visibility.Visible;
                eyeIcon.Text = "🙈";
            }
            else
            {
                // Ẩn mật khẩu
                txtPassword.Password = txtPasswordVisible.Text;
                txtPassword.Visibility = Visibility.Visible;
                txtPasswordVisible.Visibility = Visibility.Collapsed;
                eyeIcon.Text = "👁";
            }
        }

        /// <summary>
        /// Đồng bộ text từ TextBox hiển thị sang PasswordBox
        /// </summary>
        private void TxtPasswordVisible_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (isPasswordVisible)
            {
                txtPassword.Password = txtPasswordVisible.Text;
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtUsername.Text.Trim();
            string password = isPasswordVisible ? txtPasswordVisible.Text : txtPassword.Password;

            // Validate dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(email))
            {
                HienThiBaoLoi("Vui lòng nhập email");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                HienThiBaoLoi("Vui lòng nhập mật khẩu");
                if (isPasswordVisible)
                    txtPasswordVisible.Focus();
                else
                    txtPassword.Focus();
                return;
            }

            // Xác thực đăng nhập từ database
            User user = AuthService.Login(email, password);

            if (user != null)
            {
                AnBaoLoi();

                // Lưu thông tin người dùng hiện tại
                App.CurrentUser = user;

                // Mở cửa sổ chính
                QuanLyHoaDon mainWindow = new QuanLyHoaDon();
                mainWindow.Show();

                this.Close();
            }
            else
            {
                HienThiBaoLoi("Email hoặc mật khẩu không đúng");
                if (isPasswordVisible)
                    txtPasswordVisible.Clear();
                else
                    txtPassword.Clear();
                txtUsername.Focus();
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng đăng ký chưa được phát triển", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void HienThiBaoLoi(string message)
        {
            lblError.Text = message;
            lblError.Visibility = Visibility.Visible;
        }

        private void AnBaoLoi()
        {
            lblError.Visibility = Visibility.Collapsed;
            lblError.Text = "";
        }
    }
}