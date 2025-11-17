using System.Threading;
using Beamable;
using Beamable.Server.Clients;
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
    
    public abstract class BeamManagerBase : MonoBehaviour, IBeamManager
    {
        protected BeamContext _beamContext;
        protected StellarFederationClient _stellarClient;
        protected bool IsReady { get; private set; }

        public virtual async UniTask InitAsync(CancellationToken ct)
        {
            _beamContext = BeamManager.BeamContext;
            _stellarClient = BeamManager.StellarClient;
            await UniTask.Yield();
            IsReady = true;
        }
        
        public virtual async UniTask ResetAsync(CancellationToken ct)
        { 
            IsReady = false;
            await UniTask.Yield();
        }
    }
}