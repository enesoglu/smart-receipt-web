using Microsoft.AspNetCore.Mvc;

namespace smart_receipt_web.Controllers
{
    public class ReceiptController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}