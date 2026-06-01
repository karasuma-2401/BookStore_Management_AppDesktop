# TODO - ShiftSchedule Day Detail (3 ca / ngày)

## Backend (API)
- [ ] Thêm DTO: ShiftDayDetailResponseDto + ShiftDayItemDto (hoặc tương đương)
- [ ] Thêm endpoint: GET /employeeshift/day-detail?date=yyyy-MM-dd
- [ ] Implement EmployeeShiftService.GetDayDetailAsync(date)

## Client (Desktop)
- [ ] Thêm DTO tương ứng trong Models/DTOs/ShiftDTOs
- [ ] Mở rộng IEmployeeShiftApiService + EmployeeShiftApiService: GetDayDetailAsync(date)
- [ ] Tạo DayDetailDialog (XAML + code-behind hoặc ViewModel) để hiển thị đúng 3 ca và người đảm nhiệm
- [ ] Thêm command trong ShiftScheduleViewModel: OpenDayDetail(date)
- [ ] Sửa ShiftSchedulePage.xaml để click cell ngày gọi OpenDayDetailCommand (giữ nguyên grid)

## Assign vào dialog
- [ ] Trong DayDetailDialog, khi ca trống: hiển thị dropdown chọn employee + nút Assign
- [ ] Khi click Assign: gọi AssignShiftToDate(employeeId, shiftId, date) và refresh detail

## Kiểm thử
- [ ] Build API
- [ ] Chạy WPF: click 1 ngày => hiện đúng 3 ca, ca đã phân hiển thị đúng người
- [ ] Assign ca trống => refresh và cập nhật status

