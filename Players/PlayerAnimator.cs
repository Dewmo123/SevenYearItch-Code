using UnityEngine;

namespace AKH.Scripts.Players
{
    public class PlayerAnimator : MonoBehaviour, IPlayerComponent
    {
        private Animator _animator;
        private int _beforehash;
        public void Init(Player player)
        {
            _animator = GetComponent<Animator>();
            SetAnimation(Animator.StringToHash("Idle"));
            player.OnDead.AddListener(DeadAnimation);
        }
        
        public void SetAnimation(int hash)
        {
            _animator.SetBool(_beforehash, false);
            _animator.SetBool(hash, true);
            _beforehash = hash;
        }

        private void DeadAnimation() => _animator.Play("Dead");
    }
}
