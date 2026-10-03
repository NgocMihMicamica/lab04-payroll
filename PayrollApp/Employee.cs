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
    /// Lớp cơ sở Employee (nhân sự).
    /// Trách nhiệm: giữ thông tin chung (mã, họ tên, phòng ban), quản lý lịch sử thưởng của kỳ lương,
    /// cung cấp ba phiên bản NẠP CHỒNG AddBonus() và khai báo các hành vi chung
    /// (CalculateGrossPay, GetEmployeeType, DisplayPayrollInfo) để lớp dẫn xuất GHI ĐÈ.
    /// Quyết định thiết kế: Employee là lớp TRỪU TƯỢNG vì không có công thức thu nhập chung cho mọi
    /// loại nhân sự; nhờ vậy không thể tạo một nhân sự "không rõ loại lương" và trình biên dịch
    /// buộc mọi lớp dẫn xuất phải cài đặt công thức riêng.
    /// Bất biến: mã, họ tên, phòng ban không được rỗng.
    /// </summary>
    public abstract class Employee
    {
        // ----- Giá trị mặc định dùng cho constructor rút gọn của các lớp dẫn xuất -----
        public const string DefaultDepartment = "Unassigned";
        public const string DefaultBonusReason = "Thưởng cố định";

        // ----- Thuộc tính -----
        private readonly string employeeId;                  // Mã nhân sự (bất biến: không rỗng)
        private readonly string fullName;                    // Họ tên  (bất biến: không rỗng)
        private string department;                           // Phòng ban (bất biến: không rỗng, đổi qua phương thức có kiểm soát)
        private readonly List<BonusRecord> bonusHistory;      // Lịch sử thưởng của kỳ lương hiện tại

        // ----- Constructor nạp chồng 1: chỉ có mã và họ tên, phòng ban nhận giá trị mặc định -----
        // Khai báo protected vì Employee là lớp trừu tượng: lớp dẫn xuất ủy quyền qua base(...)
        protected Employee(string employeeId, string fullName)
            : this(employeeId, fullName, DefaultDepartment)
        {
        }

        // ----- Constructor nạp chồng 2: đầy đủ thông tin chung -----
        // Là nơi duy nhất kiểm tra bất biến chung; constructor phía trên ủy quyền cho nó
        // nên không phải lặp lại logic kiểm tra.
        protected Employee(string employeeId, string fullName, string department)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
                throw new ArgumentException("Mã nhân sự không được rỗng.");
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Họ tên không được rỗng.");
            if (string.IsNullOrWhiteSpace(department))
                throw new ArgumentException("Phòng ban không được rỗng.");

            this.employeeId = employeeId.Trim();
            this.fullName = fullName.Trim();
            this.department = department.Trim();
            this.bonusHistory = new List<BonusRecord>();   // tháng mới: tổng thưởng = 0
        }

        // ----- Getter -----
        public string EmployeeId => employeeId;
        public string FullName => fullName;
        public string Department => department;

        /// <summary>Tổng thưởng trong tháng: cộng dồn từ lịch sử thưởng nên luôn khớp với các khoản đã ghi.</summary>
        public double MonthlyBonus
        {
            get
            {
                double total = 0;
                foreach (BonusRecord record in bonusHistory)
                    total += record.Amount;
                return total;
            }
        }

        /// <summary>Lịch sử thưởng (chỉ đọc) để giải thích tổng thưởng.</summary>
        public IReadOnlyList<BonusRecord> BonusHistory => bonusHistory;

        /// <summary>Đổi phòng ban có kiểm soát.</summary>
        public void ChangeDepartment(string newDepartment)
        {
            if (string.IsNullOrWhiteSpace(newDepartment))
                throw new ArgumentException("Phòng ban không được rỗng.");
            department = newDepartment.Trim();
        }

        // ================= NẠP CHỒNG PHƯƠNG THỨC addBonus() =================
        // Ba phiên bản cùng tên, khác danh sách tham số; trình biên dịch chọn phiên bản
        // ngay tại thời điểm biên dịch dựa vào kiểu và số lượng đối số truyền vào.

        // ----- Nạp chồng 1: thưởng một khoản cố định, dùng lý do mặc định -----
        public void AddBonus(double amount)
        {
            AddBonus(amount, DefaultBonusReason);
        }

        // ----- Nạp chồng 2: thưởng một khoản cố định kèm lý do -----
        public void AddBonus(double amount, string reason)
        {
            // BonusRecord kiểm tra bất biến (số tiền > 0, lý do không rỗng) nên không lặp lại ở đây
            bonusHistory.Add(new BonusRecord(amount, reason));
        }

        // ----- Nạp chồng 3: thưởng theo tỷ lệ của một giá trị tham chiếu, kèm lý do -----
        // Ràng buộc theo đề bài: 0 < rate <= 0,5 và referenceAmount > 0
        public void AddBonus(double rate, double referenceAmount, string reason)
        {
            bonusHistory.Add(new BonusRecord(rate, referenceAmount, reason));
        }

        /// <summary>Đặt lại thưởng khi bắt đầu kỳ lương mới (xóa toàn bộ lịch sử thưởng).</summary>
        public void ResetBonus()
        {
            bonusHistory.Clear();
        }

        /// <summary>In lịch sử thưởng để giải thích cho tổng thưởng trong tháng.</summary>
        public void DisplayBonusHistory()
        {
            if (bonusHistory.Count == 0)
            {
                Console.WriteLine($"   {employeeId}: chưa có khoản thưởng nào trong kỳ lương.");
                return;
            }

            for (int i = 0; i < bonusHistory.Count; i++)
                Console.WriteLine($"   {employeeId} - khoản {i + 1}: {bonusHistory[i].Describe()}");

            Console.WriteLine($"   {employeeId} - tổng thưởng: {MonthlyBonus:N0} VND");
        }

        // ================= HÀNH VI CHUNG (lớp dẫn xuất ghi đè) =================

        /// <summary>Thu nhập trước khấu trừ theo công thức của từng loại nhân sự.</summary>
        public abstract double CalculateGrossPay();

        /// <summary>Tên loại nhân sự, dùng khi in bảng lương.</summary>
        public abstract string GetEmployeeType();

        /// <summary>
        /// In một dòng thông tin lương. Đây là phương thức ảo: lớp dẫn xuất ghi đè để in thêm
        /// các thành phần thu nhập riêng rồi gọi lại phiên bản của lớp cơ sở.
        /// </summary>
        public virtual void DisplayPayrollInfo()
        {
            Console.WriteLine($"[{GetEmployeeType()}] {employeeId} - {fullName}"
                              + $" | Phòng: {department}"
                              + $" | Thưởng: {MonthlyBonus:N0} VND"
                              + $" | Thu nhập: {CalculateGrossPay():N0} VND");
        }
    }
}
