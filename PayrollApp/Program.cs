/****************/
// Mã sinh viên: 202418947
// Họ tên:     Đặng Ngọc Minh
// Bài thực hành: Lab 04 - Kế thừa: Hệ thống tính lương và thưởng nhân sự
/****************/

using System;
using System.Globalization;

namespace PayrollApp
{
    /// <summary>
    /// Chương trình kiểm thử theo mục C của đề bài: dựng dữ liệu mẫu, in bảng lương bằng
    /// lời gọi đa hình, kiểm tra các tình huống biên và các trường hợp dữ liệu sai.
    /// </summary>
    class Program
    {
        // ----- Bộ đếm kết quả kiểm thử -----
        private static int passCount;
        private static int failCount;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Cố định văn hoá Việt Nam để định dạng tiền tệ ổn định khi in ra (15.000.000 VND)
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("vi-VN");
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("vi-VN");

            PrintBanner();
            RunSectionC();
            PrintBonusHistory();
            RunBoundaryTests();
            ExplainMethodSelection();
            PrintSummary();
        }

        // ----- In phần mở đầu: thông tin sinh viên và chủ đề bài thực hành -----
        static void PrintBanner()
        {
            Console.WriteLine("==============================================================");
            Console.WriteLine(" LAB 04 - KẾ THỪA (INHERITANCE)");
            Console.WriteLine(" Mã sinh viên: 202418947 - Họ tên: Đặng Ngọc Minh");
            Console.WriteLine(" Chủ đề: Hệ thống tính lương và thưởng nhân sự");
            Console.WriteLine("==============================================================\n");
        }

