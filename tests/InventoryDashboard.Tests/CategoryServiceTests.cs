using InventoryDashboard.Api.Data;
using InventoryDashboard.Api.DTOs;
using InventoryDashboard.Api.Entities;
using InventoryDashboard.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InventoryDashboard.Tests;

public class CategoryServiceTests
{
    private ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task DeleteCategory_ThrowsInvalidOperationException_WhenCategoryContainsProducts()
    {
        // Arrange
        using var context = CreateDbContext();
        var category = new Category { Name = "Automotive" };
        var product = new Product
        {
            ProductCode = "AUTO-001",
            Name = "Brake Pads",
            Category = category,
            CurrentStock = 15,
            UnitPrice = 45.00m,
            RowVersion = new byte[] { 1, 0, 0, 0 }
        };
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        // Act & Assert (Referential Integrity Protection)
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.DeleteCategoryAsync(category.Id));

        Assert.Equal("Cannot delete category because it has products associated with it.", exception.Message);

        var existingCategory = await context.Categories.FindAsync(category.Id);
        Assert.NotNull(existingCategory);
    }

    [Fact]
    public async Task CreateCategory_ThrowsArgumentException_WhenNameNotUnique()
    {
        // Arrange
        using var context = CreateDbContext();
        context.Categories.Add(new Category { Name = "Furniture" });
        await context.SaveChangesAsync();

        var service = new CategoryService(context);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateCategoryAsync(new CreateCategoryDto { Name = "Furniture" }));

        Assert.Equal("Category name must be unique.", exception.Message);
    }
}
