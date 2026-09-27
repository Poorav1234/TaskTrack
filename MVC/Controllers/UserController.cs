using Microsoft.AspNetCore.Mvc;
using Repositories;
using Repositories.Models;

namespace MyApp.Namespace
{
    public class UserController : Controller
    {
        private readonly IUserInterface _userService;

        public UserController(IUserInterface userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int user_id = int.Parse(
                HttpContext.Session.GetString("UserId")
            );

            t_user user = await _userService.GetUser(user_id);

            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            vm_EditProfile profile = new vm_EditProfile
            {
                username = user.username,
                email = user.email,
                mobile = user.mobile,
                gender = user.gender,
                city = user.city
            };

            return View(profile);
        }


        [HttpPost]
        public async Task<IActionResult> EditProfile(vm_EditProfile profile)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                int user_id = int.Parse(
                    HttpContext.Session.GetString("UserId")
                );

                t_user user = new t_user
                {
                    user_id = user_id,
                    username = profile.username,
                    email = profile.email,
                    mobile = profile.mobile,
                    gender = profile.gender,
                    city = profile.city
                };

                int result = await _userService.UpdateProfile(user);

                if (result == 1)
                {
                    HttpContext.Session.SetString(
                        "Username",
                        profile.username
                    );

                    HttpContext.Session.SetString(
                        "Email",
                        profile.email
                    );

                    TempData["Message"] = "Profile updated successfully";

                    return RedirectToAction("Index", "Task");
                }
                else
                {
                    TempData["Message"] = "Error in updating profile";
                }
            }

            return View(profile);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(
    vm_ChangePassword password)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                int user_id = int.Parse(
                    HttpContext.Session.GetString("UserId")
                );

                int result = await _userService.ChangePassword(
                    user_id,
                    password
                );

                if (result == 1)
                {
                    TempData["Message"] =
                        "Password changed successfully";

                    return RedirectToAction("Index", "Task");
                }
                else if (result == 0)
                {
                    ModelState.AddModelError(
                        "old_password",
                        "Old password is incorrect"
                    );
                }
                else
                {
                    TempData["Message"] =
                        "Error in changing password";
                }
            }

            return View(password);
        }
    }
}