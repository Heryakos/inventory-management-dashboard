using InventoryDashboard.Api.Data;
using InventoryDashboard.Api.DTOs;
using InventoryDashboard.Api.Entities;
using InventoryDashboard.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InventoryDashboard.Tests;

public class StockTransactionServiceTests
{
    private ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task StockIn_IncreasesStock_AndCreatesAuditRecord()
    {
        // Arrange
        using var context = CreateDbContext();
        var category = new Category { Name = "Hardware" };
        var product = new Product
        {
            ProductCode = "SKU-001",
            Name = "Hammer",
            Category = category,
            CurrentStock = 10,
            UnitPrice = 15.00m,
            RowVersion = new byte[] { 1, 0, 0, 0 }
        };
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new StockTransactionService(context);

        // Act
        var result = await service.StockInAsync(new StockOperationDto
        {
            ProductId = product.Id,
            Quantity = 5,
            Notes = "Supplier delivery"
        });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(15, result.NewStock);
        Assert.Equal(10, result.PreviousStock);
        Assert.Equal(5, result.Quantity);
        Assert.Equal("StockIn", result.TransactionType);

        var updatedProduct = await context.Products.FindAsync(product.Id);
        Assert.Equal(15, updatedProduct!.CurrentStock);
    }

    [Fact]
    public async Task StockOut_DecreasesStock_WhenSufficientQuantity()
    {
        // Arrange
        using var context = CreateDbContext();
        var category = new Category { Name = "Hardware" };
        var product = new Product
        {
            ProductCode = "SKU-002",
            Name = "Wrench",
            Category = category,
            CurrentStock = 20,
            UnitPrice = 25.00m,
            RowVersion = new byte[] { 1, 0, 0, 0 }
        };
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new StockTransactionService(context);

        // Act
        var result = await service.StockOutAsync(new StockOperationDto
        {
            ProductId = product.Id,
            Quantity = 8,
            Notes = "Customer order ship"
        });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(12, result.NewStock);
        Assert.Equal(20, result.PreviousStock);
        Assert.Equal(8, result.Quantity);
        Assert.Equal("StockOut", result.TransactionType);

        var updatedProduct = await context.Products.FindAsync(product.Id);
        Assert.Equal(12, updatedProduct!.CurrentStock);
    }

    [Fact]
    public async Task StockOut_ThrowsInvalidOperationException_WhenStockInsufficient()
    {
        // Arrange
        using var context = CreateDbContext();
        var category = new Category { Name = "Electronics" };
        var product = new Product
        {
            ProductCode = "SKU-003",
            Name = "Laptop",
            Category = category,
            CurrentStock = 5,
            UnitPrice = 999.00m,
            RowVersion = new byte[] { 1, 0, 0, 0 }
        };
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new StockTransactionService(context);

        // Act & Assert (Negative Stock Prevention)
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.StockOutAsync(new StockOperationDto
            {
                ProductId = product.Id,
                Quantity = 10,
                Notes = "Invalid order exceeding inventory"
            }));

        Assert.Equal("Cannot stock out more than the current stock.", exception.Message);

        var unchangedProduct = await context.Products.FindAsync(product.Id);
        Assert.Equal(5, unchangedProduct!.CurrentStock);
    }

    [Fact]
    public async Task AdjustStock_UpdatesInventoryBalance_AndCalculatesDifference()
    {
        // Arrange
        using var context = CreateDbContext();
        var category = new Category { Name = "Office" };
        var product = new Product
        {
            ProductCode = "SKU-004",
            Name = "Printer Paper",
            Category = category,
            CurrentStock = 50,
            UnitPrice = 8.50m,
            RowVersion = new byte[] { 1, 0, 0, 0 }
        };
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new StockTransactionService(context);

        // Act (Stock reconciliation audit)
        var result = await service.AdjustStockAsync(new StockOperationDto
        {
            ProductId = product.Id,
            Quantity = 42, // Physical count is 42
            Notes = "Physical cycle count discrepancy"
        });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(42, result.NewStock);
        Assert.Equal(50, result.PreviousStock);
        Assert.Equal(-8, result.Quantity); // Delta = -8
        Assert.Equal("Adjustment", result.TransactionType);

        var updatedProduct = await context.Products.FindAsync(product.Id);
        Assert.Equal(42, updatedProduct!.CurrentStock);
    }
}
