using Microsoft.EntityFrameworkCore;
using TxTracker.TransactionsApi.Data;
using TxTracker.TransactionsApi.Data.Models;

namespace TxTracker.TransactionsApi.Accounts;

public class AccountsRepository
{
    private readonly TransactionsContext dbContext;
    private DbSet<Account> Accounts { get => dbContext.Accounts; }
    private readonly ILogger<AccountsRepository> logger;

    public AccountsRepository(TransactionsContext dbContext, ILogger<AccountsRepository> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public virtual bool Exists(Guid accountId)
    {
        return Accounts.Any((acc) => acc.Id == accountId);
    }

    public virtual async Task<List<Account>> GetAccountsAsync(CancellationToken cancellationToken = default)
    {
        var ret = await Accounts.ToListAsync();
        return ret;
    }

    public virtual async Task<List<Account>> GetAccountsAsync(int page, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), page, "Page parameter must be greater than 0.");
        }

        var ret = await Accounts.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return ret;
    }

    public virtual async Task<Account?> GetAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var ret = await Accounts.SingleOrDefaultAsync((a) => a.Id == accountId, cancellationToken);

        return ret;
    }

    public virtual async Task<Account> CreateAsync(string accountName, CancellationToken cancellationToken = default)
    {

        var account = new Account
        {
            Id = Guid.NewGuid(),
            Name = accountName
        };

        Accounts.Add(account);

        await dbContext.SaveChangesAsync(cancellationToken);

        return account;
    }

    public virtual async Task<Account> CreateAsync(Account account, CancellationToken cancellationToken = default)
    {
        if (Exists(account.Id))
        {
            throw new AccountConflictException(account.Id);
        }

        Accounts.Add(account);

        await dbContext.SaveChangesAsync(cancellationToken);

        return account;
    }

    public virtual async Task<Account> UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(account.Name))
        {
            throw new ArgumentException($"Parameter {nameof(account.Name)} cannot be a null or empty.");
        }

        var acc = await GetAsync(account.Id, cancellationToken);

        if (acc == null)
        {
            throw new AccountNotFoundException(account.Id);
        }

        acc.Name = account.Name;

        await dbContext.SaveChangesAsync(cancellationToken);

        return acc;
    }

    public virtual async Task<Account> UpsertAsync(Account account, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(account.Name))
        {
            throw new ArgumentException($"Parameter {nameof(account.Name)} cannot be null or empty.");
        }

        if (Exists(account.Id))
        {
            return await CreateAsync(account, cancellationToken);
        }
        else
        {
            return await UpdateAsync(account, cancellationToken);
        }
    }

    public virtual async Task DeleteAsync(Account account, CancellationToken cancellationToken = default)
    {
        await DeleteAsync(account.Id, cancellationToken);
    }

    public virtual async Task DeleteAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var acc = await GetAsync(accountId, cancellationToken);

        Accounts.Remove(acc);

        await dbContext.SaveChangesAsync();
    }

}