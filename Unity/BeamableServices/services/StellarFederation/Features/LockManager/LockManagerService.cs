using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.StellarFederation.Features.LockManager.Storage;
using Beamable.StellarFederation.Features.Transactions;

namespace Beamable.StellarFederation.Features.LockManager;

public class LockManagerService : IService
{
    private readonly LockCollection _lockCollection;

    public LockManagerService(LockCollection lockCollection)
    {
        _lockCollection = lockCollection;
    }

    public async Task<bool> AcquireLock(string lockName, int lockTimeoutSeconds = 10 * 60)
    {
        return await _lockCollection.AcquireLock(lockName, TransactionManager.InstanceId, lockTimeoutSeconds);
    }

    public async Task ReleaseLock(string lockName)
    {
        await _lockCollection.ReleaseLock(lockName);
    }

    public async Task ReleaseLock(IEnumerable<string> lockNames)
    {
        await _lockCollection.ReleaseLock(lockNames);
    }

    public async Task<List<string>> GetLocked()
    {
        return await _lockCollection.GetLocked();
    }
}