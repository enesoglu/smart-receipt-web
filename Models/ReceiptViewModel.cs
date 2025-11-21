namespace smart_receipt_web.Models
{
	public class ReceiptViewModel
	{
		public int Id { get; set; }
		public string StoreName { get; set; } = "";
		public DateTime Date { get; set; }
		public decimal TotalAmount { get; set; }
		public string? ImagePath { get; set; }
	}
}