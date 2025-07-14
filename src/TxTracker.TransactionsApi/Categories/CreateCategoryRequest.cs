namespace TxTracker.TransactionsApi.Categories;

public class CreateCategoryRequest
{
    public Guid? Id { get; set; }
    public required string Name { get; set; }
}