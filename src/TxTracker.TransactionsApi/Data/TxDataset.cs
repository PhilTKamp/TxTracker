using Microsoft.EntityFrameworkCore;

namespace TxTracker.TransactionsApi.Data;

public abstract class TxDataset<T>
{
    private readonly TransactionsContext dbContext;

    public TxDataset(TransactionsContext dbContext)
    {
        this.dbContext = dbContext;
    }

}