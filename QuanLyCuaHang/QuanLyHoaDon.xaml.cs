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
    /// Interaction logic for QuanLyHoaDon.xaml
    /// </summary>
    public partial class QuanLyHoaDon : Window
    {
        public QuanLyHoaDon()
        {
            InitializeComponent();
            loadData();
        }
        public void loadData()
        {
            List<HoaDon> dsHoaDon = new List<HoaDon>()
            {
               new HoaDon(1,"HD001","Ngo Huu Thuan",DateTime.Now,69000),
               new HoaDon(2,"HD002","Tran Dieu Huyen",DateTime.Now,79000),
               new HoaDon(3,"HD003","Tran Cong Danh",DateTime.Now,88000),
               new HoaDon(4,"HD004","Nguyen Duy Quy",DateTime.Now,99000),
               new HoaDon(5,"HD005","Huynh Tan Dinh",DateTime.Now,34000),

            };
            dt_hoadon.ItemsSource = dsHoaDon;
        }
    }
}
