using TxTracker.TransactionsApi.Accounts;
using TxTracker.TransactionsApi.Categories;
using TxTracker.TransactionsApi.Tags;

namespace TxTracker.TransactionsApi.Transactions;

public class Transaction
{
    /// <summary>
    /// Internal Id for the transaction.
    /// </summary>
    public Guid Id { get; set; }
    public required double Amount { get; set; }
    public required DateTime Date { get; set; }
    public required Account From { get; set; }

    /// <summary>
    /// Provides an identifier used by the institution that is tracking this
    /// </summary>
    public required string AccountTransactionId { get; set; }
    public required Account To { get; set; }
    public Category? Category { get; set; }
    public List<Tag> Tags { get; set; } = new List<Tag>();
}