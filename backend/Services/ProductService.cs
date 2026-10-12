using InventoryDashboard.Api.Data;
using InventoryDashboard.Api.DTOs;
using InventoryDashboard.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryDashboard.Api.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<ProductDto>> GetProductsAsync(ProductQuery query)
    {
        var queryable = _context.Products.Include(p => p.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            queryable = queryable.Where(p => p.Name.ToLower().Contains(search) || p.ProductCode.ToLower().Contains(search));
        }

        if (query.CategoryId.HasValue)
        {
            queryable = queryable.Where(p => p.CategoryId == query.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.StockStatus))
        {
            var status = query.StockStatus.ToLower();
            if (status == "outofstock")
                queryable = queryable.Where(p => p.CurrentStock == 0);
            else if (status == "lowstock")
                queryable = queryable.Where(p => p.CurrentStock > 0 && p.CurrentStock <= p.MinimumStockLevel);
            else if (status == "instock")
                queryable = queryable.Where(p => p.CurrentStock > p.MinimumStockLevel);
        }

        queryable = query.SortBy?.ToLower() switch
        {
            "name" => query.IsDescending ? queryable.OrderByDescending(p => p.Name) : queryable.OrderBy(p => p.Name),
            "price" => query.IsDescending ? queryable.OrderByDescending(p => p.UnitPrice) : queryable.OrderBy(p => p.UnitPrice),
            "stock" => query.IsDescending ? queryable.OrderByDescending(p => p.CurrentStock) : queryable.OrderBy(p => p.CurrentStock),
            _ => query.IsDescending ? queryable.OrderByDescending(p => p.CreatedAt) : queryable.OrderBy(p => p.CreatedAt)
        };

        var totalCount = await queryable.CountAsync();
        
        var items = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                ProductCode = p.ProductCode,
                Name = p.Name,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                UnitPrice = p.UnitPrice,
                CurrentStock = p.CurrentStock,
                MinimumStockLevel = p.MinimumStockLevel,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return new PaginatedResponse<ProductDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var p = await _context.Products.Include(pr => pr.Category).FirstOrDefaultAsync(pr => pr.Id == id);
        if (p == null) return null;

        return new ProductDto
        {
            Id = p.Id,
            ProductCode = p.ProductCode,
            Name = p.Name,
            Description = p.Description,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name,
            UnitPrice = p.UnitPrice,
            CurrentStock = p.CurrentStock,
            MinimumStockLevel = p.MinimumStockLevel,
            CreatedAt = p.CreatedAt
        };
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
    {
        if (await _context.Products.AnyAsync(p => p.ProductCode == createDto.ProductCode))
        {
            throw new ArgumentException("Product code must be unique.");
        }
        
        if (!await _context.Categories.AnyAsync(c => c.Id == createDto.CategoryId))
        {
            throw new ArgumentException("Category does not exist.");
        }

        var product = new Product
        {
            ProductCode = createDto.ProductCode,
            Name = createDto.Name,
            Description = createDto.Description,
            CategoryId = createDto.CategoryId,
            UnitPrice = createDto.UnitPrice,
            CurrentStock = createDto.InitialStockQuantity,
            MinimumStockLevel = createDto.MinimumStockLevel
        };

        _context.Products.Add(product);
        
        if (createDto.InitialStockQuantity > 0)
        {
            var transaction = new StockTransaction
            {
                TransactionReference = $"INIT-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
                Product = product,
                TransactionType = TransactionType.StockIn,
                Quantity = createDto.InitialStockQuantity,
                PreviousStock = 0,
                NewStock = createDto.InitialStockQuantity,
                Notes = "Initial stock quantity"
            };
            _context.StockTransactions.Add(transaction);
        }

        await _context.SaveChangesAsync();

        return await GetProductByIdAsync(product.Id) ?? throw new Exception("Failed to retrieve created product.");
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateDto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return null;

        if (product.CategoryId != updateDto.CategoryId && !await _context.Categories.AnyAsync(c => c.Id == updateDto.CategoryId))
        {
            throw new ArgumentException("Category does not exist.");
        }

        product.Name = updateDto.Name;
        product.Description = updateDto.Description;
        product.CategoryId = updateDto.CategoryId;
        product.UnitPrice = updateDto.UnitPrice;
        product.MinimumStockLevel = updateDto.MinimumStockLevel;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetProductByIdAsync(product.Id);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.Include(p => p.StockTransactions).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return false;

        if (product.StockTransactions.Any())
        {
            throw new InvalidOperationException("Cannot delete product because it has stock transaction history.");
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}
