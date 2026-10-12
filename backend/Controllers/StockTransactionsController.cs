using InventoryDashboard.Api.DTOs;
using InventoryDashboard.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryDashboard.Api.Controllers;

[ApiController]
[Route("api/stock-transactions")]
public class StockTransactionsController : ControllerBase
{
    private readonly IStockTransactionService _service;

    public StockTransactionsController(IStockTransactionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<StockTransactionDto>>> GetAll([FromQuery] PaginationQuery query)
    {
        return Ok(await _service.GetTransactionsAsync(query));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StockTransactionDto>> Get(int id)
    {
        var transaction = await _service.GetTransactionByIdAsync(id);
        if (transaction == null) return NotFound();
        return Ok(transaction);
    }

    [HttpPost("stock-in")]
    public async Task<ActionResult<StockTransactionDto>> StockIn(StockOperationDto dto)
    {
        try
        {
            var result = await _service.StockInAsync(dto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails { Detail = ex.Message });
        }
    }

    [HttpPost("stock-out")]
    public async Task<ActionResult<StockTransactionDto>> StockOut(StockOperationDto dto)
    {
        try
        {
            var result = await _service.StockOutAsync(dto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails { Detail = ex.Message });
        }
    }

    [HttpPost("adjustment")]
    public async Task<ActionResult<StockTransactionDto>> AdjustStock(StockOperationDto dto)
    {
        try
        {
            var result = await _service.AdjustStockAsync(dto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails { Detail = ex.Message });
        }
    }
}
