-- SCRIPT THÊM DỮ LIỆU AN TOÀN - CÓ XỬ LÝ LỖI

-- BẬT XỬ LÝ LỖI
SET XACT_ABORT ON;
BEGIN TRANSACTION;

BEGIN TRY
    -- 1. XÓA DỮ LIỆU CŨ (theo thứ tự khóa ngoại)
    PRINT 'Buoc 1: Xoa du lieu cu...';
    DELETE FROM bill_items;
    DELETE FROM stock_in;
    DELETE FROM bills;
    DELETE FROM products;
    DELETE FROM customers;
    DELETE FROM employees;
    PRINT 'Da xoa du lieu cu thanh cong!';
    PRINT '';

    -- 2. THÊM EMPLOYEES
    PRINT 'Buoc 2: Them nhan vien...';
    SET IDENTITY_INSERT employees ON;
    INSERT INTO employees (id, full_name, phone, email, role, salary, created_at)
    VALUES
    (1, N'Nguyen Van An', '0901234567', 'nguyenvanan@gmail.com', N'Quan ly', 15000000, '2020-01-15'),
    (2, N'Tran Thi Binh', '0912345678', 'tranthib@gmail.com', N'Thu ngan', 8000000, '2021-03-20'),
    (3, N'Le Van Cuong', '0923456789', 'levanc@gmail.com', N'Nhan vien ban hang', 7000000, '2021-06-10'),
    (4, N'Pham Thi Dung', '0934567890', 'phamthid@gmail.com', N'Quan ly kho', 6500000, '2022-02-01'),
    (5, N'Hoang Van Em', '0945678901', 'hoangvane@gmail.com', N'Nhan vien ban hang', 7000000, '2022-08-15');
    SET IDENTITY_INSERT employees OFF;
    PRINT 'Da them ' + CAST(@@ROWCOUNT AS VARCHAR) + ' nhan vien!';
    PRINT '';

    -- 3. THÊM CUSTOMERS
    PRINT 'Buoc 3: Them khach hang...';
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
    PRINT 'Da them ' + CAST(@@ROWCOUNT AS VARCHAR) + ' khach hang!';
    PRINT '';

    -- 4. THÊM PRODUCTS
    PRINT 'Buoc 4: Them san pham...';
    SET IDENTITY_INSERT products ON;
    INSERT INTO products (id, name, price, quantity, unit, description, created_at)
    VALUES
    (1, N'Coca Cola 330ml', 12000, 500, N'Lon', N'Nuoc ngot co gas', '2024-01-01'),
    (2, N'Pepsi 330ml', 11000, 450, N'Lon', N'Nuoc ngot co gas', '2024-01-01'),
    (3, N'Nuoc suoi Aquafina 500ml', 8000, 800, N'Chai', N'Nuoc khoang tinh khiet', '2024-01-01'),
    (4, N'Tra xanh 0 do', 10000, 300, N'Chai', N'Tra xanh khong duong', '2024-01-01'),
    (5, N'Sting dau', 12000, 400, N'Lon', N'Nuoc tang luc vi dau', '2024-01-01'),
    (6, N'Redbull', 15000, 350, N'Lon', N'Nuoc tang luc', '2024-01-01'),
    (7, N'Cafe Highlands', 35000, 200, N'Ly', N'Ca phe phin', '2024-01-01'),
    (8, N'Sua tuoi Vinamilk', 9000, 500, N'Hop', N'Sua tuoi thanh trung', '2024-01-01'),
    (9, N'Snack khoai tay Lays', 15000, 400, N'Goi', N'Snack khoai tay chien', '2024-01-01'),
    (10, N'Oishi', 10000, 450, N'Goi', N'Snack cac loai', '2024-01-01'),
    (11, N'Poca', 8000, 500, N'Goi', N'Snack khoai tay', '2024-01-01'),
    (12, N'Banh Oreo', 20000, 300, N'Goi', N'Banh quy kem', '2024-01-01'),
    (13, N'Keo Mentos', 5000, 600, N'Vien', N'Keo nhai', '2024-01-01'),
    (14, N'Socola KitKat', 12000, 350, N'Thanh', N'Socola sua', '2024-01-01'),
    (15, N'Mi Hao Hao tom chua cay', 5000, 1000, N'Goi', N'Mi an lien vi tom chua cay', '2024-01-01'),
    (16, N'Mi 3 Mien', 6000, 800, N'Goi', N'Mi an lien cac vi', '2024-01-01'),
    (17, N'Mi Omachi', 7000, 700, N'Goi', N'Mi cao cap', '2024-01-01'),
    (18, N'Mi ly Hao Hao', 10000, 500, N'Ly', N'Mi ly tien loi', '2024-01-01'),
    (19, N'Dau an Simply 1L', 45000, 200, N'Chai', N'Dau an cao cap', '2024-01-01'),
    (20, N'Nuoc mam Nam Ngu 500ml', 35000, 250, N'Chai', N'Nuoc mam truyen thong', '2024-01-01'),
    (21, N'Gao ST25 5kg', 120000, 150, N'Tui', N'Gao ngon nhat the gioi', '2024-01-01'),
    (22, N'Mi chinh Ajinomoto', 15000, 400, N'Goi', N'Bot ngot', '2024-01-01'),
    (23, N'Duong trang', 20000, 300, N'Kg', N'Duong trang tinh luyen', '2024-01-01'),
    (24, N'Khan giay Kleenex', 25000, 400, N'Goi', N'Khan giay cao cap', '2024-01-01'),
    (25, N'Ban chai danh rang P/S', 18000, 350, N'Cai', N'Ban chai danh rang', '2024-01-01'),
    (26, N'Kem danh rang Colgate', 30000, 300, N'Tuyp', N'Kem danh rang', '2024-01-01'),
    (27, N'Dau goi Clear 650ml', 85000, 200, N'Chai', N'Dau goi sach gau', '2024-01-01'),
    (28, N'Sua tam Lifebuoy', 75000, 250, N'Chai', N'Sua tam diet khuan', '2024-01-01'),
    (29, N'Trung ga (vi 10 qua)', 35000, 300, N'Vi', N'Trung ga tuoi', '2024-01-01'),
    (30, N'Xuc xich', 50000, 200, N'Kg', N'Xuc xich dong lanh', '2024-01-01');
    SET IDENTITY_INSERT products OFF;
    PRINT 'Da them ' + CAST(@@ROWCOUNT AS VARCHAR) + ' san pham!';
    PRINT '';

    -- 5. THÊM STOCK_IN
    PRINT 'Buoc 5: Them phieu nhap kho...';
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
    (10, 30, 200, 45000, '2024-01-21');
    SET IDENTITY_INSERT stock_in OFF;
    PRINT 'Da them ' + CAST(@@ROWCOUNT AS VARCHAR) + ' phieu nhap!';
    PRINT '';

    -- 6. THÊM BILLS
    PRINT 'Buoc 6: Them hoa don...';
    SET IDENTITY_INSERT bills ON;
    INSERT INTO bills (id, customer_id, employee_id, total_amount, bill_date)
    VALUES
    (1, 1, 2, 69000, '2024-02-01 09:30:00'),
    (2, 2, 2, 79000, '2024-02-01 10:15:00'),
    (3, 3, 3, 88000, '2024-02-01 11:20:00'),
    (4, 4, 2, 99000, '2024-02-01 14:30:00'),
    (5, 5, 3, 120000, '2024-02-01 16:45:00');
    SET IDENTITY_INSERT bills OFF;
    PRINT 'Da them ' + CAST(@@ROWCOUNT AS VARCHAR) + ' hoa don!';
    PRINT '';

    -- 7. THÊM BILL_ITEMS
    PRINT 'Buoc 7: Them chi tiet hoa don...';
    SET IDENTITY_INSERT bill_items ON;
    INSERT INTO bill_items (id, bill_id, product_id, quantity, price)
    VALUES
    (1, 1, 1, 2, 12000),
    (2, 1, 15, 3, 5000),
    (3, 1, 9, 2, 15000),
    (4, 2, 2, 3, 11000),
    (5, 2, 10, 2, 10000),
    (6, 3, 7, 2, 35000),
    (7, 3, 14, 1, 12000),
    (8, 4, 26, 2, 30000),
    (9, 4, 25, 1, 18000),
    (10, 5, 21, 1, 120000);
    SET IDENTITY_INSERT bill_items OFF;
    PRINT 'Da them ' + CAST(@@ROWCOUNT AS VARCHAR) + ' chi tiet hoa don!';
    PRINT '';

    -- COMMIT TRANSACTION
    COMMIT TRANSACTION;
    PRINT '========================================';
    PRINT 'THANH CONG! DA THEM TAT CA DU LIEU!';
    PRINT '========================================';

END TRY
BEGIN CATCH
    -- ROLLBACK nếu có lỗi
    ROLLBACK TRANSACTION;
    PRINT '========================================';
    PRINT 'LOI XAY RA:';
    PRINT ERROR_MESSAGE();
    PRINT '========================================';
END CATCH;
GO

-- KIỂM TRA KẾT QUẢ
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