using UnityEngine;
using System.Collections;

using Monster.Entities;
using Monster.Utils;

namespace Monster.Behaviours {
    public abstract class MonsterBehaviour : MonoBehaviour {
        protected MonsterController _Controller;

        protected Rigidbody2D _Rigidbody;
        protected PolygonCollider2D _HitCollider;
        protected BoxCollider2D _GroundCollider;

        //TODO: hacked in Start
        protected ParticleSystem _ParticleSystem;

        protected float _TimeActive = 0f;
        public float TimeActive {
            get { return _TimeActive; }
        }

        public virtual void OnSwitchOut() {}

        protected abstract void UpdateContinuousInput();
        protected abstract void UpdateRigidbody();

        protected virtual void OnAction1Down() {}
        protected virtual void OnAction1Up() {}
        protected virtual void OnAction2Down() {}
        protected virtual void OnAction2Up() {}

        protected virtual void OnSwitchIn() {}

        public void Start() {
            if (SetupError()) {
                return;
            }

            _ParticleSystem = GetComponentInChildren<ParticleSystem>();
            OnSwitchIn();
        }

        public void UpdateBehaviour() {
            if (SetupError()) {
                return;
            }

            _TimeActive += Time.deltaTime;

            UpdateInput();
        }

        public void FixedUpdateBehaviour() {
            if (SetupError()) {
                return;
            }

            UpdateRigidbody();
            UpdateContinuousInput();
        }

        protected virtual bool SetupError() {
            bool hasError = !HasRigidbody() || !HasHitCollider() || !HasGroundCollider() || !HasController();
            if (hasError) {
                Debug.LogError("[MonsterBehaviour] Could not load necessary controllers");
            }
            return hasError;
        }

        protected bool HasRigidbody() {
            if (_Rigidbody != null) {
                return true;
            }

            return TryGetRigidbody();
        }

        protected bool TryGetRigidbody() {
            _Rigidbody = GetComponent<Rigidbody2D>();
            return _Rigidbody != null;
        }

        protected bool HasController() {
            if (_Controller != null) {
                return true;
            }

            return TryGetController();
        }

        protected bool TryGetController() {
            _Controller = GetComponent<MonsterController>();
            return _Controller != null;
        }

        protected bool HasGroundCollider() {
            if (_GroundCollider != null) {
                return true;
            }

            return TryGetGroundCollider();
        }

        protected bool TryGetGroundCollider() {
            _GroundCollider = GetComponentInChildren<BoxCollider2D>();
            return _GroundCollider != null;
        }

        protected bool HasHitCollider() {
            if (_HitCollider != null) {
                return true;
            }

            return TryGetHitCollider();
        }

        protected virtual bool TryGetHitCollider() {
            _HitCollider = GetComponentInChildren<PolygonCollider2D>();
            return _HitCollider != null;
        }

        private void UpdateInput() {
            if (ControllerUtils.Action1Down()) {
                OnAction1Down();
            } else if (ControllerUtils.Action1Up()) {
                OnAction1Up();
            } else if (ControllerUtils.Action2Down()) {
                OnAction2Down();
            } else if (ControllerUtils.Action2Up()) {
                OnAction2Up();
            }
        }
    }
}
