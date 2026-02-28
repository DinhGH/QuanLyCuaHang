using System;

namespace QuanLyCuaHang
{
    public class BillView
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
    }

    public class BillItemView
    {
        public int RowNumber { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal => Quantity * Price;
    }

    public class ProductView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
    }
}