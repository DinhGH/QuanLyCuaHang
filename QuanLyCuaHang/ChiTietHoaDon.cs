using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHang
{
    public class ChiTietHoaDon : INotifyPropertyChanged
    {
        private int maCTHD;
        private int maHD;
        private int maSP;
        private string tenSP;
        private decimal donGia;
        private int soLuong;

        public int MaCTHD
        {
            get => maCTHD;
            set
            {
                maCTHD = value;
                OnPropertyChanged(nameof(MaCTHD));
            }
        }

        public int MaHD
        {
            get => maHD;
            set
            {
                maHD = value;
                OnPropertyChanged(nameof(MaHD));
            }
        }

        public int MaSP
        {
            get => maSP;
            set
            {
                maSP = value;
                OnPropertyChanged(nameof(MaSP));
            }
        }

        public string TenSP
        {
            get => tenSP;
            set
            {
                tenSP = value;
                OnPropertyChanged(nameof(TenSP));
            }
        }

        public decimal DonGia
        {
            get => donGia;
            set
            {
                donGia = value;
                OnPropertyChanged(nameof(DonGia));
                OnPropertyChanged(nameof(ThanhTien));
            }
        }

        public int SoLuong
        {
            get => soLuong;
            set
            {
                soLuong = value;
                OnPropertyChanged(nameof(SoLuong));
                OnPropertyChanged(nameof(ThanhTien));
            }
        }

        public decimal ThanhTien => DonGia * SoLuong;

        public ChiTietHoaDon()
        {
        }

        public ChiTietHoaDon(int maSP, string tenSP, decimal donGia, int soLuong)
        {
            MaSP = maSP;
            TenSP = tenSP;
            DonGia = donGia;
            SoLuong = soLuong;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}