# Lab 04 — Kế thừa (Inheritance): Hệ thống tính lương và thưởng nhân sự

| Thông tin | Nội dung |
| --- | --- |
| Mã sinh viên | 202418947 |
| Họ và tên | Đặng Ngọc Minh |
| Ngôn ngữ / nền tảng | C# — .NET 8 (Console Application, mã hoá UTF-8) |
| Liên kết GitHub | `https://github.com/NgocMihMicamica/lab04-payroll` |
| Báo cáo | `docs/BaoCao-Lab04.pdf` (bản HTML sinh PDF: `docs/BaoCao-Lab04.html`) |
| Sơ đồ lớp | `docs/class_diagram.png` (nguồn: `docs/class_diagram.svg` / `.html`) |
| Kết quả chạy | `run_output.txt` — 54/54 tình huống kiểm thử ĐÚNG |

## 1. Nội dung bài thực hành

- **Phần A — Phân tích và thiết kế:** xác định các lớp `Employee` (cơ sở trừu tượng), `SalariedEmployee`, `HourlyEmployee`, `SalesEmployee`, `Payroll` và lớp phụ trợ `BonusRecord`; mô tả trách nhiệm, thuộc tính, constructor, phương thức, quan hệ; nêu các bất biến của mô hình; giải thích các quyết định kế thừa; vẽ sơ đồ lớp.
- **Phần B — Cài đặt:** nạp chồng constructor (2 constructor ở lớp cơ sở, 2 constructor ở mỗi lớp dẫn xuất), nạp chồng ba phiên bản `AddBonus()`, ghi đè `CalculateGrossPay()` / `GetEmployeeType()` / `DisplayPayrollInfo()`, kiểm tra bất biến và quan hệ kết tập `Payroll` — `Employee`.
- **Phần C — Kiểm thử:** `Program.cs` dựng dữ liệu mẫu của đề bài (E001…E004), kiểm tra thu nhập từng người, tổng bảng lương, tổng theo phòng ban, người thu nhập cao nhất và 38 tình huống biên/lỗi.

## 2. Cấu trúc thư mục

```
PayrollApp/
├── Employee.cs          # lớp cơ sở trừu tượng: 2 constructor, 3 phiên bản AddBonus, thành viên ảo/trừu tượng
├── BonusRecord.cs       # một khoản thưởng (bất biến): 2 constructor nạp chồng + constructor private kiểm tra bất biến
├── SalariedEmployee.cs  # kế thừa Employee: lương tháng + phụ cấp trách nhiệm
├── HourlyEmployee.cs    # kế thừa Employee: đơn giá giờ, số giờ làm, giờ vượt ngưỡng 160
├── SalesEmployee.cs     # kế thừa Employee: lương cơ bản, doanh số, tỷ lệ hoa hồng
├── Payroll.cs           # bảng lương: kết tập List<Employee>, tổng hợp bằng lời gọi đa hình
├── Program.cs           # dữ liệu mục C + kiểm thử biên/lỗi + tổng kết
└── PayrollApp.csproj    # .NET 8, ImplicitUsings + Nullable enable
docs/
├── BaoCao-Lab04.pdf / .html / .docx  # báo cáo bài thực hành
├── class_diagram.png / .svg / .html  # sơ đồ lớp
└── screenshots/                      # ảnh chụp biên dịch và chạy chương trình
run_output.txt            # kết quả chạy đầy đủ (54 tình huống)
_lab_instructions.txt     # nội dung đề bài (bản chép lại từ PDF)
_tools/                   # script hỗ trợ: biên dịch, chạy, chụp ảnh, sinh báo cáo, vẽ sơ đồ, đẩy GitHub
README.md                 # tệp này
```

## 3. Yêu cầu và cách chạy

- Cần **.NET SDK 8.0** trở lên (kiểm tra bằng `dotnet --version`).
- Biên dịch:

```bash
dotnet build PayrollApp/PayrollApp.csproj
```

- Chạy chương trình kiểm thử:

```bash
dotnet run --project PayrollApp/PayrollApp.csproj
```

- Hoặc vào trực tiếp thư mục dự án rồi chạy:

```bash
cd PayrollApp
dotnet run
```

## 4. Các điểm nạp chồng / ghi đè chính

| Lớp | Constructor nạp chồng | Phương thức nạp chồng / ghi đè |
| --- | --- | --- |
| `Employee` | `Employee(employeeId, fullName)`, `Employee(employeeId, fullName, department)` (đều `protected` vì lớp trừu tượng) | `AddBonus(amount)`, `AddBonus(amount, reason)`, `AddBonus(rate, referenceAmount, reason)`; `CalculateGrossPay()`, `GetEmployeeType()` là `abstract`; `DisplayPayrollInfo()` là `virtual`; `ResetBonus()` |
| `BonusRecord` | `BonusRecord(amount, reason)`, `BonusRecord(rate, referenceAmount, reason)` (ủy quyền cho constructor `private` kiểm tra bất biến) | `Describe()` |
| `SalariedEmployee` | `SalariedEmployee(id, fullName, monthlySalary)`, `SalariedEmployee(id, fullName, department, monthlySalary, allowance)` | ghi đè `CalculateGrossPay()` = lương tháng + phụ cấp + thưởng; ghi đè `GetEmployeeType()`, `DisplayPayrollInfo()` |
| `HourlyEmployee` | `HourlyEmployee(id, fullName, hourlyRate)`, `HourlyEmployee(id, fullName, department, hourlyRate, workedHours)` | `UpdateWorkedHours()`, `UpdateHourlyRate()`, `CalculateBasePay()`; ghi đè `CalculateGrossPay()` theo ngưỡng 160 giờ (hệ số 1,5) |
| `SalesEmployee` | `SalesEmployee(id, fullName, baseSalary)`, `SalesEmployee(id, fullName, department, baseSalary, salesRevenue, commissionRate)` | `UpdateSalesRevenue()`, `UpdateCommissionRate()`; ghi đè `CalculateGrossPay()` = lương cơ bản + hoa hồng + thưởng |
| `Payroll` | `Payroll(period)` | `AddEmployee()`, `FindEmployee()`, `CalculateTotalPayroll()`, `CalculatePayrollByDepartment()`, `FindHighestPaidEmployee()`, `DisplayPayroll()` |

