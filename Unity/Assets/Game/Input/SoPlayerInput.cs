using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Farm.Input
{
    [CreateAssetMenu(fileName = "PlayerInput", menuName = "Farm/SOs/PlayerInput")]
    public class SoPlayerInput : ScriptableObject, InputSystem_Actions.IPlayerActions
    {
        public event Action<Vector2> OnMoveEvent;
        public event Action OnUseToolEvent;
        public event Action OnInteractEvent;
        public event Action OnNextToolEvent;
        public event Action OnOpenInventoryEvent;
        public event Action OnEscapeKeyEvent;
        public event Action<int> OnSelectSpecificToolEvent;

        private InputSystem_Actions _inputActions;

        private void OnEnable()
        {
            if (_inputActions != null) return;
            
            _inputActions = new InputSystem_Actions();
            _inputActions.Player.SetCallbacks(this);
            SetGameplayInput();
        }

        private void OnDisable()
        {
            DisableAllInputs();
        }

        public void SetGameplayInput()
        {
            _inputActions.Player.Enable();
        }
        
        public void SetUiInput()
        {
            _inputActions.UI.Disable();
        }
        
        private void DisableAllInputs()
        {
            _inputActions.Player.Disable();
        }

        #region Player Inputs

        public void OnMove(InputAction.CallbackContext context)
        {
            OnMoveEvent?.Invoke(context.ReadValue<Vector2>().normalized);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
        }

        public void OnUseTool(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnUseToolEvent?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnInteractEvent?.Invoke();
        }

        public void OnOpenInventory(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnOpenInventoryEvent?.Invoke();
        }

        public void OnPauseGame(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnEscapeKeyEvent?.Invoke();
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
        }

        public void OnJump(InputAction.CallbackContext context)
        {
        }

        public void OnKey01(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnSelectSpecificToolEvent?.Invoke(0);
        }

        public void OnKey02(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnSelectSpecificToolEvent?.Invoke(1);
        }

        public void OnKey03(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnSelectSpecificToolEvent?.Invoke(2);
        }

        public void OnKey04(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnSelectSpecificToolEvent?.Invoke(3);
        }

        public void OnKey05(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnSelectSpecificToolEvent?.Invoke(4);
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
        }

        public void OnNext(InputAction.CallbackContext context)
        {
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
        }

        public void OnSwitchTool(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnNextToolEvent?.Invoke();
        }

        #endregion
    }
}
