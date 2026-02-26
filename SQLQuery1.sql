

-- 1. Bảng Khách hàng
CREATE TABLE customers (
    id INT IDENTITY(1,1) PRIMARY KEY, -- Thay AUTO_INCREMENT bằng IDENTITY(1,1)
    full_name NVARCHAR(100) NOT NULL, -- Dùng NVARCHAR để hỗ trợ tiếng Việt có dấu
    phone VARCHAR(20),
    email VARCHAR(100),
    address NVARCHAR(255),
    created_at DATETIME DEFAULT GETDATE() -- Thay CURRENT_TIMESTAMP bằng GETDATE()
);

-- 2. Bảng Nhân viên
CREATE TABLE employees (
    id INT IDENTITY(1,1) PRIMARY KEY,
    full_name NVARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    email VARCHAR(100),
    role NVARCHAR(50),
    salary DECIMAL(12,2),
    created_at DATETIME DEFAULT GETDATE()
);

-- 3. Bảng Sản phẩm
CREATE TABLE products (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(150) NOT NULL,
    price DECIMAL(12,2) NOT NULL,
    quantity INT NOT NULL DEFAULT 0,
    unit NVARCHAR(50),
    description NVARCHAR(MAX), -- Thay TEXT bằng NVARCHAR(MAX) trong SQL Server
    created_at DATETIME DEFAULT GETDATE()
);

-- 4. Bảng Hóa đơn
CREATE TABLE bills (
    id INT IDENTITY(1,1) PRIMARY KEY,
    customer_id INT,
    employee_id INT,
    total_amount DECIMAL(12,2),
    bill_date DATETIME DEFAULT GETDATE(),
    CONSTRAINT fk_bill_customer FOREIGN KEY (customer_id) REFERENCES customers(id),
    CONSTRAINT fk_bill_employee FOREIGN KEY (employee_id) REFERENCES employees(id)
);

-- 5. Chi tiết hóa đơn
CREATE TABLE bill_items (
    id INT IDENTITY(1,1) PRIMARY KEY,
    bill_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    price DECIMAL(12,2) NOT NULL,
    CONSTRAINT fk_item_bill FOREIGN KEY (bill_id) REFERENCES bills(id),
    CONSTRAINT fk_item_product FOREIGN KEY (product_id) REFERENCES products(id)
);

-- 6. Nhập kho
CREATE TABLE stock_in (
    id INT IDENTITY(1,1) PRIMARY KEY,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    import_price DECIMAL(12,2),
    import_date DATETIME DEFAULT GETDATE(),
    CONSTRAINT fk_stock_product FOREIGN KEY (product_id) REFERENCES products(id)
);