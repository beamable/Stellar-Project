using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Server;
using Beamable.StellarFederation.Features.BlockProcessor;
using Beamable.StellarFederation.Features.Scheduler;

namespace Beamable.StellarFederation;

public partial class StellarFederation
{
    [AdminOnlyCallable, SwaggerCategory("Scheduler")]
    public async Task Jobs(bool enable)
    {
        if (enable)
            await Provider.GetService<SchedulerService>().Start();
        await Provider.GetService<SchedulerService>().End();
    }

    [ServerCallable, SwaggerCategory("Scheduler")]
    public async Promise BlockProcessor()
    {
        await Provider.GetService<ChainBlockProcessor>().Start();
    }
}