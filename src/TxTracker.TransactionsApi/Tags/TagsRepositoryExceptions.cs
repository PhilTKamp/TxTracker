namespace TxTracker.TransactionsApi.Tags;

public class TagConflictException : Exception
{
    private Guid tagId { get; init; }
    public override string Message { get { return $"Conflict found for account with ID '{tagId}'"; } }

    public TagConflictException(Guid tagId)
    {
        this.tagId = tagId;
    }
}

public class TagNotFoundException : Exception
{
    private Guid tagId { get; init; }
    public override string Message { get { return $"Account with id '{tagId}' could not be found"; } }

    public TagNotFoundException(Guid tagId)
    {
        this.tagId = tagId;
    }
}