namespace TxTracker.TransactionsApi.Categories;

public class CategoryConflictException : Exception
{
    private Guid categoryId { get; init; }
    public override string Message { get { return $"Conflict found for category with ID '{categoryId}'"; } }

    public CategoryConflictException(Guid categoryId)
    {
        this.categoryId = categoryId;
    }
}

public class CategoryNotFoundException : Exception
{
    private Guid categoryId { get; init; }
    public override string Message { get { return $"Category with id '{categoryId}' could not be found"; } }

    public CategoryNotFoundException(Guid categoryId)
    {
        this.categoryId = categoryId;
    }
}