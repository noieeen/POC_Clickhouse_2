namespace Core.Models;

public class Product
{
}

public class ProductReq
{
    public string name { get; set; }
    public decimal price { get; set; }
    public int quantity { get; set; }
}

public class UpdateProductReq
{
    public int id { get; set; }
    public string name { get; set; }
    public decimal price { get; set; }
    public int quantity { get; set; }
}