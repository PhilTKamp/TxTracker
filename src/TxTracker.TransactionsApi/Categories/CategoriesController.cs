using Microsoft.AspNetCore.Mvc;

namespace TxTracker.TransactionsApi.Categories;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly CategoriesRepository categoriesRepository;

    private readonly ILogger<CategoriesController>? logger;

    public CategoriesController(CategoriesRepository categoriesRepository)
    {
        this.categoriesRepository = categoriesRepository;
    }

    public CategoriesController(CategoriesRepository categoriesRepository, ILogger<CategoriesController> logger)
    {
        this.categoriesRepository = categoriesRepository;
        this.logger = logger;
    }

    [HttpGet(Name = "GetCategories")]
    public async Task<ActionResult<IEnumerable<Category>>> GetAsync([FromQuery] int page = 0, [FromQuery] int pageSize = -1, CancellationToken cancellationToken = default)
    {
        if (pageSize == -1)
        {
            var accounts = await categoriesRepository.GetAsync(cancellationToken);
            return Ok(accounts);
        }
        else
        {
            var accounts = await categoriesRepository.GetAsync(page, pageSize, cancellationToken);
            return Ok(accounts);
        }
    }

    [HttpGet("{id}", Name = "GetCategory")]
    public async Task<ActionResult<Category>> GetCategory(string id, CancellationToken cancellationToken = default)
    {
        var validGuid = Guid.TryParse(id, out var guid);

        if (!validGuid)
        {
            return BadRequest();
        }

        try
        {
            var category = await categoriesRepository.GetAsync(guid, cancellationToken);
            if (category is null)
            {
                return NotFound();
            }
            return Ok(category);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Unhandled exception occurred retrieving category by id '{categoryId}'", id);
            return Problem("Error occurred when retrieving the category.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory([FromBody] CreateCategoryRequest req, CancellationToken cancellationToken = default)
    {
        if (req.Id == null)
        {
            try
            {
                var category = await categoriesRepository.CreateAsync(req.Name, cancellationToken);
                logger?.LogInformation("Category created. Id {categoryId}, Name {categoryName}", category.Id, category.Name);
                return Accepted(category);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when creating category. Create category request: Id={categoryId}, Name={categoryName}", req.Id, req.Name);
                return Problem("Error occurred when creating the category");
            }
        }
        else
        {
            try
            {
                var category = await categoriesRepository.CreateAsync(req.ToCategory());
                logger?.LogInformation("Category created. Id {categoryId}, Name {categoryName}", category.Id, category.Name);
                return CreatedAtAction(nameof(GetCategory), category);
            }
            catch (CategoryConflictException)
            {
                logger?.LogInformation("Conflicting category found on create request. Conflicting ID '{categoryId}'", req.Id);
                return Conflict(new { message = $"Category with ID '{req.Id}' already exists." });
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when creating category. Create category request: Id={categoryId}, Name={categoryName}", req.Id, req.Name);
                return Problem("Error occurred when creating the category");
            }
        }
    }

    [HttpPut]
    public async Task<ActionResult<Category>> UpdateCategory([FromBody] Category category, CancellationToken cancellationToken = default)
    {
        if (categoriesRepository.Exists(category.Id))
        {
            try
            {
                var c = await categoriesRepository.UpdateAsync(category, cancellationToken);

                return Ok(c);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when creating category. Update category request: Id={categoryId}, Name={categoryName}", category.Id, category.Name);
                return Problem("Error occurred when attempting to create the category.");
            }

        }
        else
        {
            try
            {
                var acc = await categoriesRepository.CreateAsync(category, cancellationToken);

                return CreatedAtAction(nameof(GetCategory), acc);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when updating an category. Update category request: Id={categoryId}, Name={categoryName}", category.Id, category.Name);
                return Problem("Error occurred when attempting to update the category.");
            }
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(string id, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(id, out var catGuid))
        {
            return BadRequest();
        }

        if (categoriesRepository.Exists(catGuid))
        {
            try
            {
                await categoriesRepository.DeleteAsync(catGuid, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when deleting an category. Delete category request: Id={categoryId}", id);
                return Problem("Error occurred when attempting to delete the category.");
            }
        }
        else
        {
            return NotFound();
        }
    }
}
