using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHang
{
    public class SanPham
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuongTon { get; set; }
        public string DonViTinh { get; set; }

        public SanPham()
        {
        }

        public SanPham(int maSP, string tenSP, decimal giaBan, int soLuongTon, string donViTinh)
        {
            MaSP = maSP;
            TenSP = tenSP;
            GiaBan = giaBan;
            SoLuongTon = soLuongTon;
            DonViTinh = donViTinh;
        }
    }
}