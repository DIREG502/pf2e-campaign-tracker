using CampaignTracker.Api.Auth;
using CampaignTracker.Api.DTOs;
using CampaignTracker.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampaignTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _categories;

    public CategoriesController(CategoryService categories)
    {
        _categories = categories;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll()
    {
        return await _categories.GetAllAsync(User.GetUserId());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Get(int id)
    {
        var category = await _categories.GetAsync(User.GetUserId(), id);
        return category is null ? NotFound() : category;
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CategoryRequest request)
    {
        var (result, category) = await _categories.CreateAsync(User.GetUserId(), request);

        if (result == CategoryResult.DuplicateName)
        {
            return DuplicateName();
        }

        return CreatedAtAction(nameof(Get), new { id = category!.Id }, category);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CategoryRequest request)
    {
        return await _categories.UpdateAsync(User.GetUserId(), id, request) switch
        {
            CategoryResult.NotFound => NotFound(),
            CategoryResult.DuplicateName => DuplicateName(),
            _ => NoContent()
        };
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return await _categories.DeleteAsync(User.GetUserId(), id) switch
        {
            CategoryResult.NotFound => NotFound(),
            CategoryResult.InUse => Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Category in use",
                detail: "Move or delete the expenses in this category first."),
            _ => NoContent()
        };
    }

    private ObjectResult DuplicateName() => Problem(
        statusCode: StatusCodes.Status409Conflict,
        title: "Duplicate category",
        detail: "You already have a category with this name.");
}
