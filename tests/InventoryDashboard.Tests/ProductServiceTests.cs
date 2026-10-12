using InventoryDashboard.Api.Data;
using InventoryDashboard.Api.Entities;
using InventoryDashboard.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InventoryDashboard.Tests;

public class ProductServiceTests
{
    private ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task DeleteProduct_ThrowsInvalidOperationException_WhenProductHasTransactionHistory()
    {
        // Arrange
        using var context = CreateDbContext();
        var category = new Category { Name = "Toys" };
        var product = new Product
        {
            ProductCode = "TOY-001",
            Name = "Lego Set",
            Category = category,
            CurrentStock = 10,
            UnitPrice = 29.99m,
            RowVersion = new byte[] { 1, 0, 0, 0 }
        };
        var transaction = new StockTransaction
        {
            TransactionReference = "INIT-001",
            Product = product,
            TransactionType = TransactionType.StockIn,
            Quantity = 10,
            PreviousStock = 0,
            NewStock = 10
        };

        context.Categories.Add(category);
        context.Products.Add(product);
        context.StockTransactions.Add(transaction);
        await context.SaveChangesAsync();

        var service = new ProductService(context);

        // Act & Assert (Audit Integrity Protection)
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.DeleteProductAsync(product.Id));

        Assert.Equal("Cannot delete product because it has stock transaction history.", exception.Message);

        var existingProduct = await context.Products.FindAsync(product.Id);
        Assert.NotNull(existingProduct);
    }
}
