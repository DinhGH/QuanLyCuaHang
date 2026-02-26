using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHang
{
    internal class HoaDon
    {
        int id;
        int stt;
        string maHoaDon, khachHang;
        DateTime? ngayLap;
        decimal tongTien;

        public HoaDon()
        {

        }
        public HoaDon(int stt, string maHoaDon, string khachHang, DateTime? ngayLap, decimal tongTien)
        {
            this.stt = stt;
            this.maHoaDon = maHoaDon;
            this.khachHang = khachHang;
            this.ngayLap = ngayLap;
            this.tongTien = tongTien;
        }

        public int Stt { get => stt; set => stt = value; }
        public int Id { get => id; set => id = value; }
        public string MaHoaDon { get => maHoaDon; set => maHoaDon = value; }
        public string KhachHang { get => khachHang; set => khachHang = value; }
        public DateTime? NgayLap { get => ngayLap; set => ngayLap = value; }
        public decimal TongTien { get => tongTien; set => tongTien = value; }
    }
}
