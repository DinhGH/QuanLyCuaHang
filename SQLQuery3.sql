-- Script thêm dữ liệu mẫu cho database QLCHDB
-- Cấu trúc đúng theo bảng đã tạo

-- 1. XÓA DỮ LIỆU CŨ (nếu có)
DELETE FROM bill_items;
DELETE FROM stock_in;
DELETE FROM bills;
DELETE FROM products;
DELETE FROM customers;
DELETE FROM employees;
GO

-- 2. THÊM DỮ LIỆU BẢNG EMPLOYEES (Nhân viên)
SET IDENTITY_INSERT employees ON;
INSERT INTO employees (id, full_name, phone, email, role, salary, created_at)
VALUES
(1, N'Nguyen Van An', '0901234567', 'nguyenvanan@gmail.com', N'Quan ly', 15000000, '2020-01-15'),
(2, N'Tran Thi Binh', '0912345678', 'tranthib@gmail.com', N'Thu ngan', 8000000, '2021-03-20'),
(3, N'Le Van Cuong', '0923456789', 'levanc@gmail.com', N'Nhan vien ban hang', 7000000, '2021-06-10'),
(4, N'Pham Thi Dung', '0934567890', 'phamthid@gmail.com', N'Quan ly kho', 6500000, '2022-02-01'),
(5, N'Hoang Van Em', '0945678901', 'hoangvane@gmail.com', N'Nhan vien ban hang', 7000000, '2022-08-15');
SET IDENTITY_INSERT employees OFF;
GO

-- 3. THÊM DỮ LIỆU BẢNG CUSTOMERS (Khách hàng)
SET IDENTITY_INSERT customers ON;
INSERT INTO customers (id, full_name, phone, email, address, created_at)
VALUES
(1, N'Ngo Huu Thuan', '0987654321', 'ngohuuthuan@gmail.com', N'12 Ly Thai To, Q10, TP.HCM', '2023-01-10'),
(2, N'Tran Dieu Huyen', '0976543210', 'trandh@gmail.com', N'45 Le Van Sy, Q3, TP.HCM', '2023-02-15'),
(3, N'Tran Cong Danh', '0965432109', 'trancd@gmail.com', N'78 Nguyen Thi Minh Khai, Q1, TP.HCM', '2023-03-20'),
(4, N'Nguyen Duy Quy', '0954321098', 'nguyendq@gmail.com', N'90 Pasteur, Q1, TP.HCM', '2023-04-25'),
(5, N'Huynh Tan Dinh', '0943210987', 'huynhtd@gmail.com', N'34 Cach Mang Thang 8, Q10, TP.HCM', '2023-05-30'),
(6, N'Le Thi Hoa', '0932109876', 'lethihoa@gmail.com', N'56 Dien Bien Phu, Q3, TP.HCM', '2023-06-15'),
(7, N'Vo Van Khoa', '0921098765', 'vovank@gmail.com', N'67 Truong Chinh, Q12, TP.HCM', '2023-07-20'),
(8, N'Dang Thi Lan', '0910987654', 'dangthilan@gmail.com', N'89 Phan Dang Luu, Binh Thanh, TP.HCM', '2023-08-10');
SET IDENTITY_INSERT customers OFF;
GO

-- 4. THÊM DỮ LIỆU BẢNG PRODUCTS (Sản phẩm)
SET IDENTITY_INSERT products ON;
INSERT INTO products (id, name, price, quantity, unit, description, created_at)
VALUES
-- Đồ uống
(1, N'Coca Cola 330ml', 12000, 500, N'Lon', N'Nuoc ngot co gas', '2024-01-01'),
(2, N'Pepsi 330ml', 11000, 450, N'Lon', N'Nuoc ngot co gas', '2024-01-01'),
(3, N'Nuoc suoi Aquafina 500ml', 8000, 800, N'Chai', N'Nuoc khoang tinh khiet', '2024-01-01'),
(4, N'Tra xanh 0 do', 10000, 300, N'Chai', N'Tra xanh khong duong', '2024-01-01'),
(5, N'Sting dau', 12000, 400, N'Lon', N'Nuoc tang luc vi dau', '2024-01-01'),
(6, N'Redbull', 15000, 350, N'Lon', N'Nuoc tang luc', '2024-01-01'),
(7, N'Cafe Highlands', 35000, 200, N'Ly', N'Ca phe phin', '2024-01-01'),
(8, N'Sua tuoi Vinamilk', 9000, 500, N'Hop', N'Sua tuoi thanh trung', '2024-01-01'),

