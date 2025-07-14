using Microsoft.EntityFrameworkCore;
using TxTracker.TransactionsApi.Accounts;
using TxTracker.TransactionsApi.Categories;
using TxTracker.TransactionsApi.Data.Models;

namespace TxTracker.TransactionsApi.Data;

public class TransactionsContext(DbContextOptions<TransactionsContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Tag> Tags { get; set; }
}