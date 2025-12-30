using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using smart_receipt_web.Models;
using smart_receipt_web.Services;

namespace smart_receipt_web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IReceiptApiService _receiptApiService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IReceiptApiService receiptApiService, ILogger<HomeController> logger)
        {
            _receiptApiService = receiptApiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboard = await _receiptApiService.GetDashboardDataAsync();
                return View(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Dashboard error: {ex.Message}");
                return View(new DashboardViewModel());
            }
        }

        public async Task<IActionResult> Reports()
        {
            try
            {
                var reports = await _receiptApiService.GetReportsDataAsync();
                return View(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Reports error: {ex.Message}");
                return View(new ReportsViewModel());
            }
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

