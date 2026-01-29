using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuanLyCuaHang
{
    /// <summary>
    /// Interaction logic for DangKy.xaml
    /// </summary>
    public partial class DangKy : Window
    {
        public DangKy()
        {
            InitializeComponent();
        }

        // Nút Đăng ký
        private void BtnDangKy_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đăng ký thành công (demo)!");
        }

        // Nút Quay lại đăng nhập
        private void BtnQuayLai_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // đóng cửa sổ đăng ký
        }
    }
}