## 5. Bất biến của mô hình

- Mã nhân sự, họ tên, phòng ban không được rỗng.
- Mọi khoản thưởng phải dương; tỷ lệ thưởng trong khoảng `0 < rate <= 0,5`; giá trị tham chiếu phải dương; lý do thưởng (nếu có) không rỗng.
- Lương tháng và phụ cấp trách nhiệm không âm; đơn giá giờ không âm; số giờ làm trong khoảng 0…250.
- Lương cơ bản và doanh số không âm; tỷ lệ hoa hồng trong khoảng 0…0,3.
- Không có hai nhân sự cùng mã trong một bảng lương; kỳ lương không rỗng và đúng dạng `yyyy-MM`.
- `Payroll` không kế thừa `Employee`; các phép tổng hợp chỉ gọi `CalculateGrossPay()` qua kiểu `Employee`.

## 6. Kiểm thử

Chương trình kiểm tra 54 tình huống và tất cả đều cho kết quả đúng như mong đợi:

| Nhóm tình huống | Số lượng | Kết quả |
| --- | --- | --- |
| Dữ liệu mục C (C1…C16) | 16 | 16 ĐÚNG |
| Biên và kiểm thử lỗi (B1…B38) | 38 | 38 ĐÚNG |
| **Tổng** | **54** | **54 ĐÚNG, 0 SAI** |

Dữ liệu mẫu của đề bài và kết quả:

| Nhân sự | Loại | Dữ liệu lương | Thưởng | Thu nhập mong đợi | Quan sát |
| --- | --- | --- | --- | --- | --- |
| E001 Nguyễn Minh An | Lương cố định | 15.000.000 + phụ cấp 2.000.000 | 1.000.000 | 18.000.000 | 18.000.000 ✓ |
| E002 Trần Thu Bình | Theo giờ | 100.000/giờ × 150 giờ | 500.000 | 15.500.000 | 15.500.000 ✓ |
| E003 Lê Hoàng Chi | Theo giờ | 100.000/giờ × 170 giờ (10 giờ vượt ngưỡng) | 0 | 17.500.000 | 17.500.000 ✓ |
| E004 Phạm Quốc Dũng | Kinh doanh | 8.000.000 + 200.000.000 × 5% | 2% của 50.000.000 | 19.000.000 | 19.000.000 ✓ |

Tổng bảng lương kỳ 2026-09: **70.000.000 VND**; tổng của phòng *Hỗ trợ*: **33.000.000 VND**; người thu nhập cao nhất: **E004**. Kết quả chạy đầy đủ ở `run_output.txt` và trong Phụ lục A của báo cáo.

## 7. Ghi chú kỹ thuật

- Chương trình đặt `Console.OutputEncoding = Encoding.UTF8` và cố định văn hoá `vi-VN` để định dạng tiền tệ ổn định (ví dụ `15.000.000 VND`); các tệp mã nguồn lưu ở dạng UTF-8 để hiển thị đúng tiếng Việt.
- `Employee` là lớp trừu tượng nên không thể viết `new Employee(...)`; hai constructor của lớp cơ sở được khai báo `protected` để lớp dẫn xuất uỷ quyền bằng `base(...)`.
- `BonusRecord` là lớp bất biến và `Employee` sở hữu danh sách khoản thưởng (composition) — nhờ đó `MonthlyBonus` luôn khớp với lịch sử thưởng và không cần nhiều mảng song song.
- `Payroll` giữ `List<Employee>` (kết tập, không sở hữu) nên huỷ bảng lương không huỷ nhân sự; tính lương luôn qua lời gọi đa hình, không có `if/else` theo loại nhân sự.
- Thư mục `_tools/` chứa script hỗ trợ (không thuộc phần nộp chính): `build-run.ps1` (biên dịch + chạy, lưu `run_output.txt`), `cmd/build.cmd`, `cmd/run.cmd`, `cmd/build-capture.cmd`, `cmd/run-capture.cmd`, `capture-shots.ps1` + `ConCapture.cs` (chụp ảnh màn hình cửa sổ console biên dịch/chạy), `make-diagram.mjs` (sinh sơ đồ lớp SVG/HTML/PNG bằng bun), `make-report.ps1` (sinh báo cáo HTML), `push-github.ps1` (tạo và đẩy mã nguồn lên GitHub bằng Personal Access Token).
- Báo cáo được sinh theo hai bước: `make-report.ps1` tạo `docs/BaoCao-Lab04.html`, sau đó in sang PDF bằng trình duyệt ở chế độ headless (Microsoft Edge):
  `msedge --headless=new --no-pdf-header-footer --print-to-pdf=docs\BaoCao-Lab04.pdf file:///.../docs/BaoCao-Lab04.html`.

