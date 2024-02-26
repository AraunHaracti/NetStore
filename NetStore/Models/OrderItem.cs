namespace NetStore.Models;

public class OrderItem
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public decimal PricePerUnit { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
}
