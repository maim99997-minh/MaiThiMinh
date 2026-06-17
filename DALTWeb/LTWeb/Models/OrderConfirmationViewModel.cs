namespace LTWeb.Models
{
    public class OrderConfirmationViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CartItem> OrderItems { get; set; }
    }
}
