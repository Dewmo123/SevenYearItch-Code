using DewmoLib.Dependencies;
using Scripts.Players;
using System;
using Unity.Cinemachine;
using UnityEngine;

namespace AKH.Scripts.Players
{
    public class PlayerCameraPivot : MonoBehaviour
    {
        [Inject] private SwapManager _swapManager;
        [SerializeField] private PlayerInputSO playerInput;
        private bool _isClicked;
        private void Awake()
        {
            playerInput.OnMiddleEvent += HandleMiddleClick;
        }

        private void HandleMiddleClick(bool val)
        {
            _isClicked = val;
        }

        private void LateUpdate()
        {
            Vector3 pos = new Vector3(0,transform.position.y);
            if (_isClicked)
                pos.y += playerInput.MouseDelta.y*0.12f;
            else
                pos.y = _swapManager.CurrentObj.transform.position.y;
            transform.position = pos;
        }
    }
}
