using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using smart_receipt_web.Models;
using smart_receipt_web.Services;

namespace smart_receipt_web.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly IReceiptApiService _receiptApiService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(IReceiptApiService receiptApiService, ILogger<CategoriesController> logger)
        {
            _receiptApiService = receiptApiService;
            _logger = logger;
        }

        // GET: Categories/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _receiptApiService.GetCategoriesAsync();
                var viewModel = new CategoriesViewModel { Categories = categories };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get categories error: {ex.Message}");
                TempData["Error"] = "An error occurred while loading categories.";
                return View(new CategoriesViewModel());
            }
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    TempData["Error"] = "Category name is required.";
                    return RedirectToAction(nameof(Index));
                }

                var result = await _receiptApiService.CreateCategoryAsync(request);
                if (result != null)
                {
                    TempData["Success"] = "Category added successfully.";
                }
                else
                {
                    TempData["Error"] = "An error occurred while adding category.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Create category error: {ex.Message}");
                TempData["Error"] = "An error occurred while adding category.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _receiptApiService.DeleteCategoryAsync(id);
                if (result)
                {
                    TempData["Success"] = "Category deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "An error occurred while deleting category.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete category error: {ex.Message}");
                TempData["Error"] = "An error occurred while deleting category.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/GetAll (AJAX endpoint)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var categories = await _receiptApiService.GetCategoriesAsync();
                return Json(new { success = true, data = categories });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get categories error: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while loading categories." });
            }
        }

        // GET: Categories/GetStores (AJAX endpoint for stores)
        [HttpGet]
        public async Task<IActionResult> GetStores()
        {
            try
            {
                var stores = await _receiptApiService.GetStoresAsync();
                return Json(new { success = true, data = stores });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get stores error: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while loading stores." });
            }
        }
    }
}
