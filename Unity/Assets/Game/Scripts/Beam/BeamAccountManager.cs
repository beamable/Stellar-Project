using System.Threading;
using Beamable.Player;
using Beamable.Server.Clients;
using Cysharp.Threading.Tasks;
using StellarFederationCommon;
using StellarFederationCommon.Models.Response;
using SuiFederationCommon.Models.Notifications;
using UnityEngine;

namespace Farm.Beam
{
    public class BeamAccountManager : BeamManagerBase
    {
        private string _chosenAlias;
        private AccountResponse _stellarAccount;
        
        public bool CreatingNewAccount {get; private set;}
        public PlayerAccount CurrentAccount { get; private set; }

        #region Init

        public override async UniTask InitAsync(CancellationToken ct)
        {
            await base.InitAsync(ct);
            await _beamContext.Accounts.OnReady;
            await UpdateCurrentAccount();
            
            _beamContext.Api.NotificationService.Subscribe(PlayerNotificationContext.CustodialAccountCreated, OnCustodialAccountCreated);
            IsReady = true;
        }

        public override async UniTask ResetAsync(CancellationToken ct)
        {
            await base.ResetAsync(ct);
            await UniTask.Yield();
        }

        private async UniTask UpdateCurrentAccount(PlayerAccount account = null)
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

        private async UniTask SetAlias(string alias, PlayerAccount account = null)
        {
            var ac = CurrentAccount ?? account;
            await _beamContext.Accounts.SetAlias(alias);
            await UpdateCurrentAccount(ac);
        }

        public async UniTask CreateNewAccount(string alias = "")
        {
            CreatingNewAccount = true;
            _chosenAlias = alias;
            _stellarAccount = null;
            _stellarAccount = await _stellarClient.CreateAccount();
        }
        
        private void OnCustodialAccountCreated(object obj)
        {
            OnCustodialAccountCreatedAsync(obj).Forget();
        }

        private async UniTask OnCustodialAccountCreatedAsync(object obj)
        {
            var newAccount = await _beamContext.Accounts.CreateNewAccount();
            await SetAlias(_chosenAlias, newAccount);
            await _beamContext.Accounts.AddExternalIdentity<StellarWeb3Identity, StellarFederationClient>("",
                (AsyncChallengeHandler) null, newAccount);
            await SwitchAccount(newAccount);
            CreatingNewAccount = false;
        }
        
        #endregion

        public (bool, string) HasStellarId(string stellarIdentity)
        {
            if(CurrentAccount == null || CurrentAccount.ExternalIdentities.Length < 1) return (false, "");

            foreach (var identity in CurrentAccount.ExternalIdentities)
            {
                if(identity.providerNamespace == stellarIdentity)
                    return (true, identity.userId);
            }

            return (false, "");
        }
        
        
    }
}