-- Snack & Bánh kẹo
(9, N'Snack khoai tay Lays', 15000, 400, N'Goi', N'Snack khoai tay chien', '2024-01-01'),
(10, N'Oishi', 10000, 450, N'Goi', N'Snack cac loai', '2024-01-01'),
(11, N'Poca', 8000, 500, N'Goi', N'Snack khoai tay', '2024-01-01'),
(12, N'Banh Oreo', 20000, 300, N'Goi', N'Banh quy kem', '2024-01-01'),
(13, N'Keo Mentos', 5000, 600, N'Vien', N'Keo nhai', '2024-01-01'),
(14, N'Socola KitKat', 12000, 350, N'Thanh', N'Socola sua', '2024-01-01'),

-- Mì ăn liền
(15, N'Mi Hao Hao tom chua cay', 5000, 1000, N'Goi', N'Mi an lien vi tom chua cay', '2024-01-01'),
(16, N'Mi 3 Mien', 6000, 800, N'Goi', N'Mi an lien cac vi', '2024-01-01'),
(17, N'Mi Omachi', 7000, 700, N'Goi', N'Mi cao cap', '2024-01-01'),
(18, N'Mi ly Hao Hao', 10000, 500, N'Ly', N'Mi ly tien loi', '2024-01-01'),

-- Gia vị & Đồ khô
(19, N'Dau an Simply 1L', 45000, 200, N'Chai', N'Dau an cao cap', '2024-01-01'),
(20, N'Nuoc mam Nam Ngu 500ml', 35000, 250, N'Chai', N'Nuoc mam truyen thong', '2024-01-01'),
(21, N'Gao ST25 5kg', 120000, 150, N'Tui', N'Gao ngon nhat the gioi', '2024-01-01'),
(22, N'Mi chinh Ajinomoto', 15000, 400, N'Goi', N'Bot ngot', '2024-01-01'),
(23, N'Duong trang', 20000, 300, N'Kg', N'Duong trang tinh luyen', '2024-01-01'),

-- Đồ dùng cá nhân
(24, N'Khan giay Kleenex', 25000, 400, N'Goi', N'Khan giay cao cap', '2024-01-01'),
(25, N'Ban chai danh rang P/S', 18000, 350, N'Cai', N'Ban chai danh rang', '2024-01-01'),
(26, N'Kem danh rang Colgate', 30000, 300, N'Tuyp', N'Kem danh rang', '2024-01-01'),
(27, N'Dau goi Clear 650ml', 85000, 200, N'Chai', N'Dau goi sach gau', '2024-01-01'),
(28, N'Sua tam Lifebuoy', 75000, 250, N'Chai', N'Sua tam diet khuan', '2024-01-01'),

-- Hàng tươi sống
(29, N'Trung ga (vi 10 qua)', 35000, 300, N'Vi', N'Trung ga tuoi', '2024-01-01'),
(30, N'Xuc xich', 50000, 200, N'Kg', N'Xuc xich dong lanh', '2024-01-01');
SET IDENTITY_INSERT products OFF;
GO

-- 5. THÊM DỮ LIỆU BẢNG STOCK_IN (Nhập kho)
SET IDENTITY_INSERT stock_in ON;
INSERT INTO stock_in (id, product_id, quantity, import_price, import_date)
VALUES
(1, 1, 500, 10000, '2024-01-15'),
(2, 2, 450, 9500, '2024-01-15'),
(3, 3, 800, 6000, '2024-01-16'),
(4, 15, 1000, 4000, '2024-01-16'),
(5, 9, 400, 12000, '2024-01-17'),
(6, 21, 150, 100000, '2024-01-18'),
(7, 19, 200, 40000, '2024-01-19'),
(8, 27, 200, 75000, '2024-01-20'),
(9, 29, 300, 30000, '2024-01-21'),
(10, 30, 200, 45000, '2024-01-21'),
(11, 4, 300, 8000, '2024-01-22'),
(12, 5, 400, 10000, '2024-01-22'),
(13, 10, 450, 8000, '2024-01-23'),
(14, 16, 800, 5000, '2024-01-23'),
(15, 24, 400, 20000, '2024-01-24');
SET IDENTITY_INSERT stock_in OFF;
GO

