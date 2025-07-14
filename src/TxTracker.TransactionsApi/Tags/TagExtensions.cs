namespace TxTracker.TransactionsApi.Tags;

public static class TagExtensions
{
    public static Tag ToTag(this CreateTagRequest createTagRequest)
    {
        if (createTagRequest.Id == null)
        {
            throw new ArgumentNullException(nameof(createTagRequest.Id), "Tag ID cannot be null");
        }

        return new Tag
        {
            Id = (Guid)createTagRequest.Id, // Check for null above
            Name = createTagRequest.Name
        };
    }
}