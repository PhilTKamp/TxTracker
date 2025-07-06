namespace TxTracker.TransactionsApi.Accounts;

public class CreateAccountRequest
{
    public Guid? Id { get; set; }
    public required string Name { get; set; }
}