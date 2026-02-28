using System;
using System.ComponentModel;

namespace QuanLyCuaHang
{
    public class ChiTietHoaDon : INotifyPropertyChanged
    {
        private int maSP;
        private string tenSP;
        private decimal donGia;
        private int soLuong;

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
                OnPropertyChanged(nameof(ThanhTien)); // C?p nh?t ThanhTien khi DonGia thay ??i
            }
        }

        public int SoLuong
        {
            get => soLuong;
            set
            {
                soLuong = value;
                OnPropertyChanged(nameof(SoLuong));
                OnPropertyChanged(nameof(ThanhTien)); // C?p nh?t ThanhTien khi SoLuong thay ??i
            }
        }

        // Read-only calculated property
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