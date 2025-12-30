namespace smart_receipt_web.Models
{
    // ===== Error ViewModel =====
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    // ===== API Response Wrapper =====
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
    }

    // ===== Auth Models =====
    public class LoginViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? ReturnUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class RegisterViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }

    public class AuthResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

    // ===== Receipt Models =====
    public class ReceiptDto
    {
        public int Id { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public string? ImagePath { get; set; }
        public string? Tags { get; set; }
        public List<ReceiptItemDto> Items { get; set; } = new List<ReceiptItemDto>();
    }

    public class ReceiptItemDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class CreateReceiptRequest
    {
        public string StoreName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public string? ImagePath { get; set; }
        public string? Tags { get; set; }
        public List<ReceiptItemDto> Items { get; set; } = new List<ReceiptItemDto>();
    }

    public class UpdateReceiptRequest
    {
        public int Id { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public string? ImagePath { get; set; }
        public string? Tags { get; set; }
        public List<ReceiptItemDto> Items { get; set; } = new List<ReceiptItemDto>();
    }

    // ===== Dashboard & Statistics Models =====
    public class DashboardViewModel
    {
        public decimal TotalMonthlySpend { get; set; }
        public decimal AverageReceiptValue { get; set; }
        public string MostFrequentStore { get; set; } = string.Empty;
        public int MostFrequentStoreVisits { get; set; }
        public List<MonthlySpendingItem> MonthlyData { get; set; } = new List<MonthlySpendingItem>();
        public List<StoreSpendingItem> StoreData { get; set; } = new List<StoreSpendingItem>();
        public List<DailySpendingItem> DailySpending { get; set; } = new List<DailySpendingItem>();
        public List<ReceiptDto> RecentReceipts { get; set; } = new List<ReceiptDto>();
    }

    public class ReportsViewModel
    {
        public decimal TotalSpending { get; set; }
        public decimal AverageReceiptValue { get; set; }
        public decimal AverageMonthlySpend { get; set; }
        public int TotalReceipts { get; set; }
        public List<MonthlySpendingItem> MonthlyData { get; set; } = new List<MonthlySpendingItem>();
        public List<StoreSpendingItem> StoreData { get; set; } = new List<StoreSpendingItem>();
        public List<DailySpendingItem> DailySpending { get; set; } = new List<DailySpendingItem>();
    }

    public class MonthlySpendingItem
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalSpending { get; set; }
    }

    public class StoreSpendingItem
    {
        public string StoreName { get; set; } = string.Empty;
        public decimal TotalSpending { get; set; }
        public int ReceiptCount { get; set; }
    }

    public class DailySpendingItem
    {
        public string Date { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    // ===== Receipt Edit ViewModel =====
    public class ReceiptEditViewModel
    {
        public int Id { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public string? ImagePath { get; set; }
        public string? Tags { get; set; }
        public List<ReceiptItemViewModel> Items { get; set; } = new List<ReceiptItemViewModel>();
    }

    public class ReceiptItemViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}

