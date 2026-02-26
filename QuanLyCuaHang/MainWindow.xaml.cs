using System;
using System.Windows;
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

        private void btnPayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ThanhToan paymentWindow = new ThanhToan();
                paymentWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening Payment window: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnOrderManagement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                QuanLyHoaDon orderWindow = new QuanLyHoaDon();
                orderWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening Order Management window: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnProductManagement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                QuanLySanPham productWindow = new QuanLySanPham();
                productWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening Product Management window: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
