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
        private float Gravity = 1f;

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
                _CollectedTime -= Time.deltaTime * 2f;
                if (_CollectedTime > 0f) {
                    /*
                    Debug.Log("end pos: " + Game.PointsEnd.position);
                    //Vector2 screenPos = new Vector2(Game.PointsEnd.position.x / Game.PointsEnd:w
                    //Debug.Log("screen pos: " + Game.PointsEnd.GetWorldCorners());
                    Vector2 worldPos = Game.CamController.GetComponent<UnityEngine.Camera>().ScreenToWorldPoint(Game.PointsEnd.position);
                    //Debug.Log("world pos: " + screenPos);
                    Vector2 inverse = Game.Canvas.InverseTransformPoint(Game.PointsEnd.position);
                    //Debug.Log("from " + transform.position + " to " +  Game.PointsEnd.position + " or " + inverse + " lastly ");
                    //transform.position = Vector2.Lerp(transform.position, Game.PointsEnd.position, _CollectedTime);
                    transform.position = Vector2.Lerp(transform.position, Game.CamController.GetComponent<UnityEngine.Camera>()
                            .ScreenToWorldPoint(
                                Game.Canvas.InverseTransformPoint(Game.PointsEnd.position)), _CollectedTime);
                                */
                } else {
                    Destroy(gameObject);
                }
            }
        }

        private void UpdateGravity() {
            if (_Collected) return;
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
            _ParticleSystem.startLifetime = 10f;
            _ParticleSystem.emissionRate = 1f;
            _ParticleSystem.startSpeed = 10f;
            _Collected = true;
            _Rigidbody.Sleep();
            foreach (BoxCollider2D col in GetComponentsInChildren<BoxCollider2D>()) {
                Destroy(col);
            }
            Destroy(GetComponentInChildren<MeshRenderer>());
        }
    }

}