-- 6. THÊM DỮ LIỆU BẢNG BILLS (Hóa đơn)
SET IDENTITY_INSERT bills ON;
INSERT INTO bills (id, customer_id, employee_id, total_amount, bill_date)
VALUES
(1, 1, 2, 69000, '2024-02-01 09:30:00'),
(2, 2, 2, 79000, '2024-02-01 10:15:00'),
(3, 3, 3, 88000, '2024-02-01 11:20:00'),
(4, 4, 2, 99000, '2024-02-01 14:30:00'),
(5, 5, 3, 120000, '2024-02-01 16:45:00'),
(6, 6, 2, 150000, '2024-02-02 08:00:00'),
(7, 7, 3, 45000, '2024-02-02 09:30:00'),
(8, 8, 2, 185000, '2024-02-02 11:00:00'),
(9, 1, 2, 95000, '2024-02-02 14:20:00'),
(10, 2, 3, 67000, '2024-02-02 16:30:00'),
(11, 3, 2, 156000, '2024-02-03 10:00:00'),
(12, 5, 3, 89000, '2024-02-03 14:15:00'),
(13, 6, 2, 234000, '2024-02-04 11:30:00'),
(14, 7, 3, 78000, '2024-02-04 15:45:00'),
(15, 4, 2, 125000, '2024-02-05 09:20:00');
SET IDENTITY_INSERT bills OFF;
GO

-- 7. THÊM DỮ LIỆU BẢNG BILL_ITEMS (Chi tiết hóa đơn)
SET IDENTITY_INSERT bill_items ON;

-- HD 1: Tổng 69,000
INSERT INTO bill_items (id, bill_id, product_id, quantity, price)
VALUES
(1, 1, 1, 2, 12000),   -- 2 Coca = 24,000
(2, 1, 15, 3, 5000),   -- 3 Mi Hao Hao = 15,000
(3, 1, 9, 2, 15000);   -- 2 Lays = 30,000

-- HD 2: Tổng 79,000
INSERT INTO bill_items (id, bill_id, product_id, quantity, price)
VALUES
(4, 2, 2, 3, 11000),   -- 3 Pepsi = 33,000
(5, 2, 10, 2, 10000),  -- 2 Oishi = 20,000
(6, 2, 13, 2, 5000),   -- 2 Keo Mentos = 10,000
(7, 2, 18, 1, 10000),  -- 1 Mi ly = 10,000
(8, 2, 5, 1, 12000);   -- 1 Sting = 12,000

-- HD 3: Tổng 88,000
INSERT INTO bill_items (id, bill_id, product_id, quantity, price)
VALUES
(9, 3, 7, 2, 35000),   -- 2 Cafe = 70,000
(10, 3, 14, 1, 12000), -- 1 KitKat = 12,000
(11, 3, 4, 1, 10000);  -- 1 Tra 0 do = 10,000

-- HD 4: Tổng 99,000
INSERT INTO bill_items (id, bill_id, product_id, quantity, price)
VALUES
(12, 4, 26, 2, 30000), -- 2 Kem danh rang = 60,000
(13, 4, 25, 1, 18000), -- 1 Ban chai = 18,000
(14, 4, 24, 1, 25000); -- 1 Khan giay = 25,000

-- HD 5: Tổng 120,000
INSERT INTO bill_items (id, bill_id, product_id, quantity, price)
VALUES
(15, 5, 21, 1, 120000); -- 1 Gao ST25 = 120,000

-- HD 6: Tổng 150,000
INSERT INTO bill_items (id, bill_id, product_id, quantity, price)
VALUES
(16, 6, 19, 1, 45000), -- 1 Dau an = 45,000
(17, 6, 20, 1, 35000), -- 1 Nuoc mam = 35,000
(18, 6, 22, 2, 15000), -- 2 Mi chinh = 30,000
(19, 6, 23, 2, 20000); -- 2 Duong = 40,000

-- HD 7: Tổng 45,000
INSERT INTO bill_items (id, bill_id, product_id, quantity, price)
VALUES
(20, 7, 16, 5, 6000),  -- 5 Mi 3 Mien = 30,000
(21, 7, 3, 2, 8000);   -- 2 Nuoc suoi = 16,000

-- HD 8: Tổng 185,000
INSERT INTO bill_items (id, bill_id, product_id, quantity, price)
VALUES
(22, 8, 27, 1, 85000), -- 1 Dau goi = 85,000
(23, 8, 28, 1, 75000), -- 1 Sua tam = 75,000
(24, 8, 24, 1, 25000); -- 1 Khan giay = 25,000

