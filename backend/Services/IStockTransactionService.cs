using InventoryDashboard.Api.DTOs;

namespace InventoryDashboard.Api.Services;

public interface IStockTransactionService
{
    Task<PaginatedResponse<StockTransactionDto>> GetTransactionsAsync(PaginationQuery query);
    Task<PaginatedResponse<StockTransactionDto>> GetTransactionsByProductAsync(int productId, PaginationQuery query);
    Task<StockTransactionDto?> GetTransactionByIdAsync(int id);
    Task<StockTransactionDto> StockInAsync(StockOperationDto dto);
    Task<StockTransactionDto> StockOutAsync(StockOperationDto dto);
    Task<StockTransactionDto> AdjustStockAsync(StockOperationDto dto);
}
