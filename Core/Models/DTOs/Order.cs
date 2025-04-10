namespace Core.Models.DTOs;

public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public List<int> ProductIds { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}