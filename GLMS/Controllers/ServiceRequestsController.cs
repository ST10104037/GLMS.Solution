using GLMS.Services;
using GLMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ApiService _apiService;
        private readonly CurrencyService _currencyService;

        public ServiceRequestsController(
            ApiService apiService, CurrencyService currencyService)
        {
            _apiService = apiService;
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _apiService.GetServiceRequestsAsync();
            return View(requests);
        }

        public async Task<IActionResult> Details(int id)
        {
            var sr = await _apiService.GetServiceRequestAsync(id);
            if (sr == null) return NotFound();
            return View(sr);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Contracts = await _apiService.GetContractsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest serviceRequest, decimal costUSD)
        {
            var rate = await _currencyService.GetUsdToZarRateAsync();
            serviceRequest.CostUSD = costUSD;
            serviceRequest.CostZAR = CurrencyService.ConvertUsdToZar(costUSD, rate);

            var (success, error) = await _apiService.CreateServiceRequestAsync(serviceRequest);
            if (success) return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", error ?? "Failed to create service request.");
            ViewBag.Contracts = await _apiService.GetContractsAsync();
            return View(serviceRequest);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var sr = await _apiService.GetServiceRequestAsync(id);
            if (sr == null) return NotFound();
            return View(sr);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteServiceRequestAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}