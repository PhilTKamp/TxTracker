namespace TxTracker.TransactionsApi.Accounts;

public class AccountConflictException : Exception
{
    private Guid accountId { get; init; }
    public override string Message { get { return $"Conflict found for account with ID '{accountId}'"; } }

    public AccountConflictException(Guid accountId)
    {
        this.accountId = accountId;
    }
}

public class AccountNotFoundException : Exception
{
    private Guid accountId { get; init; }
    public override string Message { get { return $"Account with id '{accountId}' could not be found"; } }

    public AccountNotFoundException(Guid accountId)
    {
        this.accountId = accountId;
    }
}