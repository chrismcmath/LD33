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
        private float Gravity = 5f;

        protected bool _Collected;
        protected float _CollectedTime = 1f;
        protected Rigidbody2D _Rigidbody;
        protected ParticleSystem _ParticleSystem;

		private Vector2 _GravityDirection = Vector2.zero; 

        public void Awake() {
            _Rigidbody = GetComponent<Rigidbody2D>();
            _ParticleSystem = GetComponentInChildren<ParticleSystem>();
        }

        public void Update() {
            UpdateGravity();
        }

        public void FixedUpdate() {
            if (_Collected) {
                _CollectedTime -= Time.deltaTime * 0.00001f;
                if (_CollectedTime > 0f) {
                    Vector2 inverse = Game.Canvas.InverseTransformPoint(Game.PointsEnd.position);
                    Debug.Log("from " + transform.position + " to " +  Game.PointsEnd.position + " or " + inverse + " lastly ");
                    //transform.position = Vector2.Lerp(transform.position, Game.PointsEnd.position, _CollectedTime);
                    transform.position = Vector2.Lerp(transform.position, Game.CamController.GetComponent<UnityEngine.Camera>()
                            .ScreenToWorldPoint(
                                Game.Canvas.InverseTransformPoint(Game.PointsEnd.position)), _CollectedTime);
                } else {
                    //Destroy(gameObject);
                }
            }
        }

        private void UpdateGravity() {
            if (_Collected) return;
            _GravityDirection = GetGravityDirection();

            _Rigidbody.AddForce(_GravityDirection * Gravity, ForceMode2D.Force);
        }

        private Vector2 GetGravityDirection() {
            Debug.Log("HostPlanet: " + HostPlanet);
            if (HostPlanet == null) {
                return Vector2.zero;
            }

            Vector2 toPlanet = (Vector2) (HostPlanet.transform.position - transform.position);

            return toPlanet;
        }

        public void Collect() {
            Debug.Log("Collect coin");
            _ParticleSystem.Play();
            _Collected = true;
            _Rigidbody.Sleep();
        }
    }

}
