using smart_receipt_web.Models;

namespace smart_receipt_web.Services
{
    public interface IReceiptApiService
    {
        // CRUD Operations
        Task<List<ReceiptDto>> GetReceiptsAsync();
        Task<ReceiptDto?> GetReceiptByIdAsync(int id);
        Task<ReceiptDto?> CreateReceiptAsync(CreateReceiptRequest request);
        Task<ReceiptDto?> UpdateReceiptAsync(int id, UpdateReceiptRequest request);
        Task<bool> DeleteReceiptAsync(int id);

        // Statistics
        Task<DashboardViewModel> GetDashboardDataAsync();
        Task<ReportsViewModel> GetReportsDataAsync();
        Task<List<DailySpendingItem>> GetDailySpendingAsync(int year, int month);

        // OCR
        Task<Dictionary<string, string>?> ScanReceiptAsync(IFormFile image);
    }
}