        /// <summary>
        /// Dựng đúng bốn nhân sự của mục C, kiểm tra thu nhập từng người, tổng bảng lương,
        /// tổng theo phòng ban và người có thu nhập cao nhất.
        /// </summary>
        static void RunSectionC()
        {
            Console.WriteLine("[1] DỮ LIỆU KIỂM THỬ THEO MỤC C");

            // Bảng lương kỳ 2026-09 (dùng constructor đầy đủ của từng lớp dẫn xuất)
            var payroll = new Payroll("2026-09");

            var e001 = new SalariedEmployee("E001", "Nguyễn Minh An", "Đào tạo", 15_000_000, 2_000_000);
            e001.AddBonus(1_000_000);                                      // nạp chồng 1: thưởng cố định

            var e002 = new HourlyEmployee("E002", "Trần Thu Bình", "Hỗ trợ", 100_000, 150);
            e002.AddBonus(500_000);                                        // nạp chồng 1: thưởng cố định

            var e003 = new HourlyEmployee("E003", "Lê Hoàng Chi", "Hỗ trợ", 100_000, 170);
            // E003 không có thưởng

            var e004 = new SalesEmployee("E004", "Phạm Quốc Dũng", "Kinh doanh", 8_000_000, 200_000_000, 0.05);
            e004.AddBonus(0.02, 50_000_000, "Thưởng theo tỷ lệ doanh số");  // nạp chồng 3: thưởng theo tỷ lệ

            Console.WriteLine("  Thêm nhân sự vào bảng lương kỳ 2026-09:");
            CheckBool("C1. Thêm E001 (lương cố định) vào bảng lương", true, payroll.AddEmployee(e001));
            CheckBool("C2. Thêm E002 (theo giờ) vào bảng lương", true, payroll.AddEmployee(e002));
            CheckBool("C3. Thêm E003 (theo giờ, có giờ vượt ngưỡng) vào bảng lương", true, payroll.AddEmployee(e003));
            CheckBool("C4. Thêm E004 (kinh doanh) vào bảng lương", true, payroll.AddEmployee(e004));

            Console.WriteLine("\n  Thu nhập từng nhân sự (gọi CalculateGrossPay() qua kiểu Employee):");
            CheckMoney("C5. E001 = lương cố định 15.000.000 + phụ cấp 2.000.000 + thưởng 1.000.000",
                       18_000_000, e001.CalculateGrossPay());
            CheckMoney("C6. E002 = 150 giờ × 100.000 + thưởng 500.000",
                       15_500_000, e002.CalculateGrossPay());
            CheckMoney("C7. E003 = 160 giờ × 100.000 + 10 giờ × 100.000 × 1,5",
                       17_500_000, e003.CalculateGrossPay());
            CheckMoney("C8. E004 = 8.000.000 + 200.000.000 × 5% + 50.000.000 × 2%",
                       19_000_000, e004.CalculateGrossPay());

            Console.WriteLine("\n  Tổng hợp bảng lương:");
            CheckMoney("C9. Tổng bảng lương kỳ 2026-09", 70_000_000, payroll.CalculateTotalPayroll());
            CheckMoney("C10. Tổng của phòng \"Hỗ trợ\"", 33_000_000, payroll.CalculatePayrollByDepartment("Hỗ trợ"));
            CheckMoney("C11. Tổng của phòng không có nhân sự (\"Kế toán\")", 0, payroll.CalculatePayrollByDepartment("Kế toán"));

            Employee? highest = payroll.FindHighestPaidEmployee();
            Check("C12. Người có thu nhập cao nhất là E004 (19.000.000)",
                  highest != null && highest.EmployeeId == "E004" && highest.CalculateGrossPay() == 19_000_000,
                  "E004 - 19.000.000 VND",
                  highest == null ? "(không có)" : $"{highest.EmployeeId} - {highest.CalculateGrossPay():N0} VND");

            // Bất biến: không có hai nhân sự cùng mã trong một bảng lương
            CheckBool("C13. Thêm lại E001 (trùng mã) phải bị từ chối", false, payroll.AddEmployee(e001));

            // Đa hình khi in bảng lương: mỗi đối tượng tự in theo loại thực tế của nó
            Console.WriteLine("\n  Bảng lương in bằng lời gọi đa hình DisplayPayrollInfo():");
            payroll.DisplayPayroll();
        }
        /// <summary>In lịch sử thưởng của hai nhân sự có thưởng để giải thích cho tổng thưởng.</summary>
        static void PrintBonusHistory()
        {
            Console.WriteLine("\n[2] LỊCH SỬ THƯỞNG (quyết định thiết kế: lưu lịch sử thay vì chỉ lưu tổng)");

            var e001 = new SalariedEmployee("E001", "Nguyễn Minh An", "Đào tạo", 15_000_000, 2_000_000);
            e001.AddBonus(1_000_000);
            e001.DisplayBonusHistory();

            var e004 = new SalesEmployee("E004", "Phạm Quốc Dũng", "Kinh doanh", 8_000_000, 200_000_000, 0.05);
            e004.AddBonus(0.02, 50_000_000, "Thưởng theo tỷ lệ doanh số");
            e004.DisplayBonusHistory();

            CheckMoney("C14. Trước khi đặt lại thưởng, tổng thưởng của E004", 1_000_000, e004.MonthlyBonus);

            // Đặt lại thưởng khi sang kỳ lương mới
            e004.ResetBonus();
            CheckMoney("C15. Sau ResetBonus(), tổng thưởng của E004 về 0", 0, e004.MonthlyBonus);
            CheckMoney("C16. Thu nhập của E004 sau khi đặt lại thưởng chỉ còn lương và hoa hồng",
                       18_000_000, e004.CalculateGrossPay());
        }

