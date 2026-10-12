namespace InventoryDashboard.Api.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int CurrentStock { get; set; }
    public int MinimumStockLevel { get; set; }
    public string StockStatus => CurrentStock == 0 ? "Out of Stock" : (CurrentStock <= MinimumStockLevel ? "Low Stock" : "In Stock");
    public DateTime CreatedAt { get; set; }
}

public class CreateProductDto
{
    public string ProductCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public decimal UnitPrice { get; set; }
    public int InitialStockQuantity { get; set; }
    public int MinimumStockLevel { get; set; }
}

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public decimal UnitPrice { get; set; }
    public int MinimumStockLevel { get; set; }
}
