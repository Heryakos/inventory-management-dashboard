using InventoryDashboard.Api.Data;
using InventoryDashboard.Api.DTOs;
using InventoryDashboard.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryDashboard.Api.Services;

public class StockTransactionService : IStockTransactionService
{
    private readonly ApplicationDbContext _context;

    public StockTransactionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<StockTransactionDto>> GetTransactionsAsync(PaginationQuery query)
    {
        return await GetTransactionsInternalAsync(_context.StockTransactions.Include(st => st.Product).AsQueryable(), query);
    }

    public async Task<PaginatedResponse<StockTransactionDto>> GetTransactionsByProductAsync(int productId, PaginationQuery query)
    {
        var queryable = _context.StockTransactions.Include(st => st.Product).Where(st => st.ProductId == productId);
        return await GetTransactionsInternalAsync(queryable, query);
    }

    private async Task<PaginatedResponse<StockTransactionDto>> GetTransactionsInternalAsync(IQueryable<StockTransaction> queryable, PaginationQuery query)
    {
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            queryable = queryable.Where(st => st.TransactionReference.ToLower().Contains(search) || st.Product.Name.ToLower().Contains(search));
        }

        queryable = query.SortBy?.ToLower() switch
        {
            "date" => query.IsDescending ? queryable.OrderByDescending(st => st.CreatedAt) : queryable.OrderBy(st => st.CreatedAt),
            "product" => query.IsDescending ? queryable.OrderByDescending(st => st.Product.Name) : queryable.OrderBy(st => st.Product.Name),
            _ => query.IsDescending ? queryable.OrderByDescending(st => st.CreatedAt) : queryable.OrderBy(st => st.CreatedAt)
        };

        var totalCount = await queryable.CountAsync();

        var items = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(st => new StockTransactionDto
            {
                Id = st.Id,
                TransactionReference = st.TransactionReference,
                ProductId = st.ProductId,
                ProductName = st.Product.Name,
                TransactionType = st.TransactionType.ToString(),
                Quantity = st.Quantity,
                PreviousStock = st.PreviousStock,
                NewStock = st.NewStock,
                Notes = st.Notes,
                Date = st.CreatedAt
            })
            .ToListAsync();

        return new PaginatedResponse<StockTransactionDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<StockTransactionDto?> GetTransactionByIdAsync(int id)
    {
        var st = await _context.StockTransactions.Include(s => s.Product).FirstOrDefaultAsync(s => s.Id == id);
        if (st == null) return null;

        return new StockTransactionDto
        {
            Id = st.Id,
            TransactionReference = st.TransactionReference,
            ProductId = st.ProductId,
            ProductName = st.Product.Name,
            TransactionType = st.TransactionType.ToString(),
            Quantity = st.Quantity,
            PreviousStock = st.PreviousStock,
            NewStock = st.NewStock,
            Notes = st.Notes,
            Date = st.CreatedAt
        };
    }

    public async Task<StockTransactionDto> StockInAsync(StockOperationDto dto)
    {
        if (dto.Quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");

        // We use a transaction because we update multiple things. 
        // EF Core SaveChanges is transactional by default, but we can explicit locking if needed.
        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null) throw new ArgumentException("Product not found.");

        var previousStock = product.CurrentStock;
        var newStock = previousStock + dto.Quantity;
        
        product.CurrentStock = newStock;
        product.UpdatedAt = DateTime.UtcNow;

        var transaction = new StockTransaction
        {
            TransactionReference = $"IN-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
            ProductId = dto.ProductId,
            TransactionType = TransactionType.StockIn,
            Quantity = dto.Quantity,
            PreviousStock = previousStock,
            NewStock = newStock,
            Notes = dto.Notes
        };

        _context.StockTransactions.Add(transaction);

        try 
        {
            await _context.SaveChangesAsync();
        } 
        catch (DbUpdateConcurrencyException) 
        {
            throw new InvalidOperationException("A concurrency conflict occurred. Please try again.");
        }

        return await GetTransactionByIdAsync(transaction.Id) ?? throw new Exception("Failed to retrieve created transaction.");
    }

    public async Task<StockTransactionDto> StockOutAsync(StockOperationDto dto)
    {
        if (dto.Quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");

        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null) throw new ArgumentException("Product not found.");

        var previousStock = product.CurrentStock;
        var newStock = previousStock - dto.Quantity;
        
        if (newStock < 0) throw new InvalidOperationException("Cannot stock out more than the current stock.");

        product.CurrentStock = newStock;
        product.UpdatedAt = DateTime.UtcNow;

        var transaction = new StockTransaction
        {
            TransactionReference = $"OUT-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
            ProductId = dto.ProductId,
            TransactionType = TransactionType.StockOut,
            Quantity = dto.Quantity,
            PreviousStock = previousStock,
            NewStock = newStock,
            Notes = dto.Notes
        };

        _context.StockTransactions.Add(transaction);
        
        try 
        {
            await _context.SaveChangesAsync();
        } 
        catch (DbUpdateConcurrencyException) 
        {
            throw new InvalidOperationException("A concurrency conflict occurred. Please try again.");
        }

        return await GetTransactionByIdAsync(transaction.Id) ?? throw new Exception("Failed to retrieve created transaction.");
    }

    public async Task<StockTransactionDto> AdjustStockAsync(StockOperationDto dto)
    {
        if (dto.Quantity < 0) throw new ArgumentException("Actual counted quantity cannot be negative.");

        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null) throw new ArgumentException("Product not found.");

        var previousStock = product.CurrentStock;
        var newStock = dto.Quantity;
        var difference = newStock - previousStock;

        product.CurrentStock = newStock;
        product.UpdatedAt = DateTime.UtcNow;

        var transaction = new StockTransaction
        {
            TransactionReference = $"ADJ-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
            ProductId = dto.ProductId,
            TransactionType = TransactionType.Adjustment,
            Quantity = difference, // For adjustments, quantity holds the difference
            PreviousStock = previousStock,
            NewStock = newStock,
            Notes = dto.Notes
        };

        _context.StockTransactions.Add(transaction);
        
        try 
        {
            await _context.SaveChangesAsync();
        } 
        catch (DbUpdateConcurrencyException) 
        {
            throw new InvalidOperationException("A concurrency conflict occurred. Please try again.");
        }

        return await GetTransactionByIdAsync(transaction.Id) ?? throw new Exception("Failed to retrieve created transaction.");
    }
}