        /// <summary>
        /// Kiểm thử biên và kiểm thử lỗi: số giờ làm (0..250, ngưỡng 160), tỷ lệ hoa hồng (0..0,3),
        /// ba phiên bản AddBonus(), bất biến của thông tin chung và bất biến của Payroll.
        /// </summary>
        static void RunBoundaryTests()
        {
            Console.WriteLine("\n[3] TÌNH HUỐNG BIÊN VÀ KIỂM THỬ LỖI");

            // --- Biên của số giờ làm việc (0..250 giờ, ngưỡng 160 giờ) ---
            var h160 = new HourlyEmployee("B160", "Biên 160 giờ", "Kiểm thử", 100_000, 160);
            CheckMoney("B1. Đúng ngưỡng 160 giờ nên không phát sinh giờ vượt ngưỡng",
                       16_000_000, h160.CalculateGrossPay());

            var h0 = new HourlyEmployee("B000", "Biên 0 giờ", "Kiểm thử", 100_000, 0);
            CheckMoney("B2. Không có giờ làm nào: thu nhập bằng 0", 0, h0.CalculateGrossPay());

            var h250 = new HourlyEmployee("B250", "Biên 250 giờ", "Kiểm thử", 100_000, 250);
            CheckMoney("B3. Biên trên 250 giờ: 160 giờ thường + 90 giờ vượt ngưỡng",
                       160 * 100_000 + 90 * 100_000 * 1.5, h250.CalculateGrossPay());

            CheckThrow("B4. 251 giờ: vượt biên trên", "Số giờ làm",
                       () => new HourlyEmployee("B251", "Sai số giờ", "Kiểm thử", 100_000, 251));
            CheckThrow("B5. -1 giờ: số giờ làm âm", "Số giờ làm",
                       () => new HourlyEmployee("B01", "Sai số giờ", "Kiểm thử", 100_000, -1));
            CheckThrow("B6. Đơn giá giờ âm", "Đơn giá giờ",
                       () => new HourlyEmployee("B06", "Sai đơn giá", "Kiểm thử", -1, 10));

            var hUpdate = new HourlyEmployee("B07", "Cập nhật số giờ", "Kiểm thử", 100_000, 100);
            CheckThrow("B7. Cập nhật số giờ 300 qua phương thức có kiểm soát", "Số giờ làm",
                       () => hUpdate.UpdateWorkedHours(300));
            hUpdate.UpdateWorkedHours(250);
            CheckNumber("B8. Cập nhật số giờ 250 (biên trên) được chấp nhận", 250, hUpdate.WorkedHours);

            // --- Biên của tỷ lệ hoa hồng (0..0,3) ---
            var s30 = new SalesEmployee("B30", "Biên hoa hồng 30%", "Kinh doanh", 5_000_000, 100_000_000, 0.3);
            CheckMoney("B9. Biên trên tỷ lệ hoa hồng 30%: 5.000.000 + 100.000.000 × 30%",
                       35_000_000, s30.CalculateGrossPay());
            CheckThrow("B10. Tỷ lệ hoa hồng 31%: vượt biên trên", "Tỷ lệ hoa hồng",
                       () => new SalesEmployee("B31", "Sai hoa hồng", "Kinh doanh", 5_000_000, 100_000_000, 0.31));
            CheckThrow("B11. Tỷ lệ hoa hồng âm", "Tỷ lệ hoa hồng",
                       () => new SalesEmployee("B11", "Sai hoa hồng", "Kinh doanh", 5_000_000, 100_000_000, -0.01));
            CheckThrow("B12. Lương cơ bản âm", "Lương cơ bản",
                       () => new SalesEmployee("B12", "Sai lương", "Kinh doanh", -1, 100_000_000, 0.05));
            CheckThrow("B13. Cập nhật doanh số âm", "Doanh số",
                       () => s30.UpdateSalesRevenue(-1));
            CheckThrow("B14. Lương tháng âm của nhân viên lương cố định", "Lương tháng",
                       () => new SalariedEmployee("B14", "Sai lương", "Đào tạo", -1, 0));
            CheckThrow("B15. Phụ cấp trách nhiệm âm", "Phụ cấp trách nhiệm",
                       () => new SalariedEmployee("B15", "Sai phụ cấp", "Đào tạo", 1_000_000, -1));
            // --- Biên của ba phiên bản AddBonus() ---
            var bonusTest = new SalariedEmployee("B16", "Kiểm tra thưởng", "Đào tạo", 10_000_000, 0);
            bonusTest.AddBonus(0.5, 10_000_000, "Biên trên tỷ lệ thưởng");   // nạp chồng 3
            CheckMoney("B16. Tỷ lệ thưởng đúng biên trên 0,5: 0,5 × 10.000.000",
                       5_000_000, bonusTest.MonthlyBonus);
            CheckThrow("B17. Tỷ lệ thưởng 0,51: vượt biên trên", "Tỷ lệ thưởng",
                       () => bonusTest.AddBonus(0.51, 10_000_000, "Vượt biên"));
            CheckThrow("B18. Tỷ lệ thưởng âm", "Tỷ lệ thưởng",
                       () => bonusTest.AddBonus(-0.1, 10_000_000, "Tỷ lệ âm"));
            CheckThrow("B19. Giá trị tham chiếu bằng 0", "Giá trị tham chiếu",
                       () => bonusTest.AddBonus(0.1, 0, "Tham chiếu bằng 0"));
            CheckThrow("B20. Số tiền thưởng bằng 0", "Số tiền thưởng",
                       () => bonusTest.AddBonus(0));
            CheckThrow("B21. Số tiền thưởng âm", "Số tiền thưởng",
                       () => bonusTest.AddBonus(-500_000));
            CheckThrow("B22. Lý do thưởng chỉ có khoảng trắng", "Lý do thưởng",
                       () => bonusTest.AddBonus(500_000, "   "));

            // Các lần gọi lỗi ở trên không được làm thay đổi tổng thưởng
            bonusTest.AddBonus(500_000, "Thưởng hợp lệ sau các trường hợp lỗi");
            CheckMoney("B23. Tổng thưởng chỉ tăng nhờ khoản thưởng hợp lệ",
                       5_500_000, bonusTest.MonthlyBonus);

            // --- Bất biến của thông tin chung (lớp Employee) ---
            CheckThrow("B24. Mã nhân sự rỗng", "Mã nhân sự",
                       () => new SalariedEmployee("   ", "Tên hợp lệ", 1_000_000));
            CheckThrow("B25. Họ tên rỗng", "Họ tên",
                       () => new SalariedEmployee("B25", "   ", 1_000_000));
            CheckThrow("B26. Phòng ban rỗng", "Phòng ban",
                       () => new SalariedEmployee("B26", "Tên hợp lệ", "   ", 1_000_000, 0));

            // --- Bất biến của Payroll ---
            CheckThrow("B27. Kỳ lương sai định dạng (2026-9)", "yyyy-MM", () => new Payroll("2026-9"));
            CheckThrow("B28. Kỳ lương có tháng 13", "yyyy-MM", () => new Payroll("2026-13"));
            CheckThrow("B29. Kỳ lương rỗng", "Kỳ lương", () => new Payroll("   "));

            var emptyPayroll = new Payroll("2026-10");
            CheckMoney("B30. Bảng lương rỗng: tổng bằng 0", 0, emptyPayroll.CalculateTotalPayroll());
            Check("B31. Bảng lương rỗng: không có ai là người thu nhập cao nhất",
                  emptyPayroll.FindHighestPaidEmployee() == null, "null",
                  emptyPayroll.FindHighestPaidEmployee() == null ? "null" : "(có đối tượng)");
            Check("B32. Tìm nhân sự không tồn tại trả về null",
                  emptyPayroll.FindEmployee("E999") == null, "null",
                  emptyPayroll.FindEmployee("E999") == null ? "null" : "(có đối tượng)");
            CheckThrow("B33. Thêm null vào bảng lương bị từ chối", "null",
                       () => emptyPayroll.AddEmployee(null!));

            var payroll2 = new Payroll("2026-11");
            payroll2.AddEmployee(new HourlyEmployee("B34", "Kiểm tra phòng ban", "Hỗ trợ", 100_000, 100));
            CheckMoney("B34. Tổng theo phòng ban không phân biệt chữ hoa/thường (\"hỗ TRỢ\")",
                       10_000_000, payroll2.CalculatePayrollByDepartment("hỗ TRỢ"));
            CheckThrow("B35. Tên phòng ban cần tính tổng bị rỗng", "Phòng ban",
                       () => payroll2.CalculatePayrollByDepartment("  "));

            // --- Constructor rút gọn dùng giá trị mặc định ---
            var shortSalaried = new SalariedEmployee("B36", "Dùng constructor rút gọn", 12_000_000);
            Check("B36. SalariedEmployee rút gọn: phòng mặc định Unassigned, phụ cấp 0, thưởng 0",
                  shortSalaried.Department == "Unassigned"
                  && shortSalaried.ResponsibilityAllowance == 0
                  && shortSalaried.MonthlyBonus == 0,
                  "Unassigned / 0 / 0",
                  $"{shortSalaried.Department} / {shortSalaried.ResponsibilityAllowance} / {shortSalaried.MonthlyBonus}");

            var shortHourly = new HourlyEmployee("B37", "Rút gọn theo giờ", 100_000);
            CheckNumber("B37. HourlyEmployee rút gọn: số giờ làm mặc định bằng 0", 0, shortHourly.WorkedHours);

            var shortSales = new SalesEmployee("B38", "Rút gọn kinh doanh", 7_000_000);
            CheckMoney("B38. SalesEmployee rút gọn: chưa có doanh số nên thu nhập bằng lương cơ bản",
                       7_000_000, shortSales.CalculateGrossPay());
        }

