using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyCuaHang
{
    public partial class QuanLySanPham : UserControl
    {
        private List<ProductView> allProducts;

        public QuanLySanPham()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                allProducts = DatabaseHelper.GetAllProducts();
                dgProducts.ItemsSource = allProducts;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message, "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();
            
            if (string.IsNullOrWhiteSpace(searchText))
            {
                dgProducts.ItemsSource = allProducts;
            }
            else
            {
                var filtered = allProducts.Where(p =>
                    p.Name.ToLower().Contains(searchText) ||
                    p.Description.ToLower().Contains(searchText) ||
                    p.Id.ToString().Contains(searchText)
                ).ToList();
                
                dgProducts.ItemsSource = filtered;
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts();
            txtSearch.Clear();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ProductEditWindow editWindow = new ProductEditWindow();
            editWindow.Owner = Window.GetWindow(this);
            if (editWindow.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag is ProductView product)
            {
                ProductEditWindow editWindow = new ProductEditWindow(product);
                editWindow.Owner = Window.GetWindow(this);
                if (editWindow.ShowDialog() == true)
                {
                    LoadProducts();
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int productId = (int)btn.Tag;
                
                MessageBoxResult result = MessageBox.Show(
                    "Are you sure you want to delete this product?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        DatabaseHelper.DeleteProduct(productId);
                        MessageBox.Show("Product deleted successfully!", "Success", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadProducts();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting product: " + ex.Message, "Error", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
