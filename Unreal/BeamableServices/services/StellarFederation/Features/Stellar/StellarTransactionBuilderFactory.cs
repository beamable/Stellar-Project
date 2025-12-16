using System.Threading.Tasks;
using StellarDotnetSdk.Accounts;
using StellarDotnetSdk.Transactions;

namespace Beamable.StellarFederation.Features.Stellar;

public class StellarTransactionBuilderFactory : IService
{
    private readonly Configuration _configuration;

    public StellarTransactionBuilderFactory(Configuration configuration)
    {
        _configuration = configuration;
    }

    public async Task<TransactionBuilder> CreateDefaultBuilder(Account sourceAccount, string memo)
    {
        return new TransactionBuilder(sourceAccount)
            .Defaults(await _configuration.BaseFeeInStroops, await _configuration.TransactionTimeoutSec, memo);
    }
}