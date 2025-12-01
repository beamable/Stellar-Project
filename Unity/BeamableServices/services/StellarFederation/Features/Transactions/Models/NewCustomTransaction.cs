namespace Beamable.StellarFederation.Features.Transactions.Models;

public record NewCustomTransaction(long GamerTag, string WalletAddress, string OperationName, string ConcurrencyKey);