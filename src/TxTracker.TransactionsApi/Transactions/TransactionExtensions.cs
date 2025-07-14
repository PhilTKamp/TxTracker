namespace TxTracker.TransactionsApi.Transactions;

public static class TransactionExtensions
{
    public static Transaction ToTransaction(this CreateTransactionRequest createTransactionRequest)
    {
        if (createTransactionRequest.Id == null)
        {
            throw new ArgumentNullException(nameof(createTransactionRequest.Id), "Transaction ID cannot be null");
        }

        return new Transaction()
        {
            Id = (Guid)createTransactionRequest.Id, // Check for null above
            Amount = createTransactionRequest.Amount,
            Date = createTransactionRequest.Date,
            From = createTransactionRequest.From,
            AccountTransactionId = createTransactionRequest.AccountTransactionId,
            To = createTransactionRequest.To,
            Category = createTransactionRequest.Category,
            Tags = createTransactionRequest.Tags
        };
    }
}