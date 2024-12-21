using System;


public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string CategoryName { get; set; }
    public int? CategoryId { get; set; }
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public string Sku { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ImageUrl { get; set; }
}


public class Category{
    public int id {get;set;}
    public string category_name{set;get;}

     
    public string category_code { get; set; } 
}

public class Sale
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public ICollection<SaleItem> SaleItems { get; set; }
}


public class SaleItem
{
    public int Id { get; set;}
    public int SaleId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

     public Product Product { get; set; } 
    
    // public virtual Sale Sale { get; set; }
    // public virtual Product Product { get; set; }
}
