using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.VisualBasic;
using TxTracker.TransactionsApi.Data.Models;

namespace TxTracker.TransactionsApi.Accounts;

[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    private readonly AccountsRepository accountsRepository;

    private readonly ILogger<AccountsController>? logger;

    public AccountsController(AccountsRepository accountsRepository)
    {
        this.accountsRepository = accountsRepository;
    }

    public AccountsController(AccountsRepository accountsRepository, ILogger<AccountsController> logger)
    {
        this.accountsRepository = accountsRepository;
        this.logger = logger;
    }

    [HttpGet(Name = "GetAccounts")]
    public async Task<ActionResult<IEnumerable<Account>>> GetAsync([FromQuery] int page = 0, [FromQuery] int pageSize = -1, CancellationToken cancellationToken = default)
    {
        if (pageSize == -1)
        {
            var accounts = await accountsRepository.GetAccountsAsync(cancellationToken);
            return Ok(accounts);
        }
        else
        {
            var accounts = await accountsRepository.GetAccountsAsync(page, pageSize, cancellationToken);
            return Ok(accounts);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Account>> GetAccount(string id, CancellationToken cancellationToken = default)
    {
        var validGuid = Guid.TryParse(id, out var guid);

        if (!validGuid)
        {
            return BadRequest();
        }

        try
        {
            var account = await accountsRepository.GetAsync(guid, cancellationToken);
            if (account is null)
            {
                return NotFound();
            }
            return Ok(account);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Unhandled exception occurred retrieving account by id '{accountId}'", id);
            return Problem("Error occurred when retrieving the account.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Account>> CreateAccount([FromBody] CreateAccountRequest req, CancellationToken cancellationToken = default)
    {
        if (req.Id == null)
        {
            try
            {
                var account = await accountsRepository.CreateAsync(req.Name, cancellationToken);
                logger?.LogInformation("Account created. Id {accountId}, Name {accountName}", account.Id, account.Name);
                return Accepted(account);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when creating account. Create account request: Id={accountId}, Name={accountName}", req.Id, req.Name);
                return Problem("Error occurred when creating the account");
            }
        }
        else
        {
            try
            {
                var account = await accountsRepository.CreateAsync(req.ToAccount());
                logger?.LogInformation("Account created. Id {accountId}, Name {accountName}", account.Id, account.Name);
                return CreatedAtAction(nameof(GetAccount), account);
            }
            catch (AccountConflictException)
            {
                logger?.LogInformation("Conflicting account found on create request. Conflicting ID '{accountId}'", req.Id);
                return Conflict(new { message = $"Account with ID '{req.Id}' already exists." });
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when creating account. Create account request: Id={accountId}, Name={accountName}", req.Id, req.Name);
                return Problem("Error occurred when creating the account");
            }
        }
    }

    [HttpPut]
    public async Task<ActionResult<Account>> UpdateAccount([FromBody] Account account, CancellationToken cancellationToken = default)
    {
        if (accountsRepository.Exists(account.Id))
        {
            try
            {
                var acc = await accountsRepository.UpdateAsync(account, cancellationToken);

                return Ok(acc);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when creating account. Update account request: Id={accountId}, Name={accountName}", account.Id, account.Name);
                return Problem("Error occurred when attempting to create the account.");
            }

        }
        else
        {
            try
            {
                var acc = await accountsRepository.CreateAsync(account, cancellationToken);

                return CreatedAtAction(nameof(GetAccount), acc);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when updating an account. Update account request: Id={accountId}, Name={accountName}", account.Id, account.Name);
                return Problem("Error occurred when attempting to update the account.");
            }
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAccount(string id, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(id, out var accGuid))
        {
            return BadRequest();
        }

        if (accountsRepository.Exists(accGuid))
        {
            try
            {
                await accountsRepository.DeleteAsync(accGuid, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when deleting an account. Delete account request: Id={accountId}", id);
                return Problem("Error occurred when attempting to delete the account.");
            }
        }
        else
        {
            return NotFound();
        }
    }
}
