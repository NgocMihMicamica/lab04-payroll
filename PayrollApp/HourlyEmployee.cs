/****************/
// Mã sinh viên: 202418947
// Họ tên:     Đặng Ngọc Minh
// Bài thực hành: Lab 04 - Kế thừa: Hệ thống tính lương và thưởng nhân sự
/****************/

using System;

namespace PayrollApp
{
    /// <summary>
    /// Lớp HourlyEmployee - nhân viên hưởng lương theo giờ, KẾ THỪA từ Employee.
    /// Trách nhiệm: quản lý đơn giá giờ và số giờ làm trong tháng; tách giờ thường và
    /// giờ vượt ngưỡng 160 giờ; cài đặt công thức thu nhập riêng.
    /// Công thức:
    ///   nếu workedHours <= 160: basePay = workedHours × hourlyRate
    ///   nếu workedHours >  160: basePay = 160 × hourlyRate + (workedHours - 160) × hourlyRate × 1,5
    ///   grossPay = basePay + monthlyBonus
    /// Lưu ý thiết kế: KHÔNG lưu riêng tiền làm thêm vì có thể tính lại từ số giờ hiện có.
    /// Bất biến: đơn giá giờ >= 0; 0 <= số giờ làm <= 250.
    /// </summary>
    public class HourlyEmployee : Employee
    {
        // ----- Hằng số quy tắc tính lương -----
        public const double StandardHours = 160;    // ngưỡng giờ chuẩn trong tháng
        public const double OvertimeFactor = 1.5;   // hệ số cho giờ vượt ngưỡng
        public const double MaxHours = 250;         // số giờ làm hợp lệ tối đa

        // ----- Thuộc tính bổ sung -----
        private double hourlyRate;    // Đơn giá giờ (không âm)
        private double workedHours;   // Số giờ làm trong tháng (0..250)

        // ----- Constructor rút gọn: chưa có dữ liệu chấm công nên số giờ = 0 -----
        public HourlyEmployee(string employeeId, string fullName, double hourlyRate)
            : this(employeeId, fullName, DefaultDepartment, hourlyRate, 0)
        {
        }

        // ----- Constructor đầy đủ -----
        public HourlyEmployee(string employeeId, string fullName, string department,
                              double hourlyRate, double workedHours)
            : base(employeeId, fullName, department)
        {
            Validate(hourlyRate, workedHours);
            this.hourlyRate = hourlyRate;
            this.workedHours = workedHours;
        }

        // ----- Kiểm tra bất biến riêng của lớp -----
        private static void Validate(double hourlyRate, double workedHours)
        {
            if (hourlyRate < 0)
                throw new ArgumentException("Đơn giá giờ không được âm.");
            if (workedHours < 0 || workedHours > MaxHours)
                throw new ArgumentException($"Số giờ làm phải trong khoảng 0 đến {MaxHours:N0} giờ.");
        }

        // ----- Getter -----
        public double HourlyRate => hourlyRate;
        public double WorkedHours => workedHours;

        /// <summary>Số giờ thường (tối đa bằng ngưỡng 160 giờ).</summary>
        public double NormalHours => Math.Min(workedHours, StandardHours);

        /// <summary>Số giờ vượt ngưỡng; tính từ trạng thái hiện có nên không cần thuộc tính lưu riêng.</summary>
        public double OvertimeHours => Math.Max(0, workedHours - StandardHours);

        /// <summary>Cập nhật số giờ làm có kiểm soát (dùng khi chốt công cuối tháng).</summary>
        public void UpdateWorkedHours(double newWorkedHours)
        {
            Validate(hourlyRate, newWorkedHours);
            workedHours = newWorkedHours;
        }

        /// <summary>Cập nhật đơn giá giờ có kiểm soát.</summary>
        public void UpdateHourlyRate(double newHourlyRate)
        {
            Validate(newHourlyRate, workedHours);
            hourlyRate = newHourlyRate;
        }

        /// <summary>Tiền lương theo giờ trước khi cộng thưởng (giờ thường + giờ vượt ngưỡng).</summary>
        public double CalculateBasePay()
        {
            return NormalHours * hourlyRate + OvertimeHours * hourlyRate * OvertimeFactor;
        }

        // ----- Ghi đè công thức thu nhập của loại nhân sự này -----
        public override double CalculateGrossPay()
        {
            return CalculateBasePay() + MonthlyBonus;
        }

        // ----- Ghi đè tên loại nhân sự -----
        public override string GetEmployeeType()
        {
            return "Nhân viên theo giờ";
        }

        // ----- Ghi đè hiển thị: nêu rõ phần giờ thường và giờ vượt ngưỡng -----
        public override void DisplayPayrollInfo()
        {
            Console.WriteLine($"   Thành phần: {NormalHours:N0} giờ thường × {hourlyRate:N0}"
                              + $" + {OvertimeHours:N0} giờ vượt ngưỡng × {hourlyRate:N0} × {OvertimeFactor}"
                              + $" = {CalculateBasePay():N0}"
                              + $" + thưởng {MonthlyBonus:N0}");
            base.DisplayPayrollInfo();
        }
    }
}
