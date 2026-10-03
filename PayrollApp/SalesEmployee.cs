/****************/
// Mã sinh viên: 202418947
// Họ tên:     Đặng Ngọc Minh
// Bài thực hành: Lab 04 - Kế thừa: Hệ thống tính lương và thưởng nhân sự
/****************/

using System;

namespace PayrollApp
{
    /// <summary>
    /// Lớp SalesEmployee - nhân viên kinh doanh hưởng lương cơ bản và hoa hồng, KẾ THỪA từ Employee.
    /// Trách nhiệm: quản lý lương cơ bản, doanh số và tỷ lệ hoa hồng; tính tiền hoa hồng;
    /// cho phép cập nhật doanh số có kiểm soát; cài đặt công thức thu nhập riêng.
    /// Công thức: grossPay = baseSalary + salesRevenue × commissionRate + monthlyBonus.
    /// Bất biến: lương cơ bản >= 0; doanh số >= 0; 0 <= tỷ lệ hoa hồng <= 0,3.
    /// </summary>
    public class SalesEmployee : Employee
    {
        // ----- Hằng số quy tắc -----
        public const double MaxCommissionRate = 0.3;   // tỷ lệ hoa hồng tối đa theo đề bài

        // ----- Thuộc tính bổ sung -----
        private double baseSalary;       // Lương cơ bản (không âm)
        private double salesRevenue;     // Doanh số trong tháng (không âm)
        private double commissionRate;   // Tỷ lệ hoa hồng (0..0,3)

        // ----- Constructor rút gọn: chưa có doanh số và chưa có tỷ lệ hoa hồng -----
        public SalesEmployee(string employeeId, string fullName, double baseSalary)
            : this(employeeId, fullName, DefaultDepartment, baseSalary, 0, 0)
        {
        }

        // ----- Constructor đầy đủ -----
        public SalesEmployee(string employeeId, string fullName, string department,
                             double baseSalary, double salesRevenue, double commissionRate)
            : base(employeeId, fullName, department)
        {
            Validate(baseSalary, salesRevenue, commissionRate);
            this.baseSalary = baseSalary;
            this.salesRevenue = salesRevenue;
            this.commissionRate = commissionRate;
        }

        // ----- Kiểm tra bất biến riêng của lớp -----
        private static void Validate(double baseSalary, double salesRevenue, double commissionRate)
        {
            if (baseSalary < 0)
                throw new ArgumentException("Lương cơ bản không được âm.");
            if (salesRevenue < 0)
                throw new ArgumentException("Doanh số không được âm.");
            if (commissionRate < 0 || commissionRate > MaxCommissionRate)
                throw new ArgumentException($"Tỷ lệ hoa hồng phải trong khoảng 0 đến {MaxCommissionRate:0.0#}.");
        }

        // ----- Getter -----
        public double BaseSalary => baseSalary;
        public double SalesRevenue => salesRevenue;
        public double CommissionRate => commissionRate;

        /// <summary>Tiền hoa hồng của tháng; tính từ doanh số và tỷ lệ hiện có nên không lưu riêng.</summary>
        public double CommissionAmount => salesRevenue * commissionRate;

        /// <summary>Cập nhật doanh số có kiểm soát (dùng khi chốt doanh số cuối tháng).</summary>
        public void UpdateSalesRevenue(double newSalesRevenue)
        {
            Validate(baseSalary, newSalesRevenue, commissionRate);
            salesRevenue = newSalesRevenue;
        }

        /// <summary>Cập nhật tỷ lệ hoa hồng có kiểm soát.</summary>
        public void UpdateCommissionRate(double newCommissionRate)
        {
            Validate(baseSalary, salesRevenue, newCommissionRate);
            commissionRate = newCommissionRate;
        }

        // ----- Ghi đè công thức thu nhập của loại nhân sự này -----
        public override double CalculateGrossPay()
        {
            return baseSalary + CommissionAmount + MonthlyBonus;
        }

        // ----- Ghi đè tên loại nhân sự -----
        public override string GetEmployeeType()
        {
            return "Nhân viên kinh doanh";
        }

        // ----- Ghi đè hiển thị: nêu rõ lương cơ bản và phần hoa hồng -----
        public override void DisplayPayrollInfo()
        {
            Console.WriteLine($"   Thành phần: lương cơ bản {baseSalary:N0}"
                              + $" + hoa hồng {commissionRate * 100:0.##}% × doanh số {salesRevenue:N0}"
                              + $" = {CommissionAmount:N0}"
                              + $" + thưởng {MonthlyBonus:N0}");
            base.DisplayPayrollInfo();
        }
    }
}
