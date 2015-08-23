using UnityEngine;
using System.Collections;

using Monster.Behaviours;
using Monster.Camera;
using Monster.Planets;
using Monster.Utils;

namespace Monster.Entities {
    public class CoinController : EntityController {
        public float GroundSpeed = 0f;
        public float AirSpeed = 10f;
        public float JumpSpeed = 20f;
        public float JumpForwardSpeed = 2f;
        public float BoostSpeed = 100f;
        public float MaxHorizontalGroundSpeed = 0.5f;
        public float MaxHorizontalAirSpeed = 0.6f;
        public float Gravity = 10f;

        protected Rigidbody2D _Rigidbody;
        protected ParticleSystem _ParticleSystem;

		private Vector2 _GravityDirection = Vector2.zero; 

        public void Awake() {
            _Rigidbody = GetComponent<Rigidbody2D>();
        }

        public void Update() {
            UpdateGravity();
        }

        private void UpdateGravity() {
            _GravityDirection = GetGravityDirection();

                _Rigidbody.AddForce(_GravityDirection * Gravity, ForceMode2D.Force);
        }

        private Vector2 GetGravityDirection() {
            if (HostPlanet == null) {
                return Vector2.zero;
            }

            Vector2 toPlanet = (Vector2) (HostPlanet.transform.position - transform.position);

            return toPlanet;
        }

        public void Collect() {
            Debug.Log("Collect coin");
        }
    }
}
