using Beamable.Common;
using Beamable.Server;
using Beamable.StellarFederation.Features.BlockProcessor;
using Beamable.StellarFederation.Features.Scheduler;
using StellarFederationCommon.Models.Response;

namespace Beamable.StellarFederation;

public partial class StellarFederation
{
    [AdminOnlyCallable, SwaggerCategory("Scheduler")]
    public async Promise<SchedulerJobResponse> Jobs(bool enable)
    {
        if (enable)
            return await Provider.GetService<SchedulerService>().Start();
        return await Provider.GetService<SchedulerService>().End();
    }

    [ServerCallable, SwaggerCategory("Scheduler")]
    public async Promise BlockProcessor()
    {
        await Provider.GetService<ChainBlockProcessor>().Handle();
    }
}