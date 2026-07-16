using EmployeeLeaveManagement.Models;
using Microsoft.AspNetCore.Mvc;
using EmployeeLeaveManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EmployeeLeaveManagement.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Display the registration form
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Receive the submitted form
        [HttpPost]
        public IActionResult Register(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            var existingEmployee = _context.Employees
                .FirstOrDefault(e => e.Email.ToLower() == employee.Email.ToLower().Trim());

            if (existingEmployee != null)
            {
                ViewBag.Error = "An account with this email already exists.";
                return View(employee);
            }

            _context.Employees.Add(employee);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }
        //Display Login form
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        //Recieve submitted form
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var employee = _context.Employees.FirstOrDefault(e =>
                e.Email == email &&
                e.PasswordHash == password);

            if (employee != null)
            {
                HttpContext.Session.SetString("EmployeeName", employee.FirstName);

                HttpContext.Session.SetString("Email", employee.Email);

                return RedirectToAction("Dashboard");
            }

            return Content("Invalid Email or Password");
        }
        [HttpGet]
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Email") == null)
            {
                return RedirectToAction("Login");
            }

            string email = HttpContext.Session.GetString("Email")!;

            var employee = _context.Employees
                .FirstOrDefault(e => e.Email == email);

            if (employee == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.EmployeeName = employee.FirstName;
            ViewBag.AvailableLeaves = employee.AvailableLeaves;

            ViewBag.Pending = _context.LeaveRequests.Count(l =>
                l.EmployeeEmail == email && l.Status == "Pending");

            ViewBag.Approved = _context.LeaveRequests.Count(l =>
                l.EmployeeEmail == email && l.Status == "Approved");

            ViewBag.Rejected = _context.LeaveRequests.Count(l =>
                l.EmployeeEmail == email && l.Status == "Rejected");

            return View();
        }

        [HttpGet]
        public IActionResult ApplyLeave()
        {
            if (HttpContext.Session.GetString("Email") == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public IActionResult ApplyLeave(LeaveRequest leaveRequest)
        {
            // Get logged-in employee's email
            leaveRequest.EmployeeEmail = HttpContext.Session.GetString("Email")!;

            if (!ModelState.IsValid)
            {
                return View(leaveRequest);
            }

            // Find the logged-in employee
            var employee = _context.Employees
                .FirstOrDefault(e => e.Email == leaveRequest.EmployeeEmail);

            if (employee == null)
            {
                return RedirectToAction("Login");
            }

            // Check if the employee has any leaves left
            if (employee.AvailableLeaves <= 0)
            {
                ViewBag.Error = "You have no available leaves remaining.";
                return View(leaveRequest);
            }

            // Reserve one leave immediately
            employee.AvailableLeaves--;

            // Save the leave request
            _context.LeaveRequests.Add(leaveRequest);

            // Save both changes
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }
        [HttpGet]
        public IActionResult MyLeaves()
        {
            if (HttpContext.Session.GetString("Email") == null)
            {
                return RedirectToAction("Login");
            }

            string email = HttpContext.Session.GetString("Email")!;

            var leaves = _context.LeaveRequests
                .Where(l => l.EmployeeEmail == email)
                .ToList();

            return View(leaves);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }

    }
}