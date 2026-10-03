/****************/
// Mã sinh viên: 202418947
// Họ tên:     Đặng Ngọc Minh
// Bài thực hành: Lab 04 - Kế thừa: Hệ thống tính lương và thưởng nhân sự
/****************/

using System;

namespace PayrollApp
{
    /// <summary>
    /// Lớp SalariedEmployee - nhân viên hưởng lương cố định, KẾ THỪA từ Employee.
    /// Trách nhiệm: quản lý lương tháng và phụ cấp trách nhiệm; cài đặt công thức
    /// thu nhập riêng của loại nhân sự này.
    /// Công thức: grossPay = monthlySalary + responsibilityAllowance + monthlyBonus.
    /// Quan hệ: kế thừa (generalization) Employee - "là một" nhân sự, dùng lại phần
    /// thông tin chung và ba phiên bản AddBonus() của lớp cơ sở.
    /// Bất biến: lương tháng >= 0, phụ cấp trách nhiệm >= 0.
    /// </summary>
    public class SalariedEmployee : Employee
    {
        // ----- Thuộc tính bổ sung -----
        private double monthlySalary;             // Lương cố định tháng (không âm)
        private double responsibilityAllowance;   // Phụ cấp trách nhiệm (không âm)

        // ----- Constructor rút gọn: dùng giá trị mặc định hợp lý -----
        // Chưa xác định phòng ban (DefaultDepartment) và chưa có phụ cấp trách nhiệm.
        public SalariedEmployee(string employeeId, string fullName, double monthlySalary)
            : this(employeeId, fullName, DefaultDepartment, monthlySalary, 0)
        {
        }

        // ----- Constructor đầy đủ: ủy quyền cho constructor rút gọn, dùng lại phần kiểm tra -----
        public SalariedEmployee(string employeeId, string fullName, string department,
                                double monthlySalary, double responsibilityAllowance)
            : base(employeeId, fullName, department)
        {
            Validate(monthlySalary, responsibilityAllowance);
            this.monthlySalary = monthlySalary;
            this.responsibilityAllowance = responsibilityAllowance;
        }

        // ----- Kiểm tra bất biến riêng của lớp -----
        private static void Validate(double monthlySalary, double allowance)
        {
            if (monthlySalary < 0)
                throw new ArgumentException("Lương tháng không được âm.");
            if (allowance < 0)
                throw new ArgumentException("Phụ cấp trách nhiệm không được âm.");
        }

        // ----- Getter -----
        public double MonthlySalary => monthlySalary;
        public double ResponsibilityAllowance => responsibilityAllowance;

        // ----- Ghi đè công thức thu nhập của loại nhân sự này -----
        public override double CalculateGrossPay()
        {
            return monthlySalary + responsibilityAllowance + MonthlyBonus;
        }

        // ----- Ghi đè tên loại nhân sự -----
        public override string GetEmployeeType()
        {
            return "Nhân viên lương cố định";
        }

        // ----- Ghi đè hiển thị: in các thành phần thu nhập rồi gọi lại lớp cơ sở -----
        public override void DisplayPayrollInfo()
        {
            Console.WriteLine($"   Thành phần: lương tháng {monthlySalary:N0}"
                              + $" + phụ cấp trách nhiệm {responsibilityAllowance:N0}"
                              + $" + thưởng {MonthlyBonus:N0}");
            base.DisplayPayrollInfo();
        }
    }
}
