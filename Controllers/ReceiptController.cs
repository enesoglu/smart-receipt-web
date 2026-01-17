using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using smart_receipt_web.Models;
using smart_receipt_web.Services;

namespace smart_receipt_web.Controllers
{
    [Authorize]
    public class ReceiptController : Controller
    {
        private readonly IReceiptApiService _receiptApiService;
        private readonly ILogger<ReceiptController> _logger;

        public ReceiptController(IReceiptApiService receiptApiService, ILogger<ReceiptController> logger)
        {
            _receiptApiService = receiptApiService;
            _logger = logger;
        }

        // GET: Receipt/Index - List all receipts
        public async Task<IActionResult> Index()
        {
            try
            {
                var receipts = await _receiptApiService.GetReceiptsAsync();
                return View(receipts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get receipts error: {ex.Message}");
                TempData["Error"] = "Fişler yüklenirken bir hata oluştu.";
                return View(new List<ReceiptDto>());
            }
        }

        // GET: Receipt/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var receipt = await _receiptApiService.GetReceiptByIdAsync(id);
                if (receipt == null)
                {
                    TempData["Error"] = "Fiş bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }
                return View(receipt);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get receipt details error: {ex.Message}");
                TempData["Error"] = "Fiş detayları yüklenirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Receipt/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _receiptApiService.GetCategoriesAsync();
            return View(new ReceiptEditViewModel { Date = DateTime.Today, Categories = categories });
        }

        // POST: Receipt/ScanTest - OCR için AJAX endpoint (Azure OCR)
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ScanTest(IFormFile image)
        {
            try
            {
                _logger.LogInformation("ScanTest called");

                if (image == null || image.Length == 0)
                {
                    _logger.LogWarning("No image provided");
                    return Json(new { success = false, message = "Please select an image." });
                }

                _logger.LogInformation($"Image received: {image.FileName}, Size: {image.Length} bytes, Type: {image.ContentType}");

                var result = await _receiptApiService.ScanReceiptAsync(image);

                if (result != null)
                {
                    _logger.LogInformation($"OCR result received. Keys: {string.Join(", ", result.Keys)}");
                    return Json(new { success = true, data = result });
                }

                _logger.LogWarning("OCR result is null");
                return Json(new { success = false, message = "OCR processing failed. Azure API did not respond." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"ScanTest error: {ex.Message}");
                return Json(new { success = false, message = $"An error occurred during OCR processing: {ex.Message}" });
            }
        }

        // POST: Receipt/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReceiptEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _receiptApiService.GetCategoriesAsync();
                return View(model);
            }

            try
            {
                var request = new CreateReceiptRequest
                {
                    StoreName = model.StoreName,
                    Date = model.Date,
                    TotalAmount = model.TotalAmount,
                    ImagePath = model.ImagePath,
                    CategoryId = model.CategoryId,
                    Items = model.Items.Select(i => new ReceiptItemDto
                    {
                        ProductName = i.ProductName,
                        Price = i.Price
                    }).ToList()
                };

                var result = await _receiptApiService.CreateReceiptAsync(request);
                if (result != null)
                {
                    TempData["Success"] = "Receipt added successfully.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "An error occurred while adding receipt.";
                model.Categories = await _receiptApiService.GetCategoriesAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Create receipt error: {ex.Message}");
                TempData["Error"] = "An error occurred while adding receipt.";
                model.Categories = await _receiptApiService.GetCategoriesAsync();
                return View(model);
            }
        }

        // GET: Receipt/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var receipt = await _receiptApiService.GetReceiptByIdAsync(id);
                if (receipt == null)
                {
                    TempData["Error"] = "Receipt not found.";
                    return RedirectToAction(nameof(Index));
                }

                var categories = await _receiptApiService.GetCategoriesAsync();
                var model = new ReceiptEditViewModel
                {
                    Id = receipt.Id,
                    StoreName = receipt.StoreName,
                    Date = receipt.Date,
                    TotalAmount = receipt.TotalAmount,
                    ImagePath = receipt.ImagePath,
                    CategoryId = receipt.CategoryId,
                    Categories = categories,
                    Items = receipt.Items.Select(i => new ReceiptItemViewModel
                    {
                        Id = i.Id,
                        ProductName = i.ProductName,
                        Price = i.Price
                    }).ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Edit receipt error: {ex.Message}");
                TempData["Error"] = "An error occurred while loading receipt.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Receipt/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReceiptEditViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await _receiptApiService.GetCategoriesAsync();
                return View(model);
            }

            try
            {
                var request = new UpdateReceiptRequest
                {
                    Id = model.Id,
                    StoreName = model.StoreName,
                    Date = model.Date,
                    TotalAmount = model.TotalAmount,
                    ImagePath = model.ImagePath,
                    CategoryId = model.CategoryId,
                    Items = model.Items.Select(i => new ReceiptItemDto
                    {
                        Id = i.Id,
                        ProductName = i.ProductName,
                        Price = i.Price
                    }).ToList()
                };

                var result = await _receiptApiService.UpdateReceiptAsync(id, request);
                if (result != null)
                {
                    TempData["Success"] = "Receipt updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "An error occurred while updating receipt.";
                model.Categories = await _receiptApiService.GetCategoriesAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update receipt error: {ex.Message}");
                TempData["Error"] = "An error occurred while updating receipt.";
                model.Categories = await _receiptApiService.GetCategoriesAsync();
                return View(model);
            }
        }

        // POST: Receipt/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _receiptApiService.DeleteReceiptAsync(id);
                if (result)
                {
                    TempData["Success"] = "Receipt deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "An error occurred while deleting receipt.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete receipt error: {ex.Message}");
                TempData["Error"] = "An error occurred while deleting receipt.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Receipt/Scan - OCR scan
        [HttpPost]
        public async Task<IActionResult> Scan(IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    return Json(new { success = false, message = "Please select an image." });
                }

                var result = await _receiptApiService.ScanReceiptAsync(image);
                if (result != null)
                {
                    return Json(new { success = true, data = result });
                }

                return Json(new { success = false, message = "OCR processing failed." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Scan receipt error: {ex.Message}");
                return Json(new { success = false, message = "An error occurred during OCR processing." });
            }
        }

        // POST: Receipt/CreateAjax - Create receipt via AJAX
        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] CreateReceiptRequest request)
        {
            try
            {
                if (request == null)
                {
                    return Json(new { success = false, message = "Invalid request." });
                }

                if (string.IsNullOrEmpty(request.StoreName))
                {
                    return Json(new { success = false, message = "Store name is required." });
                }

                var result = await _receiptApiService.CreateReceiptAsync(request);
                if (result != null)
                {
                    return Json(new { success = true, message = "Receipt added successfully.", data = result });
                }

                return Json(new { success = false, message = "An error occurred while adding receipt." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateAjax error: {ex.Message}");
                return Json(new { success = false, message = "An error occurred." });
            }
        }
    }
}

