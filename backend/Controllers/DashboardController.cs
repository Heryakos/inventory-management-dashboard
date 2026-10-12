using InventoryDashboard.Api.DTOs;
using InventoryDashboard.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
    {
        return Ok(await _service.GetSummaryAsync());
    }

    [HttpGet("category-distribution")]
    public async Task<ActionResult<IEnumerable<CategoryDistributionDto>>> GetCategoryDistribution()
    {
        return Ok(await _service.GetCategoryDistributionAsync());
    }

    [HttpGet("recent-stock-movements")]
    public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetRecentStockMovements()
    {
        return Ok(await _service.GetRecentStockMovementsAsync());
    }
    
    // We could add endpoints for low stock or recent transactions here, 
    // or just rely on ProductsController / StockTransactionsController with filters.
    // For simplicity, we can let frontend query Products?stockStatus=LowStock
}
