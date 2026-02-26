using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyCuaHang
{
    /// <summary>
    /// Interaction logic for QuanLyHoaDon.xaml
    /// </summary>
    public partial class QuanLyHoaDon : Window
    {
        private const string LocalDbServer = @"(LocalDB)\MSSQLLocalDB";
        private const string DatabaseName = "QLCHDB";
        private string activeDatabaseName = DatabaseName;

        public QuanLyHoaDon()
        {
            InitializeComponent();
            loadData();
        }

        public void loadData(DateTime? fromDate = null, DateTime? toDate = null, string invoiceId = null)
        {
            var dsHoaDon = new List<HoaDon>();

            var normalizedInvoiceId = NormalizeInvoiceId(invoiceId);

            var sql = @"SELECT
                            b.id,
                            ROW_NUMBER() OVER (ORDER BY b.bill_date DESC, b.id DESC) AS stt,
                            CAST(b.id AS NVARCHAR(20)) AS ma_hoa_don,
                            b.bill_date,
                            ISNULL(c.full_name, N'') AS khach_hang,
                            ISNULL(b.total_amount, 0) AS tong_tien
                        FROM bills b
                        LEFT JOIN customers c ON c.id = b.customer_id
                        WHERE (@fromDate IS NULL OR b.bill_date >= @fromDate)
                          AND (@toDate IS NULL OR b.bill_date < DATEADD(DAY, 1, @toDate))
                          AND (@invoiceId IS NULL OR CAST(b.id AS NVARCHAR(20)) LIKE '%' + @invoiceId + '%')
                        ORDER BY b.bill_date DESC, b.id DESC";

            try
            {
                EnsureDatabaseReady();

                using (var connection = new SqlConnection(BuildDatabaseConnectionString(activeDatabaseName)))
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@fromDate", SqlDbType.DateTime).Value = (object)fromDate ?? DBNull.Value;
                    command.Parameters.Add("@toDate", SqlDbType.DateTime).Value = (object)toDate ?? DBNull.Value;
                    command.Parameters.Add("@invoiceId", SqlDbType.NVarChar, 20).Value = string.IsNullOrWhiteSpace(normalizedInvoiceId)
                        ? (object)DBNull.Value
                        : normalizedInvoiceId;

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dsHoaDon.Add(new HoaDon
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Stt = Convert.ToInt32(reader["stt"]),
                                MaHoaDon = Convert.ToString(reader["ma_hoa_don"]),
                                NgayLap = reader["bill_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["bill_date"]),
                                KhachHang = Convert.ToString(reader["khach_hang"]),
                                TongTien = Convert.ToDecimal(reader["tong_tien"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được dữ liệu hóa đơn từ QLCHDB.\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            dt_hoadon.ItemsSource = dsHoaDon;
            lblTotalRevenue.Text = $"{dsHoaDon.Sum(x => x.TongTien):N0} VNĐ";
        }

        private static string BuildDatabaseConnectionString(string databaseName)
        {
            return $"Data Source={LocalDbServer};Initial Catalog={databaseName};Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True;";
        }

        private static string BuildMasterConnectionString()
        {
            return $"Data Source={LocalDbServer};Initial Catalog=master;Integrated Security=True;Connect Timeout=30;";
        }

        private void EnsureDatabaseReady()
        {
            var mdfCandidates = new[]
            {
                Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\QLCHDB.mdf")),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "QLCHDB.mdf"),
                Path.Combine(Environment.CurrentDirectory, "QLCHDB.mdf")
            };
            var mdfPath = mdfCandidates.FirstOrDefault(path => File.Exists(path));

            using (var connection = new SqlConnection(BuildMasterConnectionString()))
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText = "SELECT DB_ID(@dbName)";
                command.Parameters.Add("@dbName", SqlDbType.NVarChar, 128).Value = DatabaseName;
                var dbId = command.ExecuteScalar();
                command.Parameters.Clear();

                if (dbId != DBNull.Value && dbId != null)
                {
                    activeDatabaseName = DatabaseName;
                    return;
                }

                if (string.IsNullOrWhiteSpace(mdfPath) || !File.Exists(mdfPath))
                {
                    throw new FileNotFoundException("Không tìm thấy file QLCHDB.mdf để attach.", string.Join(" | ", mdfCandidates));
                }

                var escapedPath = mdfPath.Replace("'", "''");
                command.CommandText = $"CREATE DATABASE [{DatabaseName}] ON (FILENAME=N'{escapedPath}') FOR ATTACH;";
                command.ExecuteNonQuery();
                activeDatabaseName = DatabaseName;
            }
        }

        private static string NormalizeInvoiceId(string invoiceId)
        {
            if (string.IsNullOrWhiteSpace(invoiceId))
            {
                return null;
            }

            var value = invoiceId.Trim();
            if (value.StartsWith("HD", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
            }

            return value;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            loadData(dpFromDate.SelectedDate, dpToDate.SelectedDate, txtInvoiceId.Text);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ShowBillEditor("Thêm hóa đơn", string.Empty, DateTime.Now, 0m, out var customerName, out var billDate, out var totalAmount))
            {
                return;
            }

            try
            {
                EnsureDatabaseReady();
                using (var connection = new SqlConnection(BuildDatabaseConnectionString(activeDatabaseName)))
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    var customerId = GetOrCreateCustomerId(connection, customerName);

                    command.CommandText = @"INSERT INTO bills(customer_id, employee_id, total_amount, bill_date)
                                            VALUES (@customerId, NULL, @totalAmount, @billDate);";
                    command.Parameters.Add("@customerId", SqlDbType.Int).Value = customerId.HasValue
                        ? (object)customerId.Value
                        : DBNull.Value;
                    command.Parameters.Add("@totalAmount", SqlDbType.Decimal).Value = totalAmount;
                    command.Parameters["@totalAmount"].Precision = 12;
                    command.Parameters["@totalAmount"].Scale = 2;
                    command.Parameters.Add("@billDate", SqlDbType.DateTime).Value = billDate;
                    command.ExecuteNonQuery();
                }

                RefreshWithCurrentFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thêm hóa đơn thất bại.\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button) || !(button.CommandParameter is int id))
            {
                return;
            }

            var selected = dt_hoadon.Items.Cast<HoaDon>().FirstOrDefault(x => x.Id == id);
            if (selected == null)
            {
                return;
            }

            if (!ShowBillEditor("Cập nhật hóa đơn", selected.KhachHang, selected.NgayLap ?? DateTime.Now, selected.TongTien, out var customerName, out var billDate, out var totalAmount))
            {
                return;
            }

            try
            {
                EnsureDatabaseReady();
                using (var connection = new SqlConnection(BuildDatabaseConnectionString(activeDatabaseName)))
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    var customerId = GetOrCreateCustomerId(connection, customerName);

                    command.CommandText = @"UPDATE bills
                                            SET customer_id = @customerId,
                                                total_amount = @totalAmount,
                                                bill_date = @billDate
                                            WHERE id = @id;";
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.Parameters.Add("@customerId", SqlDbType.Int).Value = customerId.HasValue
                        ? (object)customerId.Value
                        : DBNull.Value;
                    command.Parameters.Add("@totalAmount", SqlDbType.Decimal).Value = totalAmount;
                    command.Parameters["@totalAmount"].Precision = 12;
                    command.Parameters["@totalAmount"].Scale = 2;
                    command.Parameters.Add("@billDate", SqlDbType.DateTime).Value = billDate;
                    command.ExecuteNonQuery();
                }

                RefreshWithCurrentFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cập nhật hóa đơn thất bại.\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button) || !(button.CommandParameter is int id))
            {
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc muốn xóa hóa đơn này không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                EnsureDatabaseReady();
                using (var connection = new SqlConnection(BuildDatabaseConnectionString(activeDatabaseName)))
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"DELETE FROM bill_items WHERE bill_id = @id;
                                            DELETE FROM bills WHERE id = @id;";
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                RefreshWithCurrentFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xóa hóa đơn thất bại.\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshWithCurrentFilters()
        {
            loadData(dpFromDate.SelectedDate, dpToDate.SelectedDate, txtInvoiceId.Text);
        }

        private int? GetOrCreateCustomerId(SqlConnection connection, string customerName)
        {
            var normalizedName = (customerName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return null;
            }

            using (var findCommand = connection.CreateCommand())
            {
                findCommand.CommandText = "SELECT TOP 1 id FROM customers WHERE full_name = @fullName ORDER BY id";
                findCommand.Parameters.Add("@fullName", SqlDbType.NVarChar, 100).Value = normalizedName;
                var existing = findCommand.ExecuteScalar();
                if (existing != null && existing != DBNull.Value)
                {
                    return Convert.ToInt32(existing);
                }
            }

            using (var insertCommand = connection.CreateCommand())
            {
                insertCommand.CommandText = @"INSERT INTO customers(full_name, created_at)
                                            VALUES (@fullName, GETDATE());
                                            SELECT CAST(SCOPE_IDENTITY() AS INT);";
                insertCommand.Parameters.Add("@fullName", SqlDbType.NVarChar, 100).Value = normalizedName;
                return Convert.ToInt32(insertCommand.ExecuteScalar());
            }
        }

        private bool ShowBillEditor(string title, string initialCustomerName, DateTime initialDate, decimal initialAmount, out string customerName, out DateTime billDate, out decimal totalAmount)
        {
            var selectedCustomerName = initialCustomerName ?? string.Empty;
            var selectedBillDate = initialDate;
            var selectedTotalAmount = initialAmount;

            var dialog = new Window
            {
                Title = title,
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Width = 360,
                Height = 300
            };

            var root = new Grid { Margin = new Thickness(14) };
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var customerPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(0, 0, 0, 10) };
            customerPanel.Children.Add(new TextBlock { Text = "Tên khách hàng", Margin = new Thickness(0, 0, 0, 6) });
            var customerTextBox = new TextBox
            {
                Height = 30,
                Text = initialCustomerName ?? string.Empty
            };
            customerPanel.Children.Add(customerTextBox);
            Grid.SetRow(customerPanel, 0);

            var dateLabel = new TextBlock { Text = "Ngày lập", Margin = new Thickness(0, 0, 0, 6) };
            Grid.SetRow(dateLabel, 1);

            var datePicker = new DatePicker
            {
                SelectedDate = initialDate,
                Height = 30,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(datePicker, 2);

            var amountPanel = new StackPanel { Orientation = Orientation.Vertical };
            amountPanel.Children.Add(new TextBlock { Text = "Tổng tiền", Margin = new Thickness(0, 0, 0, 6) });
            var amountTextBox = new TextBox
            {
                Height = 30,
                Text = initialAmount.ToString("0.##", CultureInfo.InvariantCulture)
            };
            amountPanel.Children.Add(amountTextBox);
            Grid.SetRow(amountPanel, 3);

            var actions = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 14, 0, 0)
            };
            var cancelButton = new Button { Content = "Hủy", Width = 80, Height = 30, Margin = new Thickness(0, 0, 8, 0) };
            var okButton = new Button { Content = "Lưu", Width = 80, Height = 30 };
            actions.Children.Add(cancelButton);
            actions.Children.Add(okButton);
            Grid.SetRow(actions, 5);

            cancelButton.Click += (_, __) =>
            {
                dialog.DialogResult = false;
                dialog.Close();
            };

            okButton.Click += (_, __) =>
            {
                if (!datePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vui lòng chọn ngày lập.", "Thiếu dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var normalizedCustomerName = (customerTextBox.Text ?? string.Empty).Trim();

                var input = amountTextBox.Text?.Trim();
                if (!decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsedAmount) &&
                    !decimal.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out parsedAmount))
                {
                    MessageBox.Show("Tổng tiền không hợp lệ.", "Sai định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (parsedAmount < 0)
                {
                    MessageBox.Show("Tổng tiền phải >= 0.", "Sai dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                selectedCustomerName = normalizedCustomerName;
                selectedBillDate = datePicker.SelectedDate.Value;
                selectedTotalAmount = parsedAmount;
                dialog.DialogResult = true;
                dialog.Close();
            };

            root.Children.Add(customerPanel);
            root.Children.Add(dateLabel);
            root.Children.Add(datePicker);
            root.Children.Add(amountPanel);
            root.Children.Add(actions);

            dialog.Content = root;
            var isSaved = dialog.ShowDialog() == true;
            customerName = selectedCustomerName;
            billDate = selectedBillDate;
            totalAmount = selectedTotalAmount;
            return isSaved;
        }
    }
}
