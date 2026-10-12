using InventoryDashboard.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryDashboard.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Categories.AnyAsync())
        {
            return; // DB has been seeded
        }

        var categories = new List<Category>
        {
            new Category { Name = "Electronics", Description = "Electronic devices and accessories" },
            new Category { Name = "Office Supplies", Description = "General office supplies" },
            new Category { Name = "Computer Accessories", Description = "Accessories for computers" }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        var products = new List<Product>
        {
            new Product
            {
                ProductCode = "LAP-001",
                Name = "Laptop",
                CategoryId = categories[0].Id,
                UnitPrice = 999.99m,
                CurrentStock = 15,
                MinimumStockLevel = 5,
                Description = "High performance laptop"
            },
            new Product
            {
                ProductCode = "MON-001",
                Name = "Monitor",
                CategoryId = categories[0].Id,
                UnitPrice = 199.99m,
                CurrentStock = 5,
                MinimumStockLevel = 5,
                Description = "27 inch 4K monitor" // At minimum stock
            },
            new Product
            {
                ProductCode = "KEY-001",
                Name = "Keyboard",
                CategoryId = categories[2].Id,
                UnitPrice = 49.99m,
                CurrentStock = 2,
                MinimumStockLevel = 10,
                Description = "Mechanical keyboard" // Below minimum stock
            },
            new Product
            {
                ProductCode = "MOU-001",
                Name = "Mouse",
                CategoryId = categories[2].Id,
                UnitPrice = 29.99m,
                CurrentStock = 0,
                MinimumStockLevel = 5,
                Description = "Wireless mouse" // Out of stock
            },
            new Product
            {
                ProductCode = "PRI-001",
                Name = "Printer",
                CategoryId = categories[1].Id,
                UnitPrice = 149.99m,
                CurrentStock = 8,
                MinimumStockLevel = 3
            },
            new Product
            {
                ProductCode = "USB-001",
                Name = "USB Cable",
                CategoryId = categories[2].Id,
                UnitPrice = 9.99m,
                CurrentStock = 50,
                MinimumStockLevel = 20
            }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        var transactions = new List<StockTransaction>();
        foreach (var product in products)
        {
            if (product.CurrentStock > 0)
            {
                transactions.Add(new StockTransaction
                {
                    TransactionReference = $"INIT-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
                    ProductId = product.Id,
                    TransactionType = TransactionType.StockIn,
                    Quantity = product.CurrentStock,
                    PreviousStock = 0,
                    NewStock = product.CurrentStock,
                    Notes = "Initial seed data"
                });
            }
        }
        
        // Add a few historical movements
        // For Laptop (Current 15)
        // Let's pretend it was 20, then 5 were removed
        transactions.Add(new StockTransaction
        {
            TransactionReference = $"OUT-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
            ProductId = products[0].Id,
            TransactionType = TransactionType.StockOut,
            Quantity = 5,
            PreviousStock = 20,
            NewStock = 15,
            Notes = "Sample stock out",
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        });

        await context.StockTransactions.AddRangeAsync(transactions);
        await context.SaveChangesAsync();
    }
}
