using System.Threading.Tasks;
namespace Beamable.StellarFederation.Features.BlockProcessor;

public class ChainBlockProcessor(
    HorizonBlockProcessor horizonBlockProcessor,
    SorobanBlockProcessor sorobanBlockProcessor) : IService
{
    public async Task Handle()
    {
        var tasks = new[]
        {
            horizonBlockProcessor.Handle(),
            sorobanBlockProcessor.Handle()
        };
        await Task.WhenAll(tasks);
    }
}