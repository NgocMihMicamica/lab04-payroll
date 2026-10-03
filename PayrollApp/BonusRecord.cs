/****************/
// Mã sinh viên: 202418947
// Họ tên:     Đặng Ngọc Minh
// Bài thực hành: Lab 04 - Kế thừa: Hệ thống tính lương và thưởng nhân sự
/****************/

using System;

namespace PayrollApp
{
    /// <summary>
    /// Loại khoản thưởng: cố định một số tiền, hoặc tính theo tỷ lệ của một giá trị tham chiếu.
    /// </summary>
    public enum BonusKind
    {
        Fixed,        // thưởng một số tiền cố định
        Percentage    // thưởng theo tỷ lệ của một giá trị tham chiếu
    }

    /// <summary>
    /// Lớp BonusRecord - một khoản thưởng trong kỳ lương.
    /// Trách nhiệm: ghi lại một khoản thưởng (số tiền, lý do, cách tính) và tự kiểm tra bất biến
    /// ngay khi tạo, nhờ đó lớp Employee không phải lặp lại logic kiểm tra ở cả ba phiên bản AddBonus().
    /// Đối tượng là BẤT BIẾN: tạo xong thì không sửa được, muốn đổi phải thêm khoản thưởng mới.
    /// Quan hệ: được Employee sở hữu theo quan hệ kết tập (composition) trong danh sách lịch sử thưởng.
    /// </summary>
    public class BonusRecord
    {
        // ----- Thuộc tính -----
        private readonly BonusKind kind;            // cách tính khoản thưởng
        private readonly double amount;             // số tiền thực nhận (đã tính xong)
        private readonly string reason;             // lý do thưởng (không rỗng)
        private readonly double rate;               // tỷ lệ thưởng (0 nếu là thưởng cố định)
        private readonly double referenceAmount;    // giá trị tham chiếu (0 nếu là thưởng cố định)

        // ----- Constructor nạp chồng 1: thưởng một số tiền cố định kèm lý do -----
        public BonusRecord(double amount, string reason)
            : this(BonusKind.Fixed, amount, 0, 0, reason)
        {
        }

        // ----- Constructor nạp chồng 2: thưởng theo tỷ lệ của một giá trị tham chiếu -----
        public BonusRecord(double rate, double referenceAmount, string reason)
            : this(BonusKind.Percentage, rate * referenceAmount, rate, referenceAmount, reason)
        {
        }

        // ----- Constructor đầy đủ (private): nơi duy nhất kiểm tra bất biến của khoản thưởng -----
        private BonusRecord(BonusKind kind, double amount, double rate, double referenceAmount, string reason)
        {
            if (kind == BonusKind.Fixed)
            {
                // Bất biến: khoản thưởng cố định phải lớn hơn 0
                if (amount <= 0)
                    throw new ArgumentException("Số tiền thưởng phải lớn hơn 0.");
            }
            else
            {
                // Bất biến: 0 < tỷ lệ <= 0,5 và giá trị tham chiếu phải lớn hơn 0
                if (rate <= 0 || rate > 0.5)
                    throw new ArgumentException("Tỷ lệ thưởng phải lớn hơn 0 và không quá 0,5.");
                if (referenceAmount <= 0)
                    throw new ArgumentException("Giá trị tham chiếu phải lớn hơn 0.");
            }

            // Bất biến: lý do thưởng không được rỗng
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Lý do thưởng không được rỗng.");

            this.kind = kind;
            this.amount = amount;
            this.rate = rate;
            this.referenceAmount = referenceAmount;
            this.reason = reason.Trim();
        }

        // ----- Getter -----
        public BonusKind Kind => kind;
        public double Amount => amount;
        public string Reason => reason;
        public double Rate => rate;
        public double ReferenceAmount => referenceAmount;

        /// <summary>
        /// Mô tả khoản thưởng để in ra bảng lương, ví dụ:
        /// "1,000,000 VND (Thưởng hoàn thành kế hoạch tháng 9)" hoặc
        /// "2% × 50,000,000 = 1,000,000 VND (Thưởng theo tỷ lệ doanh số)".
        /// </summary>
        public string Describe()
        {
            if (kind == BonusKind.Fixed)
                return $"{amount:N0} VND ({reason})";

            return $"{rate * 100:0.##}% × {referenceAmount:N0} = {amount:N0} VND ({reason})";
        }
    }
}
