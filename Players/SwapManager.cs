using _00Work.EJY.Scripts.Core;
using DewmoLib.Dependencies;
using DewmoLib.Utiles;
using Scripts.Players;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AKH.Scripts.Players
{
    [Provide]
    public class SwapManager : MonoBehaviour, IDependencyProvider
    {
        [SerializeField] private Player[] canSelects;
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private EventChannelSO stageChannel;
        public Player CurrentObj { get; private set; }
        public int ObjCount => canSelects.Length;
        public int CurrentCount { get; private set; }

        public bool canSwap = true;
        private void Awake()
        {
            foreach (var item in canSelects)
                item.OnDead.AddListener(HandlePlayerDead);
        }


        private void HandlePlayerDead()
        {
            var evt = GameEvents.FadeEvent;
            evt.isFadeIn = true;
            evt.onFadeEnd = () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                StageManager.Instance.heartCount = 0;
                StageManager.Instance.goalCount = 0;
            };
            stageChannel?.InvokeEvent(evt);
        }

        private void Start()
        {
            canSwap = true;
            CurrentCount = 0;
            CurrentObj = canSelects[CurrentCount];
            CurrentObj?.Select();
            playerInput.OnSwapEvent += HandleSwap;
        }
        private void OnDestroy()
        {
            playerInput.OnSwapEvent -= HandleSwap;
        }
        public void HandleSwap()
        {
            if (canSwap == false|| CurrentObj == null) return;
            if (CurrentObj.GetCompo<PlayerMovement>().IsMoving)
                return;
            CurrentObj?.DeSelect();
            CurrentCount = (CurrentCount + 1) % ObjCount;
            CurrentObj = canSelects[CurrentCount];
            CurrentObj?.Select();
        }

        public void FixCam()
        {
            CurrentObj?.DeSelect();
            CurrentCount = (CurrentCount + 1) % ObjCount;
            CurrentObj = canSelects[CurrentCount];
            CurrentObj?.Select();
        }
    }
}
