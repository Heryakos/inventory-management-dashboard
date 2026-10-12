using InventoryDashboard.Api.Data;
using InventoryDashboard.Api.DTOs;
using InventoryDashboard.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryDashboard.Api.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var totalProducts = await _context.Products.CountAsync();
        var totalUnitsInStock = await _context.Products.SumAsync(p => p.CurrentStock);
        
        var lowStockProducts = await _context.Products
            .CountAsync(p => p.CurrentStock > 0 && p.CurrentStock <= p.MinimumStockLevel);
            
        var outOfStockProducts = await _context.Products
            .CountAsync(p => p.CurrentStock == 0);

        return new DashboardSummaryDto
        {
            TotalProducts = totalProducts,
            TotalUnitsInStock = totalUnitsInStock,
            LowStockProducts = lowStockProducts,
            OutOfStockProducts = outOfStockProducts
        };
    }

    public async Task<IEnumerable<CategoryDistributionDto>> GetCategoryDistributionAsync()
    {
        return await _context.Categories
            .Select(c => new CategoryDistributionDto
            {
                CategoryName = c.Name,
                ProductCount = c.Products.Count
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<StockMovementDto>> GetRecentStockMovementsAsync(int days = 7)
    {
        var cutoff = DateTime.UtcNow.Date.AddDays(-days);
        
        var transactions = await _context.StockTransactions
            .Where(st => st.CreatedAt >= cutoff && (st.TransactionType == TransactionType.StockIn || st.TransactionType == TransactionType.StockOut))
            .GroupBy(st => st.CreatedAt.Date)
            .Select(g => new StockMovementDto
            {
                Date = g.Key,
                StockIn = g.Where(x => x.TransactionType == TransactionType.StockIn).Sum(x => x.Quantity),
                StockOut = g.Where(x => x.TransactionType == TransactionType.StockOut).Sum(x => x.Quantity)
            })
            .ToListAsync();

        // Fill in missing dates
        var result = new List<StockMovementDto>();
        for (int i = days - 1; i >= 0; i--)
        {
            var date = DateTime.UtcNow.Date.AddDays(-i);
            var movement = transactions.FirstOrDefault(t => t.Date == date);
            if (movement != null)
            {
                result.Add(movement);
            }
            else
            {
                result.Add(new StockMovementDto { Date = date, StockIn = 0, StockOut = 0 });
            }
        }
        
        return result;
    }
}
