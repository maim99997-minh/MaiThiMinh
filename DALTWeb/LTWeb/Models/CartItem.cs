namespace LTWeb.Models
{
    public class CartItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } // Đảm bảo có dòng này
        public decimal Total => Price * Quantity;
    }
}
