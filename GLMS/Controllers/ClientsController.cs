using GLMS.Services;
using GLMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApiService _apiService;

        public ClientsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _apiService.GetClientsAsync();
            return View(clients);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = await _apiService.GetClientAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (ModelState.IsValid)
            {
                await _apiService.CreateClientAsync(client);
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = await _apiService.GetClientAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteClientAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}