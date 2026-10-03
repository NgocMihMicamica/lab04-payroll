/****************/
// Mã sinh viên: 202418947
// Họ tên:     Đặng Ngọc Minh
// Bài thực hành: Lab 04 - Kế thừa: Hệ thống tính lương và thưởng nhân sự
/****************/

using System;
using System.Collections.Generic;

namespace PayrollApp
{
    /// <summary>
    /// Lớp Payroll - bảng lương của một kỳ lương.
    /// Trách nhiệm: quản lý danh sách nhân sự của kỳ lương (thêm, tìm theo mã),
    /// tính tổng bảng lương, tính tổng theo phòng ban, tìm người có thu nhập cao nhất
    /// và in bảng lương.
    /// Quan hệ: KẾT TẬP (aggregation) với Employee - Payroll giữ danh sách THAM CHIẾU
    /// tới các nhân sự; Payroll KHÔNG phải là một loại nhân sự nên KHÔNG kế thừa Employee.
    /// Bất biến: kỳ lương không rỗng và theo dạng yyyy-MM; không có hai nhân sự cùng mã
    /// trong một bảng lương.
    /// </summary>
    public class Payroll
    {
        // ----- Thuộc tính -----
        private readonly string period;                  // Kỳ lương, ví dụ "2026-09"
        private readonly List<Employee> employees;       // Danh sách nhân sự của kỳ lương

        /// <summary>
        /// Constructor: nhận kỳ lương. Bất biến của kỳ lương được kiểm tra ngay khi tạo bảng lương.
        /// </summary>
        public Payroll(string period)
        {
            if (string.IsNullOrWhiteSpace(period))
                throw new ArgumentException("Kỳ lương không được rỗng.");
            if (!IsValidPeriod(period.Trim()))
                throw new ArgumentException("Kỳ lương phải có dạng yyyy-MM, ví dụ 2026-09.");

            this.period = period.Trim();
            this.employees = new List<Employee>();
        }

        // ----- Kiểm tra định dạng kỳ lương -----
        private static bool IsValidPeriod(string period)
        {
            if (period.Length != 7 || period[4] != '-')
                return false;

            for (int i = 0; i < period.Length; i++)
            {
                if (i == 4)
                    continue;
                if (!char.IsDigit(period[i]))
                    return false;
            }

            int month = int.Parse(period.Substring(5, 2));
            return month >= 1 && month <= 12;
        }

        // ----- Getter -----
        public string Period => period;
        public IReadOnlyList<Employee> Employees => employees;
        public int EmployeeCount => employees.Count;

        /// <summary>
        /// Thêm nhân sự vào bảng lương. Trả về false nếu mã nhân sự đã tồn tại
        /// (giữ bất biến "không trùng mã trong một bảng lương").
        /// </summary>
        public bool AddEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee), "Nhân sự không được null.");
            if (FindEmployee(employee.EmployeeId) != null)
                return false;

            employees.Add(employee);
            return true;
        }

        /// <summary>Tìm nhân sự theo mã; trả về null nếu không tìm thấy.</summary>
        public Employee? FindEmployee(string employeeId)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
                return null;

            foreach (Employee e in employees)
            {
                if (string.Equals(e.EmployeeId, employeeId.Trim(), StringComparison.OrdinalIgnoreCase))
                    return e;
            }

            return null;
        }

        /// <summary>
        /// Tổng thu nhập của cả bảng lương. Các phép tổng hợp đều gọi CalculateGrossPay()
        /// qua kiểu chung Employee (gọi đa hình), không dùng chuỗi if/else theo loại nhân sự.
        /// Bảng lương rỗng trả về 0.
        /// </summary>
        public double CalculateTotalPayroll()
        {
            double total = 0;
            foreach (Employee e in employees)
                total += e.CalculateGrossPay();
            return total;
        }

        /// <summary>Tổng thu nhập của một phòng ban; trả về 0 nếu phòng ban không có nhân sự nào.</summary>
        public double CalculatePayrollByDepartment(string department)
        {
            if (string.IsNullOrWhiteSpace(department))
                throw new ArgumentException("Phòng ban cần tính tổng không được rỗng.");

            double total = 0;
            foreach (Employee e in employees)
            {
                // So sánh không phân biệt chữ hoa/chữ thường để tránh lỗi nhập liệu
                if (string.Equals(e.Department, department.Trim(), StringComparison.OrdinalIgnoreCase))
                    total += e.CalculateGrossPay();
            }

            return total;
        }

        /// <summary>Tìm nhân sự có thu nhập cao nhất; trả về null nếu bảng lương rỗng.</summary>
        public Employee? FindHighestPaidEmployee()
        {
            Employee? highest = null;
            foreach (Employee e in employees)
            {
                if (highest == null || e.CalculateGrossPay() > highest.CalculateGrossPay())
                    highest = e;
            }

            return highest;
        }

        /// <summary>
        /// In bảng lương của kỳ lương: mỗi nhân sự được in bằng lời gọi đa hình DisplayPayrollInfo().
        /// </summary>
        public void DisplayPayroll()
        {
            Console.WriteLine($"=== BẢNG LƯƠNG KỲ {period} ===");
            if (employees.Count == 0)
            {
                Console.WriteLine("(Bảng lương chưa có nhân sự nào)");
                return;
            }

            foreach (Employee e in employees)
            {
                e.DisplayPayrollInfo();   // đa hình: gọi phiên bản của lớp thực tế
            }

            Console.WriteLine($"Tổng bảng lương: {CalculateTotalPayroll():N0} VND");
        }
    }
}
