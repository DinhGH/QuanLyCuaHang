using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyCuaHang
{
    public partial class ThanhToan : UserControl
    {
        private List<ChiTietHoaDon> danhSachMuaHang = new List<ChiTietHoaDon>();
        private List<SanPham> danhSachSanPham = new List<SanPham>();

        public ThanhToan()
        {
            InitializeComponent();
            LoadDanhSachSanPham();
            CapNhatTongTien();
        }

        private void LoadDanhSachSanPham()
        {
            try
            {
                danhSachSanPham = DatabaseHelper.GetAllSanPham();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message, "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnThemSanPham_Click(object sender, RoutedEventArgs e)
        {
            // Create product selection window
            var windowChonSP = new Window
            {
                Title = "Select Product",
                Width = 500,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this)
            };

            var grid = new Grid { Margin = new Thickness(10) };
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Create ListView with template
            var listView = new ListView { Margin = new Thickness(0, 0, 0, 10) };
            listView.ItemTemplate = CreateSanPhamTemplate();
            listView.ItemsSource = danhSachSanPham;
            
            listView.MouseDoubleClick += (s, args) =>
            {
                if (listView.SelectedItem is SanPham sp)
                {
                    ThemSanPhamVaoGio(sp);
                    windowChonSP.Close();
                }
            };

            var btnChon = new Button
            {
                Content = "Select",
                Width = 100,
                Height = 35,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            btnChon.Click += (s, args) =>
            {
                if (listView.SelectedItem is SanPham sp)
                {
                    ThemSanPhamVaoGio(sp);
                    windowChonSP.Close();
                }
                else
                {
                    MessageBox.Show("Please select a product!", "Warning", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };

            Grid.SetRow(listView, 0);
            Grid.SetRow(btnChon, 1);
            grid.Children.Add(listView);
            grid.Children.Add(btnChon);
            windowChonSP.Content = grid;

            windowChonSP.ShowDialog();
        }

        private DataTemplate CreateSanPhamTemplate()
        {
            var template = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(StackPanel));
            factory.SetValue(StackPanel.MarginProperty, new Thickness(5));
            
            var txtTen = new FrameworkElementFactory(typeof(TextBlock));
            txtTen.SetValue(TextBlock.FontWeightProperty, System.Windows.FontWeights.SemiBold);
            txtTen.SetValue(TextBlock.FontSizeProperty, 14.0);
            txtTen.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("TenSP"));
            
            var txtGia = new FrameworkElementFactory(typeof(TextBlock));
            txtGia.SetValue(TextBlock.ForegroundProperty, System.Windows.Media.Brushes.Gray);
            txtGia.SetValue(TextBlock.FontSizeProperty, 12.0);
            txtGia.SetBinding(TextBlock.TextProperty, 
                new System.Windows.Data.Binding("GiaBan") { StringFormat = "Price: ${0:N2}" });
            
            var txtSoLuong = new FrameworkElementFactory(typeof(TextBlock));
            txtSoLuong.SetValue(TextBlock.ForegroundProperty, System.Windows.Media.Brushes.DarkGreen);
            txtSoLuong.SetValue(TextBlock.FontSizeProperty, 11.0);
            txtSoLuong.SetBinding(TextBlock.TextProperty, 
                new System.Windows.Data.Binding("SoLuongTon") { StringFormat = "In stock: {0}" });
            
            factory.AppendChild(txtTen);
            factory.AppendChild(txtGia);
            factory.AppendChild(txtSoLuong);
            template.VisualTree = factory;
            
            return template;
        }

        private void ThemSanPhamVaoGio(SanPham sp)
        {
            // Check stock
            if (sp.SoLuongTon <= 0)
            {
                MessageBox.Show("This product is out of stock!", "Warning", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existingItem = danhSachMuaHang.FirstOrDefault(x => x.MaSP == sp.MaSP);
            
            if (existingItem != null)
            {
                // Check if adding more exceeds stock
                if (existingItem.SoLuong + 1 > sp.SoLuongTon)
                {
                    MessageBox.Show($"Not enough stock! Available: {sp.SoLuongTon}", "Warning", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                existingItem.SoLuong++;
            }
            else
            {
                danhSachMuaHang.Add(new ChiTietHoaDon(sp.MaSP, sp.TenSP, sp.GiaBan, 1));
            }
            
            CapNhatDanhSachHienThi();
            CapNhatTongTien();
        }

        private void btnTangSoLuong_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ChiTietHoaDon item)
            {
                // Check stock before increasing
                var sanPham = danhSachSanPham.FirstOrDefault(x => x.MaSP == item.MaSP);
                if (sanPham != null && item.SoLuong + 1 > sanPham.SoLuongTon)
                {
                    MessageBox.Show($"Not enough stock! Available: {sanPham.SoLuongTon}", "Warning", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                item.SoLuong++;
                CapNhatDanhSachHienThi();
                CapNhatTongTien();
            }
        }

        private void btnGiamSoLuong_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ChiTietHoaDon item)
            {
                if (item.SoLuong > 1)
                {
                    item.SoLuong--;
                }
                else
                {
                    danhSachMuaHang.Remove(item);
                }
                
                CapNhatDanhSachHienThi();
                CapNhatTongTien();
            }
        }

        private void CapNhatDanhSachHienThi()
        {
            lvSanPham.ItemsSource = null;
            lvSanPham.ItemsSource = danhSachMuaHang;
        }

        private void CapNhatTongTien()
        {
            decimal tongTien = danhSachMuaHang.Sum(x => x.ThanhTien);
            txtTongTien.Text = tongTien.ToString("N2");
            TinhTienThua();
        }

        private void txtTienKhachDua_TextChanged(object sender, TextChangedEventArgs e)
        {
            TinhTienThua();
        }

        private void rdThanhToan_Checked(object sender, RoutedEventArgs e)
        {
            if (rdChuyenKhoan != null && rdChuyenKhoan.IsChecked == true)
            {
                if (txtTienKhachDua != null)
                    txtTienKhachDua.IsEnabled = false;
                if (txtTienThua != null)
                    txtTienThua.Text = "0.00";
            }
            else
            {
                if (txtTienKhachDua != null)
                    txtTienKhachDua.IsEnabled = true;
                TinhTienThua();
            }
        }

        private void TinhTienThua()
        {
            if (txtTienThua == null || txtTongTien == null || txtTienKhachDua == null)
                return;

            if (rdChuyenKhoan?.IsChecked == true)
            {
                txtTienThua.Text = "0.00";
                return;
            }

            if (decimal.TryParse(txtTongTien.Text.Replace(",", ""), out decimal tongTien) &&
                decimal.TryParse(txtTienKhachDua.Text.Replace(",", ""), out decimal tienKhachDua))
            {
                decimal tienThua = tienKhachDua - tongTien;
                txtTienThua.Text = tienThua >= 0 ? tienThua.ToString("N2") : "0.00";
            }
            else
            {
                txtTienThua.Text = "0.00";
            }
        }

        private void btnXuatHoaDon_Click(object sender, RoutedEventArgs e)
        {
            if (danhSachMuaHang.Count == 0)
            {
                MessageBox.Show("Please add products to cart!", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (rdTienMat.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(txtTienKhachDua.Text) ||
                    !decimal.TryParse(txtTienKhachDua.Text.Replace(",", ""), out decimal tienKhachDua))
                {
                    MessageBox.Show("Please enter cash received!", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (decimal.TryParse(txtTongTien.Text.Replace(",", ""), out decimal tongTien))
                {
                    if (tienKhachDua < tongTien)
                    {
                        MessageBox.Show("Cash received is not enough!", "Warning",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            try
            {
                HoaDon hoaDon = new HoaDon
                {
                    MaHoaDon = DatabaseHelper.TaoMaHoaDon(),
                    KhachHang = string.IsNullOrWhiteSpace(txtHoTen.Text) ? "Guest" : txtHoTen.Text,
                    SoDienThoai = txtSDT.Text,
                    DiaChi = txtDiaChi.Text,
                    NgayLap = DateTime.Now,
                    TongTien = double.Parse(txtTongTien.Text.Replace(",", "")),
                    HinhThucThanhToan = rdTienMat.IsChecked == true ? "Cash" : "Credit Card",
                    TienKhachDua = string.IsNullOrWhiteSpace(txtTienKhachDua.Text) ? 0 :
                        double.Parse(txtTienKhachDua.Text.Replace(",", "")),
                    TienThua = double.Parse(txtTienThua.Text.Replace(",", ""))
                };

                int maHD = DatabaseHelper.LuuHoaDon(hoaDon);
                DatabaseHelper.LuuChiTietHoaDon(maHD, danhSachMuaHang);

                MessageBox.Show($"Checkout successful!\nOrder ID: {hoaDon.MaHoaDon}",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Clear the cart after successful checkout
                danhSachMuaHang.Clear();
                CapNhatDanhSachHienThi();
                CapNhatTongTien();
                
                // Clear customer information
                txtHoTen.Clear();
                txtSDT.Clear();
                txtDiaChi.Clear();
                txtTienKhachDua.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Checkout error: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}