using GLMS.Services;
using GLMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContractsController(ApiService apiService, IWebHostEnvironment env)
        {
            _apiService = apiService;
            _webHostEnvironment = env;
        }

        public async Task<IActionResult> Index(
            DateTime? startDate, DateTime? endDate, ContractStatus? status)
        {
            var contracts = await _apiService.GetContractsAsync(startDate, endDate, status);
            return View(contracts);
        }

        public async Task<IActionResult> Details(int id)
        {
            var contract = await _apiService.GetContractAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Clients = await _apiService.GetClientsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile? signedAgreement)
        {
            if (signedAgreement != null)
            {
                try
                {
                    var uploadsFolder = Path.Combine(
                        _webHostEnvironment.WebRootPath, "uploads");
                    var fileName = await FileValidationService.SaveFileAsync(
                        signedAgreement, uploadsFolder);
                    contract.SignedAgreementPath = fileName;
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    ViewBag.Clients = await _apiService.GetClientsAsync();
                    return View(contract);
                }
            }

            if (ModelState.IsValid)
            {
                await _apiService.CreateContractAsync(contract);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Clients = await _apiService.GetClientsAsync();
            return View(contract);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _apiService.GetContractAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteContractAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}