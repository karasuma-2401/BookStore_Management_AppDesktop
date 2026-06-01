using CommunityToolkit.Mvvm.ComponentModel;

namespace BookStore_Management_AppDesktop.Models.DTOs.ShiftDTOs
{
    public partial class EmployeeShiftResponseDto : ObservableObject
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ShiftName { get; set; } = string.Empty;
        public string WorkTime { get; set; } = string.Empty;
        public string WorkDate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? CheckInTime { get; set; }
        public bool IsPaid { get; set; }

        [ObservableProperty]
        private bool isSelected;
    }
}

