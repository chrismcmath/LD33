using UnityEngine;
using System.Collections;

using Monster.Utils;

namespace Monster.Behaviours {
    public class HumanHitBehaviour : HumanBehaviour {
        public float GroundSpeed = 0f;
        public float AirSpeed = 10f;
        public float JumpSpeed = 20f;
        public float JumpForwardSpeed = 2f;
        public float BoostSpeed = 100f;
        public float MaxHorizontalGroundSpeed = 0.5f;
        public float MaxHorizontalAirSpeed = 0.6f;
        public float Gravity = 1f;

		private Vector2 _GravityDirection = Vector2.zero; 
        private Quaternion _TargetRotation;

		protected override void OnSwitchIn() {
            _Rigidbody.constraints = RigidbodyConstraints2D.None;
            _Rigidbody.angularVelocity = 50f;
            _ParticleSystem.Play();
		}

        protected override void UpdateRigidbody() {
            UpdateGravity();
        }

		protected override void OnAction2Down() {
		}

        protected override void UpdateContinuousInput() {
		}

        private void UpdateGravity() {
            _GravityDirection = GetGravityDirection();

            _Rigidbody.AddForce(_GravityDirection * Gravity, ForceMode2D.Force);
            _TargetRotation = Quaternion.Euler(new Vector3(0f, 0f, VectorUtils.GetAngle(Vector2.down, _GravityDirection)));
        }

        private Vector2 GetGravityDirection() {
            if (_HumanController.HostPlanet == null) {
                return Vector2.zero;
            }

            Vector2 toPlanet = (Vector2) (_HumanController.HostPlanet.transform.position - transform.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlanet, Mathf.Infinity, _HumanController.PlanetLayerMask);
            if (hit.collider != null) {
                //Debug.Log("hit: " + hit.collider.name);
                _HumanController.GroundPoint = hit.point;
                return -1 * hit.normal;
            } else {
                _HumanController.GroundPoint = Vector2.zero;
            }
            return toPlanet;
        }

        public void OnCollisionEnter2D(Collision2D col) {
            Debug.Log("human OnCollisionEnter2D: " + col.collider.name);
            if (col.collider.gameObject.layer == LayerMask.NameToLayer("Planet")) {
                if (_HumanController == null) {
                    Destroy(col.collider.gameObject);
                } else {
                    _HumanController.RemoveMonsterBehaviour();
                    _HumanController.Behaviour = _HumanController.gameObject.AddComponent<IdleHumanBehaviour>();
                }
            }
        }
    }
}
