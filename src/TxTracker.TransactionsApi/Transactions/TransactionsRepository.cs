using Microsoft.EntityFrameworkCore;
using TxTracker.TransactionsApi.Data;

namespace TxTracker.TransactionsApi.Transactions;

public class TransactionsRepository
{
    private readonly TransactionsContext dbContext;
    private DbSet<Transaction> Transactions { get => dbContext.Transactions; }
    private readonly ILogger<TransactionsRepository> logger;

    public TransactionsRepository(TransactionsContext dbContext, ILogger<TransactionsRepository> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public virtual bool Exists(Guid transactionId)
    {
        return Transactions.Any((transaction) => transaction.Id == transactionId);
    }

    public virtual async Task<List<Transaction>> GetAsync(CancellationToken cancellationToken = default)
    {
        var ret = await Transactions.ToListAsync();
        return ret;
    }

    public virtual async Task<List<Transaction>> GetAsync(int page, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), page, "Page parameter must be greater than 0.");
        }

        var ret = await Transactions.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return ret;
    }

    public virtual async Task<Transaction?> GetAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        var ret = await Transactions.SingleOrDefaultAsync((transaction) => transaction.Id == transactionId, cancellationToken);

        return ret;
    }

    public virtual async Task<Transaction> CreateAsync(CreateTransactionRequest createTransactionRequest, CancellationToken cancellationToken = default)
    {
        if (createTransactionRequest.Id == null)
        {
            createTransactionRequest.Id = Guid.NewGuid();
        }

        var transaction = createTransactionRequest.ToTransaction();

        Transactions.Add(transaction);

        await dbContext.SaveChangesAsync(cancellationToken);

        return transaction;
    }

    public virtual async Task<Transaction> CreateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        if (Exists(transaction.Id))
        {
            throw new TransactionConflictException(transaction.Id);
        }

        Transactions.Add(transaction);

        await dbContext.SaveChangesAsync(cancellationToken);

        return transaction;
    }

    public virtual async Task<Transaction> UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        var t = await GetAsync(transaction.Id, cancellationToken);

        if (t == null)
        {
            throw new TransactionNotFoundException(transaction.Id);
        }

        t = transaction;

        await dbContext.SaveChangesAsync(cancellationToken);

        return t;
    }

    public virtual async Task<Transaction> UpsertAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        if (Exists(transaction.Id))
        {
            return await CreateAsync(transaction, cancellationToken);
        }
        else
        {
            return await UpdateAsync(transaction, cancellationToken);
        }
    }

    public virtual async Task DeleteAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        await DeleteAsync(transaction.Id, cancellationToken);
    }

    public virtual async Task DeleteAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        var transaction = await GetAsync(transactionId, cancellationToken);

        if (transaction is null)
        {
            throw new TransactionNotFoundException(transactionId);
        }

        Transactions.Remove(transaction);

        await dbContext.SaveChangesAsync();
    }

}