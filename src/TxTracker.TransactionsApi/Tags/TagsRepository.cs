using Microsoft.EntityFrameworkCore;
using TxTracker.TransactionsApi.Data;

namespace TxTracker.TransactionsApi.Tags;

public class TagsRepository
{
    private readonly TransactionsContext dbContext;
    private DbSet<Tag> Tags { get => dbContext.Tags; }
    private readonly ILogger<TagsRepository> logger;

    public TagsRepository(TransactionsContext dbContext, ILogger<TagsRepository> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public virtual bool Exists(Guid tagId)
    {
        return Tags.Any((t) => t.Id == tagId);
    }

    public virtual async Task<List<Tag>> GetAsync(CancellationToken cancellationToken = default)
    {
        var ret = await Tags.ToListAsync();
        return ret;
    }

    public virtual async Task<List<Tag>> GetAsync(int page, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), page, "Page parameter must be greater than 0.");
        }

        var ret = await Tags.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return ret;
    }

    public virtual async Task<Tag?> GetAsync(Guid tagId, CancellationToken cancellationToken = default)
    {
        var ret = await Tags.SingleOrDefaultAsync((t) => t.Id == tagId, cancellationToken);

        return ret;
    }

    public virtual async Task<Tag> CreateAsync(string tagName, CancellationToken cancellationToken = default)
    {

        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = tagName
        };

        Tags.Add(tag);

        await dbContext.SaveChangesAsync(cancellationToken);

        return tag;
    }

    public virtual async Task<Tag> CreateAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        if (Exists(tag.Id))
        {
            throw new TagConflictException(tag.Id);
        }

        Tags.Add(tag);

        await dbContext.SaveChangesAsync(cancellationToken);

        return tag;
    }

    public virtual async Task<Tag> UpdateAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(tag.Name))
        {
            throw new ArgumentException($"Parameter {nameof(Tag.Name)} cannot be a null or empty.");
        }

        var t = await GetAsync(tag.Id, cancellationToken);

        if (t == null)
        {
            throw new TagNotFoundException(tag.Id);
        }

        t.Name = t.Name;

        await dbContext.SaveChangesAsync(cancellationToken);

        return t;
    }

    public virtual async Task<Tag> UpsertAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(tag.Name))
        {
            throw new ArgumentException($"Parameter {nameof(Tag.Name)} cannot be null or empty.");
        }

        if (Exists(tag.Id))
        {
            return await CreateAsync(tag, cancellationToken);
        }
        else
        {
            return await UpdateAsync(tag, cancellationToken);
        }
    }

    public virtual async Task DeleteAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        await DeleteAsync(tag.Id, cancellationToken);
    }

    public virtual async Task DeleteAsync(Guid tagId, CancellationToken cancellationToken = default)
    {
        var t = await GetAsync(tagId, cancellationToken);

        if (t is null)
        {
            throw new TagNotFoundException(tagId);
        }

        Tags.Remove(t);

        await dbContext.SaveChangesAsync();
    }

}