        /// <summary>
        /// In phần giải thích sự khác nhau giữa nạp chồng và ghi đè: phiên bản AddBonus() được chọn
        /// ở thời điểm biên dịch, còn phiên bản CalculateGrossPay() được chọn ở thời điểm chạy.
        /// </summary>
        static void ExplainMethodSelection()
        {
            Console.WriteLine("\n[4] GIẢI THÍCH LỰA CHỌN PHƯƠNG THỨC");
            Console.WriteLine("  a) AddBonus(1.000.000)                 → nạp chồng 1: AddBonus(double amount)");
            Console.WriteLine("  b) AddBonus(500.000, \"...\")           → nạp chồng 2: AddBonus(double, string)");
            Console.WriteLine("  c) AddBonus(0,02, 50.000.000, \"...\")  → nạp chồng 3: AddBonus(double, double, string)");
            Console.WriteLine("  Việc chọn phiên bản AddBonus() diễn ra ở THỜI ĐIỂM BIÊN DỊCH: trình biên dịch đối");
            Console.WriteLine("  chiếu số lượng và kiểu của đối số với danh sách tham số của từng phiên bản.");
            Console.WriteLine("  Việc chọn CalculateGrossPay() diễn ra ở THỜI ĐIỂM CHẠY (đa hình động): biến tham");
            Console.WriteLine("  chiếu khai báo kiểu Employee nhưng phương thức được gọi là phiên bản ghi đè của");
            Console.WriteLine("  lớp thực tế (SalariedEmployee / HourlyEmployee / SalesEmployee).");
            Console.WriteLine("  Nhờ vậy Payroll không cần chuỗi if/else theo loại nhân sự khi tính lương.");
            Console.WriteLine("  Employee là lớp trừu tượng nên không thể viết new Employee(...) trực tiếp.");
        }

