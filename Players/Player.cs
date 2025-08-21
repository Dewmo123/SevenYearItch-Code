using AKH.Scripts.Select;
using Scripts.Players;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace AKH.Scripts.Players
{
    public class Player : MonoBehaviour, Select.ISelectable
    {
        private Dictionary<Type, IPlayerComponent> _compos;
        public Sprite profile;
        [field: SerializeField] public PlayerInputSO PlayerInput { get; private set; }
        private ISelectableElem[] _canSelectCompo;
        public UnityEvent OnDead;
        public UnityEvent<float> OnLand;
        public UnityEvent OnTileMapLand;
        public bool IsDead { get; private set; }
        private void Awake()
        {
            InitCompos();
        }

        private void InitCompos()
        {
            _compos = new();
            _canSelectCompo = GetComponentsInChildren<ISelectableElem>();
            IEnumerable<IPlayerComponent> compo = GetComponentsInChildren<IPlayerComponent>();
            foreach (var item in compo)
                _compos.Add(item.GetType(), item);
            foreach (var item in compo)
                item.Init(this);
        }

        public T GetCompo<T>() where T : IPlayerComponent
        {
            if (_compos.ContainsKey(typeof(T)))
                return (T)_compos[typeof(T)];
            return default;
        }

        public void Select()
        {
            gameObject.layer = LayerMask.NameToLayer("SelectPlayer");
            foreach (var item in _canSelectCompo)
            {
                item.Select();
            }
        }
        public void DeSelect()
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            foreach (var item in _canSelectCompo)
            {
                item.DeSelect();
            }
        }
        public void Dead()
        {
            if (IsDead)
                return;
            IsDead = true;
            OnDead?.Invoke();
        }
    }
}
