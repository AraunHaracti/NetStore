using System;

namespace NetStore.Models;

public class Order
{
    public int OrderId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int UserId { get; set; }
    public int ShippingAddressId { get; set; }
    public OrderStatusEnum StatusId { get; set; }
}
