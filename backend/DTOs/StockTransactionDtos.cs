namespace InventoryDashboard.Api.DTOs;

public class StockTransactionDto
{
    public int Id { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }
    public string? Notes { get; set; }
    public DateTime Date { get; set; }
}

public class StockOperationDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } // for Adjustment, this is the counted quantity
    public string? Notes { get; set; }
}
