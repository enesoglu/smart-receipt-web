using smart_receipt_web.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace smart_receipt_web.Services
{
    public class ReceiptApiService : IReceiptApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ReceiptApiService> _logger;

        private const string TokenSessionKey = "JwtToken";

        public ReceiptApiService(
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ReceiptApiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7018";
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        private void SetAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString(TokenSessionKey);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // ===== CRUD Operations =====

        public async Task<List<ReceiptDto>> GetReceiptsAsync()
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync("/api/receipts");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<ApiResponse<List<ReceiptDto>>>(content, options);
                    return result?.Data ?? new List<ReceiptDto>();
                }

                _logger.LogWarning($"GetReceipts failed: {response.StatusCode}");
                return new List<ReceiptDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetReceipts error: {ex.Message}");
                return new List<ReceiptDto>();
            }
        }

        public async Task<ReceiptDto?> GetReceiptByIdAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"/api/receipts/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<ApiResponse<ReceiptDto>>(content, options);
                    return result?.Data;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetReceiptById error: {ex.Message}");
                return null;
            }
        }

        public async Task<ReceiptDto?> CreateReceiptAsync(CreateReceiptRequest request)
        {
            try
            {
                SetAuthorizationHeader();
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/receipts", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<ApiResponse<ReceiptDto>>(responseContent, options);
                    return result?.Data;
                }

                _logger.LogWarning($"CreateReceipt failed: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateReceipt error: {ex.Message}");
                return null;
            }
        }

        public async Task<ReceiptDto?> UpdateReceiptAsync(int id, UpdateReceiptRequest request)
        {
            try
            {
                SetAuthorizationHeader();
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/api/receipts/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<ApiResponse<ReceiptDto>>(responseContent, options);
                    return result?.Data;
                }

                _logger.LogWarning($"UpdateReceipt failed: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateReceipt error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteReceiptAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"/api/receipts/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError($"DeleteReceipt error: {ex.Message}");
                return false;
            }
        }

        // ===== Categories =====

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync("/api/categories");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<ApiResponse<List<CategoryDto>>>(content, options);
                    return result?.Data ?? new List<CategoryDto>();
                }

                _logger.LogWarning($"GetCategories failed: {response.StatusCode}");
                return new List<CategoryDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetCategories error: {ex.Message}");
                return new List<CategoryDto>();
            }
        }

        // ===== Statistics =====

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            try
            {
                SetAuthorizationHeader();
                var viewModel = new DashboardViewModel();

                // Get stats
                var statsResponse = await _httpClient.GetAsync("/api/receipts/stats");
                if (statsResponse.IsSuccessStatusCode)
                {
                    var content = await statsResponse.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<ApiResponse<DashboardStatsDto>>(content, options);

                    if (result?.Data != null)
                    {
                        viewModel.TotalMonthlySpend = result.Data.TotalMonthlySpending;
                        viewModel.AverageReceiptValue = result.Data.AverageReceiptValue;
                        viewModel.MostFrequentStore = result.Data.MostFrequentStore;
                        viewModel.MostFrequentStoreVisits = result.Data.MostFrequentStoreVisitCount;
                    }
                }

                // Get store data
                var storeResponse = await _httpClient.GetAsync("/api/receipts/store-stats");
                if (storeResponse.IsSuccessStatusCode)
                {
                    var content = await storeResponse.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<ApiResponse<List<StoreSpendingItem>>>(content, options);
                    viewModel.StoreData = result?.Data ?? new List<StoreSpendingItem>();
                }

                // Get recent receipts
                var receipts = await GetReceiptsAsync();
                viewModel.RecentReceipts = receipts.OrderByDescending(r => r.Date).Take(5).ToList();

                // Get daily spending for current month
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                viewModel.DailySpending = await GetDailySpendingAsync(year, month);

                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetDashboardData error: {ex.Message}");
                return new DashboardViewModel();
            }
        }

        public async Task<ReportsViewModel> GetReportsDataAsync()
        {
            try
            {
                SetAuthorizationHeader();
                var viewModel = new ReportsViewModel();

                // Get all receipts for calculations
                var receipts = await GetReceiptsAsync();
                viewModel.TotalReceipts = receipts.Count;
                viewModel.TotalSpending = receipts.Sum(r => r.TotalAmount);
                viewModel.AverageReceiptValue = receipts.Count > 0 ? receipts.Average(r => r.TotalAmount) : 0;

                // Calculate monthly data from receipts
                var monthlyGroups = receipts
                    .GroupBy(r => new { r.Date.Year, r.Date.Month })
                    .Select(g => new MonthlySpendingItem
                    {
                        Month = g.Key.Month,
                        Year = g.Key.Year,
                        TotalSpending = g.Sum(r => r.TotalAmount)
                    })
                    .OrderBy(m => m.Year)
                    .ThenBy(m => m.Month)
                    .ToList();

                viewModel.MonthlyData = monthlyGroups;

                // Calculate average monthly spend (last 6 months or all if less)
                var recentMonths = monthlyGroups.TakeLast(6).ToList();
                viewModel.AverageMonthlySpend = recentMonths.Count > 0
                    ? recentMonths.Average(m => m.TotalSpending)
                    : 0;

                // Get store data
                var storeResponse = await _httpClient.GetAsync("/api/receipts/store-stats");
                if (storeResponse.IsSuccessStatusCode)
                {
                    var content = await storeResponse.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<ApiResponse<List<StoreSpendingItem>>>(content, options);
                    viewModel.StoreData = result?.Data ?? new List<StoreSpendingItem>();
                }

                // Get daily spending for current month
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                viewModel.DailySpending = await GetDailySpendingAsync(year, month);

                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetReportsData error: {ex.Message}");
                return new ReportsViewModel();
            }
        }

        public async Task<List<DailySpendingItem>> GetDailySpendingAsync(int year, int month)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"/api/receipts/daily-spending?year={year}&month={month}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<ApiResponse<List<DailySpendingItem>>>(content, options);
                    return result?.Data ?? new List<DailySpendingItem>();
                }

                return new List<DailySpendingItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetDailySpending error: {ex.Message}");
                return new List<DailySpendingItem>();
            }
        }

        // ===== Stores =====

        public async Task<List<StoreDto>> GetStoresAsync()
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync("/api/stores");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<ApiResponse<List<StoreDto>>>(content, options);
                    return result?.Data ?? new List<StoreDto>();
                }

                _logger.LogWarning($"GetStores failed: {response.StatusCode}");
                return new List<StoreDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetStores error: {ex.Message}");
                return new List<StoreDto>();
            }
        }

        // ===== Category CRUD =====

        public async Task<CategoryDto?> CreateCategoryAsync(CreateCategoryRequest request)
        {
            try
            {
                SetAuthorizationHeader();
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/categories", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<ApiResponse<CategoryDto>>(responseContent, options);
                    return result?.Data;
                }

                _logger.LogWarning($"CreateCategory failed: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateCategory error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"/api/categories/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError($"DeleteCategory error: {ex.Message}");
                return false;
            }
        }

        // ===== OCR =====

        public async Task<Dictionary<string, object>?> ScanReceiptAsync(IFormFile image)
        {
            try
            {
                SetAuthorizationHeader();

                using var content = new MultipartFormDataContent();
                using var stream = image.OpenReadStream();
                using var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
                content.Add(streamContent, "image", image.FileName);

                var response = await _httpClient.PostAsync("/api/receipts/scan", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<ApiResponse<Dictionary<string, object>>>(responseContent, options);
                    return result?.Data;
                }

                _logger.LogWarning($"ScanReceipt failed: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ScanReceipt error: {ex.Message}");
                return null;
            }
        }
    }

    // Helper DTO for deserializing stats
    internal class DashboardStatsDto
    {
        public decimal TotalMonthlySpending { get; set; }
        public decimal AverageReceiptValue { get; set; }
        public string MostFrequentStore { get; set; } = string.Empty;
        public int MostFrequentStoreVisitCount { get; set; }
    }
}

