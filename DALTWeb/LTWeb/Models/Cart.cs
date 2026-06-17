namespace LTWeb.Models
{
    public class Cart
    {
        // Khởi tạo List mới để nó không bao giờ bị null
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        public decimal GrandTotal => CartItems.Sum(x => x.Price * x.Quantity);
    }
}
