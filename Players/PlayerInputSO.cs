using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Players
{
    [CreateAssetMenu(fileName = "PlayerInput", menuName = "SO/PlayerInput", order = 0)]
    public class PlayerInputSO : ScriptableObject, InputSystem_Actions.IPlayerActions, InputSystem_Actions.IUIActions
    {
        public event Action<Vector2> OnMoveEvent;
        public event Action OnSwapEvent;
        public event Action OnESCEvent;
        public event Action<bool> OnMiddleEvent;
        public Vector2 MouseDelta { get; private set; }
        private InputSystem_Actions _controls;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new InputSystem_Actions();
                _controls.Player.SetCallbacks(this);
                _controls.UI.SetCallbacks(this);
            }
            _controls.Player.Enable();
            _controls.UI.Enable();
        }

        private void OnDisable()
        {
            _controls.Player.Disable();
            _controls.UI.Disable();
        }

        public void SetInput(bool isActive)
        {
            if (isActive)
            {
                _controls.Enable();
            }
            else
            {
                _controls.Player.Disable();
            }
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnSwap(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnSwapEvent?.Invoke();
        }
        
        public void OnESC(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnESCEvent?.Invoke();
        }
        public void OnMiddleClick(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnMiddleEvent?.Invoke(true);
            else if(context.canceled)
                OnMiddleEvent?.Invoke(false);

        }
        #region UnUseRegion
        public void OnNavigate(InputAction.CallbackContext context)
        {
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
        }


        public void OnPoint(InputAction.CallbackContext context)
        {
        }
        public void OnClick(InputAction.CallbackContext context)
        {
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
        }



        public void OnScrollWheel(InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {
        }

        public void OnMouseDelta(InputAction.CallbackContext context)
        {
            MouseDelta = context.ReadValue<Vector2>();
        }

        #endregion

    }
}
