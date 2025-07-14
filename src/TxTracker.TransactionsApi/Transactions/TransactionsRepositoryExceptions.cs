namespace TxTracker.TransactionsApi.Transactions;

public class TransactionConflictException : Exception
{
    private Guid transactionId { get; init; }
    public override string Message { get { return $"Conflict found for transaction with ID '{transactionId}'"; } }

    public TransactionConflictException(Guid transactionId)
    {
        this.transactionId = transactionId;
    }
}

public class TransactionNotFoundException : Exception
{
    private Guid transactionId { get; init; }
    public override string Message { get { return $"Transaction with id '{transactionId}' could not be found"; } }

    public TransactionNotFoundException(Guid transactionId)
    {
        this.transactionId = transactionId;
    }
}