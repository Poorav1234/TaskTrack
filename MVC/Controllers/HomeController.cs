using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using Repositories;
using Repositories.Models;

namespace MVC.Controllers;

public class HomeController : Controller
{
    private readonly IUserInterface userInterface;
    private readonly ILogger<HomeController> logger;

    public HomeController(ILogger<HomeController> logger, IUserInterface userInterface)
    {
        this.logger = logger;
        this.userInterface = userInterface;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(vm_Login login)
    {
        if (ModelState.IsValid)
        {
            t_user user = await userInterface.Login(login);

            if (user != null)
            {
                HttpContext.Session.SetString("UserId", user.user_id.ToString());
                HttpContext.Session.SetString("Username", user.username);
                HttpContext.Session.SetString("Email", user.email);
                HttpContext.Session.SetString("Gender", user.gender);
                HttpContext.Session.SetString("City", user.city);
                HttpContext.Session.SetString("Mobile", user.mobile);
                HttpContext.Session.SetString("Password", user.password);

                return RedirectToAction("Index", "Task");
            }
            else
            {
                ViewData["Message"] = "Invalid Email ID and Password";
            }
        }

        return View(login);
    }
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(t_user user)
    {
        if (ModelState.IsValid)
        {
            int status = await userInterface.Register(user);
            if (status == 1)
            {
                return RedirectToAction("Login", "Home");
            }
            else if (status == 0)
            {
                ViewData["message"] = "User already exists";
            }
            else
            {
                ViewData["message"] = "Error in registration";
            }
        }
        else
        {
            ModelState.AddModelError("", "Invalid data");
        }
        return View(user);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
