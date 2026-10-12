namespace InventoryDashboard.Api.DTOs;

public class DashboardSummaryDto
{
    public int TotalProducts { get; set; }
    public int TotalUnitsInStock { get; set; }
    public int LowStockProducts { get; set; }
    public int OutOfStockProducts { get; set; }
}

public class CategoryDistributionDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
}

public class StockMovementDto
{
    public DateTime Date { get; set; }
    public int StockIn { get; set; }
    public int StockOut { get; set; }
}
