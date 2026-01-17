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

        // Categories
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<CategoryDto?> CreateCategoryAsync(CreateCategoryRequest request);
        Task<bool> DeleteCategoryAsync(int id);

        // Stores
        Task<List<StoreDto>> GetStoresAsync();

        // Statistics
        Task<DashboardViewModel> GetDashboardDataAsync();
        Task<ReportsViewModel> GetReportsDataAsync();
        Task<List<DailySpendingItem>> GetDailySpendingAsync(int year, int month);

        // OCR
        Task<Dictionary<string, object>?> ScanReceiptAsync(IFormFile image);
    }
}

