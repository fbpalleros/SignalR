using Microsoft.AspNetCore.Mvc;

namespace TpSignalR.Web.Controllers
{
    public class RegistroController : Controller
    {
        [HttpPost]
        public IActionResult LogInCookie(string username, string userId)
        {
            Console.WriteLine($"Usuario: {username}, ID: {userId}");
            TempData["UserId"] = userId;
            return RedirectToAction("LogInCookie");
        }

        [HttpGet]
        public IActionResult LogInCookie()
        {
            return View();
        }

        public IActionResult LogIn()
        {
            return View();
        }
    }
}
