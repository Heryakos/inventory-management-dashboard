using InventoryDashboard.Api.DTOs;

namespace InventoryDashboard.Api.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync();
    Task<IEnumerable<CategoryDistributionDto>> GetCategoryDistributionAsync();
    Task<IEnumerable<StockMovementDto>> GetRecentStockMovementsAsync(int days = 7);
}
