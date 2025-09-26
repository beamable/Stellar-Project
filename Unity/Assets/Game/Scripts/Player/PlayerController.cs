using System;
using Farm.Input;
using Farm.UI;
using UnityEngine;

namespace Farm.Player
{
    public class PlayerController : MonoBehaviour
    {
        #region Variables

        [Header("Settings")]
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private SoPlayerInput playerInput;
        
        private bool _isWalking, _isBlocked, _canGoToSleep;
        private Vector2 _moveInput;
        
        private Rigidbody2D _rb2D;
        private PlayerAnimation _playerAnimation;
        private PlayerToolsController _playerToolsController;
        
        public bool IsBlocked => _isBlocked;
        
        #endregion
        
        #region Actions

        public static event Action OnSwitchTool;
        public static event Action<int> OnSelectSpecificTool;
        public static event Action OnGoToSleep;
        
        public static void RaiseSwitchTool() => OnSwitchTool?.Invoke();
        public static void RaiseSelectSpecificTool(int index) => OnSelectSpecificTool?.Invoke(index);

        #endregion

        #region Unity_Calls

        private void Awake()
        {
            _rb2D = GetComponent<Rigidbody2D>();
            _playerAnimation = GetComponent<PlayerAnimation>();
            _playerToolsController = GetComponent<PlayerToolsController>();
            _playerAnimation.InitAwake(this);
            _playerToolsController.InitAwake(this, _playerAnimation, playerInput);
        }

        private void Start()
        {
            _playerToolsController.InitStart();
        }

        private void OnEnable()
        {
            playerInput.SetGameplayInput();
            _playerToolsController.OnInitEnable();    
            
            playerInput.OnMoveEvent += OnMoveEvent;
            playerInput.OnInteractEvent += OnInteract;
            UiManager.OnPlayerAwoken += OnPlayerAwoken;
            UiManager.OnOpenUi += OnOpenUi;
        }
        
        private void OnDisable()
        {
            _playerToolsController.OnDeinit();
            
            playerInput.OnMoveEvent -= OnMoveEvent;
            playerInput.OnInteractEvent -= OnInteract;
            UiManager.OnPlayerAwoken -= OnPlayerAwoken;
            UiManager.OnOpenUi -= OnOpenUi;
        }

        private void FixedUpdate()
        {
            _rb2D.linearVelocity = _moveInput * moveSpeed;
        }

        #endregion
        
        public void SetSwitchingAreas(bool isSwitching)
        {
            _isBlocked = isSwitching;
            if (isSwitching)
            {
                OnMoveEvent(Vector2.zero);
            }
        }
        
        public void OnMoveEvent(Vector2 moveInput)
        {
            if (_playerToolsController.IsUsingTool || _isBlocked) //do not move while using tool
            {
                _moveInput = Vector2.zero;
                _isWalking = false;
                _playerAnimation.SetIsWalking(_isWalking);
                return;
            }
            _moveInput = moveInput;
            _isWalking = moveInput != Vector2.zero;
            FlipPlayer(moveInput.x);
            _playerAnimation.SetIsWalking(_isWalking);
        }
        
        public void SetCanGoToSleep(bool canGoToSleep)
        {
            _canGoToSleep = canGoToSleep;
        }

        private void FlipPlayer(float xValue)
        {
            if(xValue == 0) return;
            //keep xValue == 1 or -1
            var desired = Mathf.Sign(xValue);
            if (Mathf.Approximately(transform.localScale.x, desired)) return;

            transform.localScale = new Vector3(desired, 1f, 1f);
        }
        
        private void OnInteract()
        {
            if (!_canGoToSleep) return;
            
            OnGoToSleep?.Invoke();
            _isBlocked = true;
            _canGoToSleep = false;
        }
        
        private void OnPlayerAwoken()
        {
            _isBlocked = false;
        }
        
        private void OnOpenUi(bool inventoryOpen)
        {
            _isBlocked = inventoryOpen;
        }
        
    }
}
