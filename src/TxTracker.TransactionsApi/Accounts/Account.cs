namespace TxTracker.TransactionsApi.Data.Models;

public class Account
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}