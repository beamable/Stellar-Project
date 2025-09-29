using System.Threading;
using Cysharp.Threading.Tasks;
using Farm.Managers;
using UnityEngine;

namespace Farm.Beam
{
    public interface IBeamManager
    {
        UniTask InitAsync(CancellationToken ct);
        UniTask ResetAsync(CancellationToken ct);
    }
    
    public abstract class BeamManagerBase<T> : MonoBehaviour, IBeamManager
        where T : BeamManagerBase<T>
    {
        public abstract UniTask InitAsync(CancellationToken ct);
        public virtual UniTask ResetAsync(CancellationToken ct) => UniTask.CompletedTask;
    }
}