        /// <summary>In tổng kết số tình huống kiểm thử đúng/sai.</summary>
        static void PrintSummary()
        {
            Console.WriteLine("\n[5] TỔNG KẾT KIỂM THỬ");
            Console.WriteLine($"  Số tình huống ĐÚNG: {passCount}");
            Console.WriteLine($"  Số tình huống SAI : {failCount}");
            Console.WriteLine($"  Tổng số tình huống: {passCount + failCount}");
            Console.WriteLine(failCount == 0
                ? "\n  => Tất cả tình huống kiểm thử đều cho kết quả đúng như mong đợi."
                : "\n  => Còn tình huống cho kết quả khác mong đợi, cần kiểm tra lại.");
            Console.WriteLine("\nKẾT THÚC CHƯƠNG TRÌNH.");
        }

        // ================= CÁC PHƯƠNG THỨC HỖ TRỢ KIỂM THỬ =================

        /// <summary>Ghi nhận và in kết quả của một tình huống kiểm thử.</summary>
        static void Check(string label, bool ok, string expected, string observed)
        {
            if (ok)
                passCount++;
            else
                failCount++;

            Console.WriteLine($"  [{(ok ? "ĐÚNG" : "SAI ")}] {label}");
            Console.WriteLine($"        Mong đợi: {expected}");
            Console.WriteLine($"        Quan sát: {observed}");
        }

        /// <summary>So sánh hai giá trị tiền tệ.</summary>
        static void CheckMoney(string label, double expected, double actual)
        {
            Check(label, Math.Abs(expected - actual) < 0.5, $"{expected:N0} VND", $"{actual:N0} VND");
        }

        /// <summary>So sánh hai giá trị số.</summary>
        static void CheckNumber(string label, double expected, double actual)
        {
            Check(label, Math.Abs(expected - actual) < 0.0001, $"{expected:N0}", $"{actual:N0}");
        }

        /// <summary>So sánh hai giá trị logic.</summary>
        static void CheckBool(string label, bool expected, bool actual)
        {
            Check(label, expected == actual, expected.ToString(), actual.ToString());
        }

        /// <summary>Kiểm tra một hành động có ném ngoại lệ với thông báo mong đợi hay không.</summary>
        static void CheckThrow(string label, string expectedKeyword, Action action)
        {
            try
            {
                action();
                Check(label, false, $"Ném ngoại lệ chứa \"{expectedKeyword}\"", "Không có ngoại lệ");
            }
            catch (Exception ex)
            {
                Check(label, ex.Message.Contains(expectedKeyword, StringComparison.OrdinalIgnoreCase),
                      $"Ném ngoại lệ chứa \"{expectedKeyword}\"", $"Ngoại lệ: {ex.Message}");
            }
        }
    }
}


