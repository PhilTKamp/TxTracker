namespace TxTracker.TransactionsApi.Accounts;

public class Account
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}