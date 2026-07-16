using System.ComponentModel.DataAnnotations;

namespace EmployeeLeaveManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public int AvailableLeaves { get; set; } = 5;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;

        public string Role { get; set; } = "Employee";
    }
}
