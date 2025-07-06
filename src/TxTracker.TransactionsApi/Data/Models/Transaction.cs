namespace TxTracker.TransactionsApi.Data.Models;

public class Transaction
{
    public int Id { get; set; }
    public required double Amount { get; set; }
    public required DateTime Date { get; set; }
    public required Account From { get; set; }
    public required Account To { get; set; }
    public Category? Category { get; set; }
    public List<Tag> Tags { get; set; } = new List<Tag>();
}