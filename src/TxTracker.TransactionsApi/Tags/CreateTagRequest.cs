namespace TxTracker.TransactionsApi.Tags;

public class CreateTagRequest
{
    public Guid? Id { get; set; }
    public required string Name { get; set; }
}