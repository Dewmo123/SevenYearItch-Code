using DewmoLib.Dependencies;
using UnityEngine;
using UnityEngine.UI;

namespace AKH.Scripts.Players
{
    public class ProfileUI : MonoBehaviour
    {
        [Inject] private SwapManager _swapManager;
        [SerializeField] private Image image;
        private void Update()
        {
            image.sprite = _swapManager.CurrentObj.profile;
        }
    }
}
