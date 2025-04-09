namespace Core.Models;

public class OrderReq
{
    public Guid? OrderId { get; set; } = Guid.NewGuid();
    public int CustomerId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}