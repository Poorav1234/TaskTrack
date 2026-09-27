using Microsoft.AspNetCore.Mvc;
using Repositories;
using Repositories.Models;
using Microsoft.AspNetCore.Http;

namespace MyApp.Namespace
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class TaskController : Controller
    {
        private readonly ITaskInterface _taskService;
        private readonly IUserInterface _userService;

        public TaskController(ITaskInterface taskService, IUserInterface userService)
        {
            _taskService = taskService;
            _userService = userService;
        }


        // Task Dashboard
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                int user_id = int.Parse(
                    HttpContext.Session.GetString("UserId")
                );

                List<t_task> tasks = await _taskService.GetTasksByUser(user_id);

                return View("List", tasks);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        // Logout
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Home");
        }


        // Task List
        public async Task<IActionResult> List()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                List<t_task> tasks = await _taskService.GetTasksByUser(int.Parse(HttpContext.Session.GetString("UserId")));

                return View(tasks);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        // GET: Create / Edit Task
        [HttpGet]
        public async Task<IActionResult> Create(string id = "")
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            t_task task = new t_task();

            if (id != "")
            {
                int taskId = int.Parse(id);
                task = await _taskService.GetTask(taskId);

                if (task == null)
                {
                    TempData["Message"] = "Task not found";
                    return RedirectToAction("List", "Task");
                }
                else
                {
                    return View(task);
                }
            }

            return View(task);
        }


        // POST: Create / Update Task
        [HttpPost]
        public async Task<IActionResult> Create(t_task task)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                task.user_id = int.Parse(
                    HttpContext.Session.GetString("UserId")
                );

                int result = 0;

                if (task.task_id == 0)
                {
                    result = await _taskService.CreateTask(task);
                }
                else
                {
                    result = await _taskService.UpdateTask(task);
                }

                if (result == 1)
                {
                    TempData["Message"] = task.task_id == 0
                        ? "Task created successfully"
                        : "Task updated successfully";
                }
                else
                {
                    TempData["Message"] = "Error in saving task";
                }

                return RedirectToAction("List", "Task");
            }

            return View(task);
        }


        // Delete Task
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int result = await _taskService.DeleteTask(id);

            if (result == 1)
            {
                TempData["Message"] = "Task deleted successfully";
            }
            else
            {
                TempData["Message"] = "Error in deleting task";
            }

            return RedirectToAction("List", "Task");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            t_task task = await _taskService.GetTask(id);

            if (task == null)
            {
                TempData["Message"] = "Task not found";
                return RedirectToAction("List", "Task");
            }

            return View(task);
        }

        [HttpGet]
        public async Task<IActionResult> Search(
    string search = "",
    string category = "",
    string priority = "",
    string status = "",
    DateTime? due_date = null)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int user_id = int.Parse(
                HttpContext.Session.GetString("UserId")
            );

            List<t_task> tasks = await _taskService.SearchTasks(
                user_id,
                search,
                category,
                priority,
                status,
                due_date
            );

            ViewBag.Search = search;
            ViewBag.Category = category;
            ViewBag.Priority = priority;
            ViewBag.Status = status;
            ViewBag.DueDate = due_date?.ToString("yyyy-MM-dd");

            return View("List", tasks);
        }

        [HttpGet]
        public IActionResult ClearSearch()
        {
            return RedirectToAction("List", "Task");
        }
    }
}