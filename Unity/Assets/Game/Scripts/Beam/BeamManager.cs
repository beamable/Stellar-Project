using System;
using System.Collections.Generic;
using System.Threading;
using Beamable;
using Beamable.Server.Clients;
using Cysharp.Threading.Tasks;
using Farm.Managers;
using UnityEngine;

namespace Farm.Beam
{
    public class BeamManager : MonoSingleton<BeamManager>
    {
        #region Variables

        [Header("Managers (init in this exact order)")]
        [SerializeField] private List<BeamManagerBase> beamManagers = new();
        
        private CancellationTokenSource _cts;
        private readonly List<IBeamManager> _managers = new();
        
        public static bool IsReady { get; private set; }
        public static BeamContext BeamContext { get; private set; }
        public static StellarFederationClient StellarClient { get; private set; }
        public IReadOnlyList<IBeamManager> Managers => _managers;
        
        public event Action OnInitialized;
        public event Action OnResetStarted;
        public event Action OnResetCompleted;

        #endregion

        #region Managers

        public BeamAccountManager AccountManager { get; private set; }

        #endregion

        protected override void OnInitOnce()
        {
            base.OnInitOnce();
            DontDestroyOnLoad(this);
            RebuildManagerCacheFromList();

            AccountManager = GetComponentInChildren<BeamAccountManager>();
        }

        protected override void OnAfterInitialized()
        {
            base.OnAfterInitialized();
            InitAllAsync().Forget();
        }
        
        private void RebuildManagerCacheFromList()
        {
            _managers.Clear();
            foreach (var manager in beamManagers)
            {
                if (manager == null) continue;
                _managers.Add(manager);
                Debug.Log($"Adding manager '{manager.name}' to cache.");
            }
        }
        
        public async UniTask InitAllAsync(CancellationToken externalCt = default)
        {
            if (IsReady) return;

            _cts?.Cancel();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(externalCt);
            var ct = _cts.Token;

            await Init();

            // Forward order == exact Inspector order
            foreach (var m in _managers)
            {
                ct.ThrowIfCancellationRequested();
                await m.InitAsync(ct);
                Debug.Log($"Manager '{m.GetType().Name}' initialized.");
            }

            IsReady = true;
            Debug.Log($"{BeamContext}");
            OnInitialized?.Invoke();
        }

        public async UniTask ResetAllAsync(CancellationToken externalCt = default)
        {
            OnResetStarted?.Invoke();

            _cts?.Cancel();
            using var localCts = CancellationTokenSource.CreateLinkedTokenSource(externalCt);
            var ct = localCts.Token;

            // Reverse order for safe teardown
            for (int i = _managers.Count - 1; i >= 0; i--)
            {
                ct.ThrowIfCancellationRequested();
                try { await _managers[i].ResetAsync(ct); }
                catch (Exception e) { Debug.LogException(e); /* best-effort cleanup */ }
            }

            IsReady = false;
            OnResetCompleted?.Invoke();
        }

        public async UniTask ReinitializeAsync(CancellationToken ct = default)
        {
            await ResetAllAsync(ct);
            await InitAllAsync(ct);
        }
        
        private async UniTask Init()
        {
            BeamContext ??= BeamContext.Default;
            await BeamContext.OnReady;
            StellarClient ??= BeamContext.Microservices.StellarFederation();
        }
    }
}