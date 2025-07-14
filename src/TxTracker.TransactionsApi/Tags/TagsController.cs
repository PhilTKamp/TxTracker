using Microsoft.AspNetCore.Mvc;

namespace TxTracker.TransactionsApi.Tags;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly TagsRepository tagsRepository;

    private readonly ILogger<TagsController>? logger;

    public TagsController(TagsRepository tagsRepository)
    {
        this.tagsRepository = tagsRepository;
    }

    public TagsController(TagsRepository tagsRepository, ILogger<TagsController> logger)
    {
        this.tagsRepository = tagsRepository;
        this.logger = logger;
    }

    [HttpGet(Name = "GetTags")]
    public async Task<ActionResult<IEnumerable<Tag>>> GetAsync([FromQuery] int page = 0, [FromQuery] int pageSize = -1, CancellationToken cancellationToken = default)
    {
        if (pageSize == -1)
        {
            var accounts = await tagsRepository.GetAsync(cancellationToken);
            return Ok(accounts);
        }
        else
        {
            var accounts = await tagsRepository.GetAsync(page, pageSize, cancellationToken);
            return Ok(accounts);
        }
    }

    [HttpGet("{id}", Name = "GetTag")]
    public async Task<ActionResult<Tag>> GetTagAsync(string id, CancellationToken cancellationToken = default)
    {
        var validGuid = Guid.TryParse(id, out var guid);

        if (!validGuid)
        {
            return BadRequest();
        }

        try
        {
            var tag = await tagsRepository.GetAsync(guid, cancellationToken);
            if (tag is null)
            {
                return NotFound();
            }
            return Ok(tag);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Unhandled exception occurred retrieving tag by id '{tagId}'", id);
            return Problem("Error occurred when retrieving the tag.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Tag>> CreateTag([FromBody] CreateTagRequest req, CancellationToken cancellationToken = default)
    {
        if (req.Id == null)
        {
            try
            {
                var tag = await tagsRepository.CreateAsync(req.Name, cancellationToken);
                logger?.LogInformation("Tag created. Id {tagId}, Name {tagName}", tag.Id, tag.Name);
                return Accepted(tag);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when creating tag. Create tag request: Id={tagId}, Name={tagName}", req.Id, req.Name);
                return Problem("Error occurred when creating the tag");
            }
        }
        else
        {
            try
            {
                var tag = await tagsRepository.CreateAsync(req.ToTag());
                logger?.LogInformation("Tag created. Id {tagId}, Name {tagName}", tag.Id, tag.Name);
                return CreatedAtAction(nameof(GetTagAsync), tag);
            }
            catch (TagConflictException)
            {
                logger?.LogInformation("Conflicting tag found on create request. Conflicting ID '{tagId}'", req.Id);
                return Conflict(new { message = $"Tag with ID '{req.Id}' already exists." });
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when creating tag. Create tag request: Id={tagId}, Name={tagName}", req.Id, req.Name);
                return Problem("Error occurred when creating the tag");
            }
        }
    }

    [HttpPut]
    public async Task<ActionResult<Tag>> UpdateTag([FromBody] Tag tag, CancellationToken cancellationToken = default)
    {
        if (tagsRepository.Exists(tag.Id))
        {
            try
            {
                var t = await tagsRepository.UpdateAsync(tag, cancellationToken);

                return Ok(t);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when creating tag. Update tag request: Id={tagId}, Name={tagName}", tag.Id, tag.Name);
                return Problem("Error occurred when attempting to create the tag.");
            }

        }
        else
        {
            try
            {
                var acc = await tagsRepository.CreateAsync(tag, cancellationToken);

                return CreatedAtAction(nameof(GetTagAsync), acc);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when updating an tag. Update tag request: Id={tagId}, Name={tagName}", tag.Id, tag.Name);
                return Problem("Error occurred when attempting to update the tag.");
            }
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTag(string id, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(id, out var tagGuid))
        {
            return BadRequest();
        }

        if (tagsRepository.Exists(tagGuid))
        {
            try
            {
                await tagsRepository.DeleteAsync(tagGuid, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when deleting an tag. Delete tag request: Id={tagId}", id);
                return Problem("Error occurred when attempting to delete the tag.");
            }
        }
        else
        {
            return NotFound();
        }
    }
}
