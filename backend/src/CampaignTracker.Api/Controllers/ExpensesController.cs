using System.Text;
using CampaignTracker.Api.Auth;
using CampaignTracker.Api.DTOs;
using CampaignTracker.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampaignTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/expenses")]
public class ExpensesController : ControllerBase
{
    private readonly ExpenseService _expenses;
    private readonly CategoryService _categories;

    public ExpensesController(ExpenseService expenses, CategoryService categories)
    {
        _expenses = expenses;
        _categories = categories;
    }

    [HttpGet]
    public async Task<ActionResult<List<ExpenseDto>>> GetAll([FromQuery] ExpenseFilter filter)
    {
        return await _expenses.GetAllAsync(User.GetUserId(), filter);
    }

    /// <summary>Totals by category and by month for the charts.</summary>
    [HttpGet("summary")]
    public async Task<ActionResult<ExpenseSummaryDto>> Summary([FromQuery] ExpenseFilter filter)
    {
        var expenses = await _expenses.GetAllAsync(User.GetUserId(), filter);
        return ExpenseReportService.Summarize(expenses);
    }

    /// <summary>Downloads the filtered expenses as a CSV file.</summary>
    [HttpGet("export")]
    [Produces("text/csv")]
    public async Task<IActionResult> Export([FromQuery] ExpenseFilter filter)
    {
        var expenses = await _expenses.GetAllAsync(User.GetUserId(), filter);
        var csv = ExpenseCsvExporter.ToCsv(expenses.OrderBy(e => e.Date).ThenBy(e => e.Id));

        // The UTF-8 BOM makes Excel detect the encoding (umlauts, emoji in descriptions).
        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv)).ToArray();
        var fileName = $"expenses-{DateTime.UtcNow:yyyyMMdd}.csv";

        return File(bytes, "text/csv; charset=utf-8", fileName);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExpenseDto>> Get(int id)
    {
        var expense = await _expenses.GetAsync(User.GetUserId(), id);
        return expense is null ? NotFound() : expense;
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseDto>> Create(ExpenseRequest request)
    {
        var userId = User.GetUserId();
        if (!await IsValidCategoryAsync(userId, request.CategoryId!.Value))
        {
            return ValidationProblem(ModelState);
        }

        var id = await _expenses.CreateAsync(userId, request);
        var expense = await _expenses.GetAsync(userId, id);
        return CreatedAtAction(nameof(Get), new { id }, expense);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ExpenseRequest request)
    {
        var userId = User.GetUserId();
        if (!await IsValidCategoryAsync(userId, request.CategoryId!.Value))
        {
            return ValidationProblem(ModelState);
        }

        return await _expenses.UpdateAsync(userId, id, request) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return await _expenses.DeleteAsync(User.GetUserId(), id) ? NoContent() : NotFound();
    }

    private async Task<bool> IsValidCategoryAsync(int userId, int categoryId)
    {
        if (await _categories.ExistsAsync(userId, categoryId))
        {
            return true;
        }

        ModelState.AddModelError(nameof(ExpenseRequest.CategoryId), "Category not found.");
        return false;
    }
}
