using Microsoft.AspNetCore.Mvc;
using Repositories;
using Repositories.Models;

namespace MyApp.Namespace
{
    public class AdminController : Controller
    {
        private readonly IUserInterface _userService;
        private readonly ITaskInterface _taskService;

        public AdminController(IUserInterface userService, ITaskInterface taskService)
        {
            _userService = userService;
            _taskService = taskService;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int totalUsers = await _userService.GetTotalUsers();

            vm_AdminDashboard dashboard =
                await _taskService.GetDashboardSummary();

            dashboard.TotalUsers = totalUsers;

            return View(dashboard);
        }

        public async Task<IActionResult> Users()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            List<t_user> users = await _userService.GetUsers();

            return View(users);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Home");
        }

        public async Task<IActionResult> Tasks()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            List<t_task> tasks = await _taskService.GetAllTasks();

            return View(tasks);
        }

        [HttpGet]
        public async Task<IActionResult> SearchUsers(
    string username = "",
    string email = "",
    string mobile = "",
    string gender = "",
    string city = "")
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            List<t_user> users = await _userService.SearchUsers(
                username,
                email,
                mobile,
                gender,
                city
            );

            ViewBag.Username = username;
            ViewBag.Email = email;
            ViewBag.Mobile = mobile;
            ViewBag.Gender = gender;
            ViewBag.City = city;

            return View("Users", users);
        }

        [HttpGet]
        public async Task<IActionResult> SearchTasks(
    string task_name = "",
    int? user_id = null,
    string category = "",
    string priority = "",
    string status = "",
    DateTime? due_date = null)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            List<t_task> tasks = await _taskService.SearchAllTasks(
                task_name,
                user_id,
                category,
                priority,
                status,
                due_date
            );

            ViewBag.TaskName = task_name;
            ViewBag.UserId = user_id;
            ViewBag.Category = category;
            ViewBag.Priority = priority;
            ViewBag.Status = status;
            ViewBag.DueDate = due_date?.ToString("yyyy-MM-dd");

            return View("Tasks", tasks);
        }

        public async Task<IActionResult> UserTaskSummary()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            List<vm_UserTaskSummary> summary =
                await _taskService.GetUserTaskSummary();

            return View(summary);
        }

        public async Task<IActionResult> TaskStatusReport()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            List<vm_TaskStatusReport> report =
                await _taskService.GetTaskStatusReport();

            return View(report);
        }

        public async Task<IActionResult> OverdueTaskReport()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            List<vm_OverdueTaskReport> report =
                await _taskService.GetOverdueTaskReport();

            return View(report);
        }

        [HttpGet]
        public async Task<IActionResult> TasksDueOnDate(DateTime? due_date)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (!due_date.HasValue)
            {
                return View(new List<vm_TasksDueOnDate>());
            }

            List<vm_TasksDueOnDate> report =
                await _taskService.GetTasksDueOnDate(due_date.Value);

            ViewBag.DueDate = due_date.Value.ToString("yyyy-MM-dd");

            return View(report);
        }
        [HttpGet]
        public async Task<IActionResult> UserTaskDetail(int? user_id)
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            List<t_user> users = await _userService.GetUsers();

            ViewBag.Users = users;

            if (!user_id.HasValue)
            {
                return View(new List<vm_UserTaskDetail>());
            }

            List<vm_UserTaskDetail> report =
                await _taskService.GetUserTaskDetail(user_id.Value);

            ViewBag.UserId = user_id.Value;

            t_user selectedUser = await _userService.GetUser(user_id.Value);

            if (selectedUser != null)
            {
                ViewBag.Username = selectedUser.username;
            }

            return View(report);
        }
    }
}