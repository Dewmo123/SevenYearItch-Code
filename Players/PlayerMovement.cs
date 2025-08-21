using AKH.Scripts.Select;
using DG.Tweening;
using Scripts.Players;
using System.Collections;
using System.Collections.Generic;
using _00Work.LKW.Code;
using Scripts.Core.Sound;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AKH.Scripts.Players
{
    struct DestInfo
    {
        public Vector2 pos;
        public Vector2 dir;
        public float distance;
    }
    public class PlayerMovement : MonoBehaviour, IPlayerComponent, ISelectableElem
    {
        private Player _player;
        private PlayerInputSO _playerInput;
        [SerializeField] private Tilemap map;
        [SerializeField] private LayerMask mapLayer;
        [SerializeField] private Transform visual;
        [SerializeField] private SoundSO jumpSound;
        private Queue<DestInfo> _destinations = new();
        private PlayerAnimator _animator;
        private DetectBreakableWall _detector;
        private static int _dashHash = Animator.StringToHash("Dash");
        private static int _idleHash = Animator.StringToHash("Idle");
        private Tween _moveTween; 
        public bool IsMoving { get; private set; }
        public Vector2 CurrentDir { get; private set; }
        public void Init(Player player)
        {
            _player = player;
            _detector = player.GetCompo<DetectBreakableWall>();
            _animator = _player.GetCompo<PlayerAnimator>();
            _playerInput = _player.PlayerInput;
            player.OnDead.AddListener(HandleDead);
        }

        private void HandleDead()
        {
            StopAllCoroutines();
            _moveTween.Kill(false);
        }

        private void HandleMoveEvent(Vector2 vector)
        {
            if (IsMoving || (Mathf.Abs(vector.y) < 0.9f && Mathf.Abs(vector.x) < 0.9f))
                return;
            SoundManager.Instance.PlaySFX(jumpSound, transform.position);
            GetDestination(vector);
        }
        private void GetDestination(Vector2 dir)
        {
            Vector2 origin = transform.position;
            Vector2 direction = dir;
            RaycastHit2D hit = new();
            for(int i=0;i<30;i++)
            {
                hit = Physics2D.Raycast(origin, direction, 100, mapLayer);
                if (hit)
                {
                    // 반사 방향 계산
                    Vector2 before = direction;
                    direction = Vector2.Reflect(direction, hit.normal);
                    float dot = Vector2.Dot(before, direction);
                    Vector2 tile = map.GetCellCenterWorld(map.WorldToCell(hit.point + hit.normal * 0.01f));
                    _destinations.Enqueue(new DestInfo() { distance = hit.distance, dir = direction, pos = tile });
                    if (Mathf.Approximately(Mathf.Abs(dot), 1f))
                        break;
                    // 새로운 출발점
                    origin = hit.point + hit.normal * 0.01f; // 겹침 방지용
                }
                else
                {
                    break;
                }
            }
            if (hit == default)
                return;
            StartCoroutine(MoveToDestination(hit));
        }
        private Vector3 _prevPos;
        private void Update()
        {
            Vector3 dir = (transform.position - _prevPos).normalized;
            if (dir == Vector3.zero)
                return;
            CurrentDir = dir;
            _prevPos = transform.position;
        }

        private IEnumerator MoveToDestination(RaycastHit2D hit)
        {
            IsMoving = true;
            _animator.SetAnimation(_dashHash);
            float distance = 0;
            bool isBreak = false;
            while (_destinations.Count > 0 && !_player.IsDead)
            {
                DestInfo info = _destinations.Dequeue();
                distance += info.distance;
                isBreak = _detector.DetectWall();
                visual.transform.up = ((Vector3)info.pos - transform.position).normalized;
                _moveTween = _player.transform.DOMove(info.pos, info.distance / 30)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        visual.transform.up = info.dir;
                        _player.OnLand?.Invoke(distance);
                    });
                yield return _moveTween.WaitForCompletion();
            }

            if (!isBreak && hit.collider.TryGetComponent(out Tilemap tilemap))
                _player.OnTileMapLand?.Invoke();
            _animator.SetAnimation(_idleHash);
            IsMoving = false;
        }
        public void Select()
        {
            _playerInput.OnMoveEvent += HandleMoveEvent;
        }

        public void DeSelect()
        {
            _playerInput.OnMoveEvent -= HandleMoveEvent;
        }
        private void OnDestroy()
        {
            DeSelect();
        }
        void OnDrawGizmosSelected()
        {
            Vector2 origin = transform.position;
            Vector2 direction = -transform.right;
            Gizmos.DrawRay(transform.position, CurrentDir);
            //for (int i = 0; i < 15; i++)
            //{
            //    RaycastHit2D hit = Physics2D.Raycast(origin, direction, 100, mapLayer);
            //    if (hit)
            //    {
            //        // 선 그리기 (디버그용)
            //        Gizmos.color = Color.red;
            //        Gizmos.DrawLine(origin, hit.point);

            //        // 반사 방향 계산
            //        direction = Vector2.Reflect(direction, hit.normal);

            //        // 새로운 출발점
            //        origin = hit.point + hit.normal * 0.01f; // 겹침 방지용 작은 offset
            //    }
            //    else
            //    {
            //        // 맞지 않으면 직선 끝까지 그리기
            //        Gizmos.DrawLine(origin, origin + direction * 100);
            //        break;
            //    }
            //}
        }
    }
}
