using System.ComponentModel.DataAnnotations;

namespace EmployeeLeaveManagement.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        public string EmployeeEmail { get; set; } = string.Empty;

        [Required]
        public string LeaveType { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";
    }
}