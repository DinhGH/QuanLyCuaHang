using System;
using System.Collections.Generic;
using System.Windows;

namespace QuanLyCuaHang
{
    public partial class ChiTietHoaDonWindow : Window
    {
        private int billId;

        public ChiTietHoaDonWindow(int billId)
        {
            InitializeComponent();
            this.billId = billId;
            LoadBillDetails();
        }

        private void LoadBillDetails()
        {
            try
            {
                var billInfo = DatabaseHelper.GetBillInfo(billId);
                var billItems = DatabaseHelper.GetBillItems(billId);

                if (billInfo != null)
                {
                    // Display bill info
                    txtOrderId.Text = billInfo.Id.ToString();
                    txtCustomer.Text = billInfo.CustomerName ?? "Guest";
                    txtEmployee.Text = billInfo.EmployeeName;
                    txtDate.Text = billInfo.OrderDate.ToString("MM/dd/yyyy HH:mm");
                    txtTotal.Text = "$" + billInfo.TotalAmount.ToString("N2");

                    // Display items
                    int rowNum = 1;
                    foreach (var item in billItems)
                    {
                        item.RowNumber = rowNum++;
                    }
                    dgItems.ItemsSource = billItems;
                }
                else
                {
                    MessageBox.Show("Order not found!", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading bill details: " + ex.Message, "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}