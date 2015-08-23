using UnityEngine;
using System.Collections;

using Monster.Entities;
using Monster.Utils;

namespace Monster.Behaviours {
    public abstract class HumanBehaviour : MonsterBehaviour {
        protected HumanController _HumanController;
        protected BoxCollider2D _HumanCollider;

        protected override bool SetupError() {
            bool hasError = !HasRigidbody() || !HasHitCollider() || !HasHumanController();
            /*
            Debug.Log("r: " + HasRigidbody());
            Debug.Log("h: " + HasHitCollider());
            Debug.Log("c: " + HasHumanController());
            */
            if (hasError) {
                Debug.LogError("[HumanBehaviour] Could not load necessary controllers");
            }
            return hasError;
        }

        protected override bool TryGetHitCollider() {
            _HumanCollider = GetComponentInChildren<BoxCollider2D>();
            return _HumanCollider != null;
        }

        protected bool HasHumanController() {
            if (_HumanController != null) {
                return true;
            }

            return TryGetHumanController();
        }

        protected bool TryGetHumanController() {
            _HumanController = GetComponent<HumanController>();
            return _HumanController != null;
        }
    }
}
