using System.Threading.Tasks;
using Beamable.StellarFederation.Features.BlockProcessor.Handlers;

namespace Beamable.StellarFederation.Features.BlockProcessor;

public class NativeBlockProcessor(
    CreateAccountBlockHandler accountBlockHandler) : IService
{
    public async Task Process(uint fromBlock, uint toBlock)
    {
        await accountBlockHandler.Handle(fromBlock, toBlock);
    }
}