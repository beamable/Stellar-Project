using System;
using DG.Tweening;
using UnityEngine;

namespace Farm.Managers
{
    
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [Header("Singleton Settings")]
        [SerializeField] protected bool persistAcrossScenes;
        
        private static T _instance;
        private static int _instanceId;
        private static readonly object _lock = new object();
        private bool _initialized;
        private static bool _isQuitting;
        
        public bool IsInitialized => _instance != null;

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance != null) return _instance;
                    if (_isQuitting) return null;

                    var found = FindAnyObjectByType<T>(FindObjectsInactive.Include);
                    if (found != null)
                    {
                        Assign(found);
                        return _instance!;
                    }
                    
                    return null!; // Intentionally null when no instance and no auto-create.
                }
            }
        }
        
        /// <summary>
        /// Called exactly once for the winning instance—safe place for one-time init.
        /// </summary>
        protected virtual void OnInitOnce() { }
        /// <summary>
        /// Called after OnInitOnce on every domain play, useful for scene-bound wiring.
        /// </summary>

        protected virtual void OnAfterInitialized() { }
        
        protected virtual void OnDuplicateFound(T duplicate)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[MonoSingleton<{typeof(T).Name}>] Duplicate detected on '{duplicate.gameObject.name}'. Destroying the duplicate.");
#endif
            if (duplicate != null) Destroy(duplicate.gameObject);
        }
        
        /// <summary>
        /// Awake is sealed to ensure a consistent singleton assignment.
        /// Use OnInitOnce </summary>
        private void Awake()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    Assign(this as T);
                }
                else if (_instance != this)
                {
                    OnDuplicateFound(this as T);
                    return;
                }
            }

            // Only the winning instance reaches here; safe to use protected members.
            if (persistAcrossScenes)
                DontDestroyOnLoad(gameObject);

            if (!_initialized)
            {
                _initialized = true;
                try { OnInitOnce(); } catch (Exception ex) { Debug.LogException(ex, this); }
            }
        }

        private void Start()
        {
            try
            {
                OnAfterInitialized();
                _initialized = true;
            }
            catch (Exception ex) { Debug.LogException(ex, this); }
        }

        /// <summary> Assigns the winning instance, sets persistence, and caches ID. </summary>
        private static void Assign(T winner)
        {
            _instance = winner;
            _instanceId = winner.GetInstanceID();
        }
        
        protected virtual void OnApplicationQuit()
        {
            _isQuitting = true;
        }
        
        protected virtual void OnDestroy()
        {
            // Only clear if this object is the current singleton.
            if (_instanceId != GetInstanceID()) return;
            lock (_lock)
            {
                _instance = null;
            }
            _instanceId = -1;
            
            DOTween.KillAll();
        }
    }
}