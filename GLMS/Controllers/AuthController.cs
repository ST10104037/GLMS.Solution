using GLMS.Services;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;

        public AuthController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("JwtToken") != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var token = await _apiService.LoginAsync(username, password);
            if (token == null)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }
            HttpContext.Session.SetString("JwtToken", token);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
