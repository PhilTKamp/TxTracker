using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace TxTracker.TransactionsApi.Transactions;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly TransactionsRepository transactionsRepository;

    private readonly ILogger<TransactionsController>? logger;

    public TransactionsController(TransactionsRepository transactionsRepository)
    {
        this.transactionsRepository = transactionsRepository;
    }

    public TransactionsController(TransactionsRepository transactionsRepository, ILogger<TransactionsController> logger)
    {
        this.transactionsRepository = transactionsRepository;
        this.logger = logger;
    }

    [HttpGet(Name = "GetTransactions")]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetAsync([FromQuery] int page = 0, [FromQuery] int pageSize = -1, CancellationToken cancellationToken = default)
    {
        if (pageSize == -1)
        {
            var transactions = await transactionsRepository.GetAsync(cancellationToken);
            return Ok(transactions);
        }
        else
        {
            var transactions = await transactionsRepository.GetAsync(page, pageSize, cancellationToken);
            return Ok(transactions);
        }
    }

    [HttpGet("{id}", Name = "GetTransaction")]
    public async Task<ActionResult<Transaction>> GetTransactionAsync(string id, CancellationToken cancellationToken = default)
    {
        var validGuid = Guid.TryParse(id, out var guid);

        if (!validGuid)
        {
            return BadRequest();
        }

        try
        {
            var transaction = await transactionsRepository.GetAsync(guid, cancellationToken);
            if (transaction is null)
            {
                return NotFound();
            }
            return Ok(transaction);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Unhandled exception occurred retrieving transaction by id '{transactionId}'", id);
            return Problem("Error occurred when retrieving the transaction.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Transaction>> CreateTransaction([FromBody] CreateTransactionRequest req, CancellationToken cancellationToken = default)
    {
        if (req.Id == null)
        {
            try
            {
                var transaction = await transactionsRepository.CreateAsync(req, cancellationToken);
                logger?.LogInformation("Transaction created. Id {transactionId}", transaction.Id);
                return Accepted(transaction);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when creating transaction. Create transaction request: Transaction={transactionId}", JsonSerializer.Serialize(req));
                return Problem("Error occurred when creating the transaction");
            }
        }
        else
        {
            try
            {
                var transaction = await transactionsRepository.CreateAsync(req.ToTransaction());
                logger?.LogInformation("Transaction created. Id = {transactionId}", transaction.Id);
                return CreatedAtAction(nameof(GetTransactionAsync), transaction);
            }
            catch (TransactionConflictException)
            {
                logger?.LogInformation("Conflicting transaction found on create request. Transaction = {transaction}", JsonSerializer.Serialize(req));
                return Conflict(new { message = $"Transaction with ID '{req.Id}' already exists." });
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when creating transaction. Create transaction request: {transaction}", JsonSerializer.Serialize(req));
                return Problem("Error occurred when creating the transaction");
            }
        }
    }

    [HttpPut]
    public async Task<ActionResult<Transaction>> UpdateTransaction([FromBody] Transaction transaction, CancellationToken cancellationToken = default)
    {
        if (transactionsRepository.Exists(transaction.Id))
        {
            try
            {
                var t = await transactionsRepository.UpdateAsync(transaction, cancellationToken);

                return Ok(t);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when creating transaction. Transaction Request = {transaction}", JsonSerializer.Serialize(transaction));
                return Problem("Error occurred when attempting to create the transaction.");
            }

        }
        else
        {
            try
            {
                var t = await transactionsRepository.CreateAsync(transaction, cancellationToken);

                return CreatedAtAction(nameof(GetAsync), t);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when updating an transaction. Transaction = {transaction}", JsonSerializer.Serialize(transaction));
                return Problem("Error occurred when attempting to update the transaction.");
            }
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTransaction(string id, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(id, out var transGuid))
        {
            return BadRequest();
        }

        if (transactionsRepository.Exists(transGuid))
        {
            try
            {
                await transactionsRepository.DeleteAsync(transGuid, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception occurred when deleting an transaction. Delete transaction request: Id={transactionId}", id);
                return Problem("Error occurred when attempting to delete the transaction.");
            }
        }
        else
        {
            return NotFound();
        }
    }
}
