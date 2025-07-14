using Microsoft.EntityFrameworkCore;
using TxTracker.TransactionsApi.Data;
using TxTracker.TransactionsApi.Data.Models;

namespace TxTracker.TransactionsApi.Categories;

public class CategoriesRepository
{
    private readonly TransactionsContext dbContext;
    private DbSet<Category> Categories { get => dbContext.Categories; }
    private readonly ILogger<CategoriesRepository> logger;

    public CategoriesRepository(TransactionsContext dbContext, ILogger<CategoriesRepository> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public virtual bool Exists(Guid categoryId)
    {
        return Categories.Any((category) => category.Id == categoryId);
    }

    public virtual async Task<List<Category>> GetAsync(CancellationToken cancellationToken = default)
    {
        var ret = await Categories.ToListAsync();
        return ret;
    }

    public virtual async Task<List<Category>> GetAsync(int page, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), page, "Page parameter must be greater than 0.");
        }

        var ret = await Categories.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return ret;
    }

    public virtual async Task<Category?> GetAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var ret = await Categories.SingleOrDefaultAsync((c) => c.Id == categoryId, cancellationToken);

        return ret;
    }

    public virtual async Task<Category> CreateAsync(string categoryName, CancellationToken cancellationToken = default)
    {

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = categoryName
        };

        Categories.Add(category);

        await dbContext.SaveChangesAsync(cancellationToken);

        return category;
    }

    public virtual async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        if (Exists(category.Id))
        {
            throw new CategoryConflictException(category.Id);
        }

        Categories.Add(category);

        await dbContext.SaveChangesAsync(cancellationToken);

        return category;
    }

    public virtual async Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(category.Name))
        {
            throw new ArgumentException($"Parameter {nameof(category.Name)} cannot be a null or empty.");
        }

        var c = await GetAsync(category.Id, cancellationToken);

        if (c == null)
        {
            throw new CategoryNotFoundException(category.Id);
        }

        c.Name = category.Name;

        await dbContext.SaveChangesAsync(cancellationToken);

        return c;
    }

    public virtual async Task<Category> UpsertAsync(Category category, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(category.Name))
        {
            throw new ArgumentException($"Parameter {nameof(category.Name)} cannot be null or empty.");
        }

        if (Exists(category.Id))
        {
            return await CreateAsync(category, cancellationToken);
        }
        else
        {
            return await UpdateAsync(category, cancellationToken);
        }
    }

    public virtual async Task DeleteAsync(Category category, CancellationToken cancellationToken = default)
    {
        await DeleteAsync(category.Id, cancellationToken);
    }

    public virtual async Task DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var c = await GetAsync(categoryId, cancellationToken);

        if (c is null)
        {
            throw new CategoryNotFoundException(categoryId);
        }

        Categories.Remove(c);

        await dbContext.SaveChangesAsync();
    }

}