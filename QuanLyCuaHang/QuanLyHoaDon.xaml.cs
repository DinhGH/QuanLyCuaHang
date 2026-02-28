using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyCuaHang
{
    public partial class QuanLyHoaDon : UserControl
    {
        private List<BillView> allOrders;

        public QuanLyHoaDon()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                allOrders = DatabaseHelper.GetAllBills();
                dgOrders.ItemsSource = allOrders;
                UpdateSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading orders: " + ex.Message, "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateSummary()
        {
            if (allOrders != null)
            {
                txtTotalOrders.Text = allOrders.Count.ToString();
                txtTotalRevenue.Text = allOrders.Sum(o => o.TotalAmount).ToString("N2");
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();
            
            if (string.IsNullOrWhiteSpace(searchText))
            {
                dgOrders.ItemsSource = allOrders;
            }
            else
            {
                var filtered = allOrders.Where(o =>
                    o.CustomerName.ToLower().Contains(searchText) ||
                    o.EmployeeName.ToLower().Contains(searchText) ||
                    o.Id.ToString().Contains(searchText)
                ).ToList();
                
                dgOrders.ItemsSource = filtered;
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
            txtSearch.Clear();
        }

        private void dgOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection if needed
        }

        private void btnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int billId = (int)btn.Tag;
                ChiTietHoaDonWindow detailWindow = new ChiTietHoaDonWindow(billId);
                detailWindow.Owner = Window.GetWindow(this);
                detailWindow.ShowDialog();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int billId = (int)btn.Tag;
                
                MessageBoxResult result = MessageBox.Show(
                    "Are you sure you want to delete this order?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        DatabaseHelper.DeleteBill(billId);
                        MessageBox.Show("Order deleted successfully!", "Success", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadOrders();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting order: " + ex.Message, "Error", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