-- HD 9: Tổng 95,000
INSERT INTO bill_items (id, bill_id, product_id, quantity, price)
VALUES
(25, 9, 29, 2, 35000), -- 2 Vi trung = 70,000
(26, 9, 8, 3, 9000);   -- 3 Sua tuoi = 27,000

-- HD 10: Tổng 67,000
INSERT INTO bill_items (id, bill_id, product_id, quantity, price)
VALUES
(27, 10, 11, 3, 8000), -- 3 Poca = 24,000
(28, 10, 12, 1, 20000),-- 1 Oreo = 20,000
(29, 10, 6, 1, 15000), -- 1 Redbull = 15,000
(30, 10, 3, 1, 8000);  -- 1 Nuoc suoi = 8,000

-- HD 11: Tổng 156,000
INSERT INTO bill_items (id, bill_id, product_id, quantity, price)
VALUES
(31, 11, 1, 5, 12000), -- 5 Coca = 60,000
(32, 11, 9, 4, 15000), -- 4 Lays = 60,000
(33, 11, 17, 3, 7000), -- 3 Mi Omachi = 21,000
(34, 11, 4, 2, 10000); -- 2 Tra 0 do = 20,000

-- HD 12: Tổng 89,000
INSERT INTO bill_items (id, bill_id, product_id, quantity, price)
VALUES
(35, 12, 2, 4, 11000), -- 4 Pepsi = 44,000
(36, 12, 10, 3, 10000),-- 3 Oishi = 30,000
(37, 12, 15, 3, 5000); -- 3 Mi Hao Hao = 15,000

-- HD 13: Tổng 234,000
INSERT INTO bill_items (id, bill_id, product_id, quantity, price)
VALUES
(38, 13, 21, 1, 120000), -- 1 Gao = 120,000
(39, 13, 30, 1, 50000),  -- 1 Xuc xich = 50,000
(40, 13, 19, 1, 45000),  -- 1 Dau an = 45,000
(41, 13, 20, 1, 35000);  -- 1 Nuoc mam = 35,000

-- HD 14: Tổng 78,000
INSERT INTO bill_items (id, bill_id, product_id, quantity, price)
VALUES
(42, 14, 5, 3, 12000),  -- 3 Sting = 36,000
(43, 14, 11, 3, 8000),  -- 3 Poca = 24,000
(44, 14, 18, 2, 10000); -- 2 Mi ly = 20,000

-- HD 15: Tổng 125,000
INSERT INTO bill_items (id, bill_id, product_id, quantity, price)
VALUES
(45, 15, 26, 3, 30000), -- 3 Kem danh rang = 90,000
(46, 15, 25, 2, 18000); -- 2 Ban chai = 36,000

SET IDENTITY_INSERT bill_items OFF;
GO

-- 8. KIỂM TRA DỮ LIỆU
PRINT '========================================';
PRINT 'THONG KE DU LIEU SAU KHI THEM';
PRINT '========================================';

SELECT 'employees' AS [Bang], COUNT(*) AS [So luong] FROM employees
UNION ALL
SELECT 'customers', COUNT(*) FROM customers
UNION ALL
SELECT 'products', COUNT(*) FROM products
UNION ALL
SELECT 'stock_in', COUNT(*) FROM stock_in
UNION ALL
SELECT 'bills', COUNT(*) FROM bills
UNION ALL
SELECT 'bill_items', COUNT(*) FROM bill_items;
GO

-- 9. HIỂN THỊ MẪU DỮ LIỆU
PRINT '';
PRINT '===== 5 HOA DON MOI NHAT =====';
SELECT TOP 5 
    b.id,
    c.full_name AS [Khach hang],
    e.full_name AS [Nhan vien],
    b.total_amount AS [Tong tien],
    b.bill_date AS [Ngay lap]
FROM bills b
LEFT JOIN customers c ON b.customer_id = c.id
LEFT JOIN employees e ON b.employee_id = e.id
ORDER BY b.bill_date DESC;
GO

PRINT '';
PRINT '===== 10 SAN PHAM BAN CHAY NHAT =====';
SELECT TOP 10
    p.name AS [San pham],
    SUM(bi.quantity) AS [So luong ban],
    SUM(bi.quantity * bi.price) AS [Doanh thu]
FROM bill_items bi
JOIN products p ON bi.product_id = p.id
GROUP BY p.name
ORDER BY SUM(bi.quantity) DESC;
GO

PRINT '';
PRINT '===== HOAN THANH THEM DU LIEU =====';