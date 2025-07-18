namespace TxTracker.TransactionsApi.Categories;

public static class CategoryExtensions
{
    public static Category ToCategory(this CreateCategoryRequest createCategoryRequest)
    {
        if (createCategoryRequest.Id == null)
        {
            throw new ArgumentNullException(nameof(createCategoryRequest.Id), "Account ID cannot be null");
        }

        return new Category
        {
            Id = (Guid)createCategoryRequest.Id, // Check for null above
            Name = createCategoryRequest.Name
        };
    }
}