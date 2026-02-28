using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace QuanLyCuaHang
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Employee currentEmployee;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
        }

        public MainWindow(Employee employee) : this()
        {
            currentEmployee = employee;
            txtWelcome.Text = $"Welcome, {employee.FullName} ({employee.Role})";
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            txtDateTime.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy - HH:mm:ss");
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            LoadContent(new Dashboard());
        }

        private void btnPayment_Click(object sender, RoutedEventArgs e)
        {
            LoadContent(new ThanhToan());
        }

        private void btnOrderManagement_Click(object sender, RoutedEventArgs e)
        {
            LoadContent(new QuanLyHoaDon());
        }

        private void btnProductManagement_Click(object sender, RoutedEventArgs e)
        {
            LoadContent(new QuanLySanPham());
        }

        private void LoadContent(UserControl userControl)
        {
            WelcomePanel.Visibility = Visibility.Collapsed;
            MainContentControl.Visibility = Visibility.Visible;
            MainContentControl.Content = userControl;
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Confirm Logout",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                timer.Stop();
                Login loginWindow = new Login();
                loginWindow.Show();
                this.Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            timer?.Stop();
            base.OnClosed(e);
        }
    }
}
