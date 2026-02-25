using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyCuaHang
{
    public partial class QuanLySanPham : Window
    {
        private List<ProductRow> _allProducts = new List<ProductRow>();

        public QuanLySanPham()
        {
            InitializeComponent();
            loadData();
        }

        private void loadData()
        {
            using (var context = new QuanLyCuaHangEntities1())
            {
                _allProducts = context.products
                    .OrderBy(p => p.name)
                    .Select(p => new ProductRow
                    {
                        Id = p.id,
                        TenSP = p.name,
                        LoaiSP = p.unit,
                        Gia = p.price,
                        SoLuong = p.quantity
                    })
                    .ToList();

                dgDanhSach.ItemsSource = _allProducts;

                var loai = context.products
                    .Select(p => p.unit)
                    .Where(u => u != null && u != "")
                    .Distinct()
                    .OrderBy(u => u)
                    .ToList();

                cbLoai.ItemsSource = loai;
            }
        }

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            if (!TryGetFormValues(out var ten, out var loai, out var gia, out var soLuong))
                return;

            using (var context = new QuanLyCuaHangEntities1())
            {
                var entity = new product
                {
                    name = ten,
                    unit = loai,
                    price = gia,
                    quantity = soLuong,
                    created_at = DateTime.Now
                };

                context.products.Add(entity);
                context.SaveChanges();
            }

            loadData();
            ClearForm();
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgDanhSach.SelectedItem is ProductRow row))
            {
                MessageBox.Show("Chọn sản phẩm cần sửa.");
                return;
            }

            if (!TryGetFormValues(out var ten, out var loai, out var gia, out var soLuong))
                return;

            using (var context = new QuanLyCuaHangEntities1())
            {
                var entity = context.products.FirstOrDefault(p => p.id == row.Id);
                if (entity == null)
                {
                    MessageBox.Show("Sản phẩm không tồn tại.");
                    return;
                }

                entity.name = ten;
                entity.unit = loai;
                entity.price = gia;
                entity.quantity = soLuong;
                context.SaveChanges();
            }

            loadData();
            ClearForm();
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgDanhSach.SelectedItem is ProductRow row))
            {
                MessageBox.Show("Chọn sản phẩm cần xóa.");
                return;
            }

            var result = MessageBox.Show("Xác nhận xóa sản phẩm?", "Xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                using (var context = new QuanLyCuaHangEntities1())
                {
                    var entity = context.products.FirstOrDefault(p => p.id == row.Id);
                    if (entity == null)
                    {
                        MessageBox.Show("Sản phẩm không tồn tại.");
                        return;
                    }

                    context.products.Remove(entity);
                    context.SaveChanges();
                }

                loadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa sản phẩm: " + ex.Message);
            }
        }

        private void txtTimKiem_TextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = txtTimKiem.Text?.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                dgDanhSach.ItemsSource = _allProducts;
                return;
            }

            var filtered = _allProducts
                .Where(p =>
                    (!string.IsNullOrEmpty(p.TenSP) && p.TenSP.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(p.LoaiSP) && p.LoaiSP.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0))
                .ToList();

            dgDanhSach.ItemsSource = filtered;
        }

        private void dgDanhSach_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDanhSach.SelectedItem is ProductRow row)
            {
                txtTen.Text = row.TenSP;
                cbLoai.Text = row.LoaiSP;
                txtGia.Text = row.Gia.ToString();
                txtSoLuong.Text = row.SoLuong.ToString();
            }
        }

        private bool TryGetFormValues(out string ten, out string loai, out decimal gia, out int soLuong)
        {
            ten = txtTen.Text?.Trim();
            loai = cbLoai.Text?.Trim();

            if (string.IsNullOrWhiteSpace(ten))
            {
                MessageBox.Show("Tên sản phẩm không được để trống.");
                gia = 0;
                soLuong = 0;
                return false;
            }

            if (!decimal.TryParse(txtGia.Text, out gia))
            {
                MessageBox.Show("Giá không hợp lệ.");
                soLuong = 0;
                return false;
            }

            if (!int.TryParse(txtSoLuong.Text, out soLuong))
            {
                MessageBox.Show("Số lượng không hợp lệ.");
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            txtTen.Clear();
            cbLoai.Text = string.Empty;
            txtGia.Clear();
            txtSoLuong.Clear();
            dgDanhSach.SelectedItem = null;
        }

        private class ProductRow
        {
            public int Id { get; set; }
            public string TenSP { get; set; }
            public string LoaiSP { get; set; }
            public decimal Gia { get; set; }
            public int SoLuong { get; set; }
        }
    }
}
