namespace TxTracker.TransactionsApi.Accounts;

public static class AccountExtensions
{
    public static Account ToAccount(this CreateAccountRequest createAccountRequest)
    {
        if (createAccountRequest.Id == null)
        {
            throw new ArgumentNullException(nameof(createAccountRequest.Id), "Account ID cannot be null");
        }

        return new Account
        {
            Id = (Guid)createAccountRequest.Id, // Check for null above
            Name = createAccountRequest.Name
        };
    }
}