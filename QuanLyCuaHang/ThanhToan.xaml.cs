using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyCuaHang
{
    public partial class ThanhToan : Window
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
                MessageBox.Show("L?i khi t?i danh sách s?n ph?m: " + ex.Message, "L?i", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnThemSanPham_Click(object sender, RoutedEventArgs e)
        {
            // T?o window ch?n s?n ph?m
            var windowChonSP = new Window
            {
                Title = "Ch?n S?n Ph?m",
                Width = 500,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            var grid = new Grid { Margin = new Thickness(10) };
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var listView = new ListView { Margin = new Thickness(0, 0, 0, 10) };
            listView.ItemTemplate = (DataTemplate)this.FindResource("SanPhamItemTemplate") ?? CreateSanPhamTemplate();
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
                Content = "Ch?n",
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
            
            var txtTen = new FrameworkElementFactory(typeof(TextBlock));
            txtTen.SetValue(TextBlock.FontWeightProperty, System.Windows.FontWeights.SemiBold);
            txtTen.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("TenSP"));
            
            var txtGia = new FrameworkElementFactory(typeof(TextBlock));
            txtGia.SetValue(TextBlock.ForegroundProperty, System.Windows.Media.Brushes.Gray);
            txtGia.SetBinding(TextBlock.TextProperty, 
                new System.Windows.Data.Binding("GiaBan") { StringFormat = "Giá: {0:N0} VN?" });
            
            factory.AppendChild(txtTen);
            factory.AppendChild(txtGia);
            template.VisualTree = factory;
            
            return template;
        }

        private void ThemSanPhamVaoGio(SanPham sp)
        {
            var existingItem = danhSachMuaHang.FirstOrDefault(x => x.MaSP == sp.MaSP);
            
            if (existingItem != null)
            {
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
            txtTongTien.Text = tongTien.ToString("N0");
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
                    txtTienThua.Text = "0";
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
                txtTienThua.Text = "0";
                return;
            }

            if (decimal.TryParse(txtTongTien.Text.Replace(",", ""), out decimal tongTien) &&
                decimal.TryParse(txtTienKhachDua.Text.Replace(",", ""), out decimal tienKhachDua))
            {
                decimal tienThua = tienKhachDua - tongTien;
                txtTienThua.Text = tienThua >= 0 ? tienThua.ToString("N0") : "0";
            }
            else
            {
                txtTienThua.Text = "0";
            }
        }

        private void btnXuatHoaDon_Click(object sender, RoutedEventArgs e)
        {
            if (danhSachMuaHang.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm s?n ph?m vào gi? hàng!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (rdTienMat.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(txtTienKhachDua.Text) || 
                    !decimal.TryParse(txtTienKhachDua.Text.Replace(",", ""), out decimal tienKhachDua))
                {
                    MessageBox.Show("Vui lòng nh?p ti?n khách ??a!", "Thông báo", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (decimal.TryParse(txtTongTien.Text.Replace(",", ""), out decimal tongTien))
                {
                    if (tienKhachDua < tongTien)
                    {
                        MessageBox.Show("Ti?n khách ??a không ??!", "Thông báo", 
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            try
            {
                // T?o hóa ??n
                HoaDon hoaDon = new HoaDon
                {
                    MaHoaDon = DatabaseHelper.TaoMaHoaDon(),
                    KhachHang = string.IsNullOrWhiteSpace(txtHoTen.Text) ? "Khách l?" : txtHoTen.Text,
                    SoDienThoai = txtSDT.Text,
                    DiaChi = txtDiaChi.Text,
                    NgayLap = DateTime.Now,
                    TongTien = double.Parse(txtTongTien.Text.Replace(",", "")),
                    HinhThucThanhToan = rdTienMat.IsChecked == true ? "Ti?n m?t" : "Chuy?n kho?n",
                    TienKhachDua = string.IsNullOrWhiteSpace(txtTienKhachDua.Text) ? 0 : 
                        double.Parse(txtTienKhachDua.Text.Replace(",", "")),
                    TienThua = double.Parse(txtTienThua.Text.Replace(",", ""))
                };

                // L?u vào database
                int maHD = DatabaseHelper.LuuHoaDon(hoaDon);
                DatabaseHelper.LuuChiTietHoaDon(maHD, danhSachMuaHang);

                MessageBox.Show($"Xu?t hóa ??n thành công!\nMã hóa ??n: {hoaDon.MaHoaDon}", 
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Reset form
                danhSachMuaHang.Clear();
                CapNhatDanhSachHienThi();
                CapNhatTongTien();
                txtHoTen.Clear();
                txtSDT.Clear();
                txtDiaChi.Clear();
                txtTienKhachDua.Clear();
                rdTienMat.IsChecked = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("L?i khi xu?t hóa ??n: " + ex.Message, "L?i", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}