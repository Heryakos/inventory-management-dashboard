namespace InventoryDashboard.Api.Entities;

public enum TransactionType
{
    StockIn = 1,
    StockOut = 2,
    Adjustment = 3
}

public class StockTransaction
{
    public int Id { get; set; }
    public string TransactionReference { get; set; } = null!;
    public int ProductId { get; set; }
    public TransactionType TransactionType { get; set; }
    
    // For StockIn/Out this is the amount. For Adjustment, this can represent the difference.
    public int Quantity { get; set; }
    
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Product Product { get; set; } = null!;
}
