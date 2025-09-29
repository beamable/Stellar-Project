using System.Threading;
using Beamable.Player;
using Beamable.Server.Clients;
using Cysharp.Threading.Tasks;
using StellarFederationCommon;
using UnityEngine;

namespace Farm.Beam
{
    public class BeamAccountManager : BeamManagerBase
    {
        #region Unity Calls
        
        public PlayerAccount CurrentAccount { get; private set; }

        public override async UniTask InitAsync(CancellationToken ct)
        {
            await base.InitAsync(ct);
            await _beamContext.Accounts.OnReady;
            await UpdateCurrentAccount();
        }

        public override async UniTask ResetAsync(CancellationToken ct)
        {
            await base.ResetAsync(ct);
            await UniTask.Yield();
        }

        public async UniTask UpdateCurrentAccount(PlayerAccount account = null)
        {
            await _beamContext.Accounts.OnReady;
            CurrentAccount = account ?? _beamContext.Accounts.Current;
        }

        public async UniTask SwitchAccount(PlayerAccount newAccount)
        {
            await _beamContext.Accounts.SwitchToAccount(newAccount);
            await _beamContext.Accounts.AddDeviceId(newAccount);
            await UpdateCurrentAccount(newAccount);
        }

        public async UniTask SetAlias(string alias, PlayerAccount account = null)
        {
            var ac = CurrentAccount ?? account;
            await _beamContext.Accounts.SetAlias(alias);
            await UpdateCurrentAccount(ac);
        }

        public async UniTask CreateNewAccount(string alia = "")
        {
            var newAccount = await _beamContext.Accounts.CreateNewAccount();
            await SetAlias(alia, newAccount);
            await _beamContext.Accounts.AddExternalIdentity<StellarWeb3Identity, StellarFederationClient>("",
                (AsyncChallengeHandler) null, newAccount);
            await SwitchAccount(newAccount);
            
        }
        
        #endregion
        
    }
}