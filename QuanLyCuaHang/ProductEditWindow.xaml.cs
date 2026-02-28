using System;
using System.Windows;

namespace QuanLyCuaHang
{
    public partial class ProductEditWindow : Window
    {
        private ProductView currentProduct;
        private bool isEditMode = false;

        public ProductEditWindow()
        {
            InitializeComponent();
            txtTitle.Text = "ADD NEW PRODUCT";
            txtName.Focus();
            
            // Set default unit
            cboUnit.SelectedIndex = 0;
        }

        public ProductEditWindow(ProductView product) : this()
        {
            isEditMode = true;
            currentProduct = product;
            txtTitle.Text = "EDIT PRODUCT";
            
            // Show product ID panel in edit mode
            pnlProductId.Visibility = Visibility.Visible;
            txtProductId.Text = $"#{product.Id}";
            
            // Load data
            txtName.Text = product.Name;
            txtPrice.Text = product.Price.ToString("F2");
            txtQuantity.Text = product.Quantity.ToString();
            cboUnit.Text = product.Unit;
            txtDescription.Text = product.Description;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate Product Name
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter product name!", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return;
            }

            // Validate Price
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid price greater than 0!", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPrice.Focus();
                return;
            }

            // Validate Quantity
            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Please enter a valid quantity (0 or greater)!", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtQuantity.Focus();
                return;
            }

            // Get unit (use default if empty)
            string unit = string.IsNullOrWhiteSpace(cboUnit.Text) ? "pcs" : cboUnit.Text.Trim();

            try
            {
                if (isEditMode)
                {
                    // Update existing product
                    DatabaseHelper.UpdateProduct(
                        currentProduct.Id,
                        txtName.Text.Trim(),
                        price,
                        quantity,
                        unit,
                        txtDescription.Text.Trim()
                    );
                    
                    MessageBox.Show($"Product '{txtName.Text}' updated successfully!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Add new product
                    DatabaseHelper.AddProduct(
                        txtName.Text.Trim(),
                        price,
                        quantity,
                        unit,
                        txtDescription.Text.Trim()
                    );
                    
                    MessageBox.Show($"Product '{txtName.Text}' added successfully!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}