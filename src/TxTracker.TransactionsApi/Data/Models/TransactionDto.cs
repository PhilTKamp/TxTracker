namespace TxTracker.TransactionsApi.Data.Models;

public class TransactionDto
{
    public int? Id { get; set; }
    public double Amount { get; set; }
    public Account FromAccount { get; set; }
    public Account ToAccount { get; set; }
    public DateTime Date { get; set; }

}