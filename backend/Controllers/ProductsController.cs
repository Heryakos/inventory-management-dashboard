using InventoryDashboard.Api.DTOs;
using InventoryDashboard.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IStockTransactionService _stockTransactionService;

    public ProductsController(IProductService productService, IStockTransactionService stockTransactionService)
    {
        _productService = productService;
        _stockTransactionService = stockTransactionService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<ProductDto>>> GetAll([FromQuery] ProductQuery query)
    {
        return Ok(await _productService.GetProductsAsync(query));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> Get(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
    {
        try
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }
        catch (ArgumentException ex)
        {
            return Conflict(new ProblemDetails { Detail = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> Update(int id, UpdateProductDto dto)
    {
        try
        {
            var product = await _productService.UpdateProductAsync(id, dto);
            if (product == null) return NotFound();
            return Ok(product);
        }
        catch (ArgumentException ex)
        {
            return Conflict(new ProblemDetails { Detail = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
    }

    [HttpGet("{id}/stock-transactions")]
    public async Task<ActionResult<PaginatedResponse<StockTransactionDto>>> GetTransactions(int id, [FromQuery] PaginationQuery query)
    {
        return Ok(await _stockTransactionService.GetTransactionsByProductAsync(id, query));
    }
}
