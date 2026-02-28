using System;
using System.Linq;
using System.Windows.Controls;

namespace QuanLyCuaHang
{
    public partial class Dashboard : UserControl
    {
        public Dashboard()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                // Load all bills
                var allBills = DatabaseHelper.GetAllBills();
                
                // Load all products
                var allProducts = DatabaseHelper.GetAllProducts();

                // Calculate statistics
                decimal totalRevenue = allBills.Sum(b => b.TotalAmount);
                int totalOrders = allBills.Count;
                int totalProducts = allProducts.Count;
                int lowStockCount = allProducts.Count(p => p.Quantity < 10);

                // Update statistics cards
                txtTotalRevenue.Text = "$" + totalRevenue.ToString("N2");
                txtTotalOrders.Text = totalOrders.ToString();
                txtTotalProducts.Text = totalProducts.ToString();
                txtLowStock.Text = lowStockCount.ToString();

                // Today's statistics
                var today = DateTime.Today;
                var todayBills = allBills.Where(b => b.OrderDate.Date == today).ToList();
                decimal todayRevenue = todayBills.Sum(b => b.TotalAmount);
                int todayOrders = todayBills.Count;
                decimal avgOrderValue = todayOrders > 0 ? todayRevenue / todayOrders : 0;

                txtTodayRevenue.Text = "$" + todayRevenue.ToString("N2");
                txtTodayOrders.Text = todayOrders.ToString();
                txtAvgOrderValue.Text = "$" + avgOrderValue.ToString("N2");

                // Recent orders (last 5)
                dgRecentOrders.ItemsSource = allBills.Take(5).ToList();

                // Low stock products
                dgLowStockProducts.ItemsSource = allProducts
                    .Where(p => p.Quantity < 10)
                    .OrderBy(p => p.Quantity)
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error loading dashboard data: " + ex.Message, 
                    "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}