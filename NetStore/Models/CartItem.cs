namespace NetStore.Models;

public class CartItem
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
}