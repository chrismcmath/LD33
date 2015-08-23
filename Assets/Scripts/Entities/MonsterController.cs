using UnityEngine;
using System.Collections;

using Monster.Planets;
using Monster.Utils;

namespace Monster.Entities {
    public class MonsterController : MonoBehaviour {
        public GameController Game;

        public float Speed = 10f;
        public float RotationSpeed = 10f;
        public float Gravity = 500f;
        public LayerMask PlanetLayerMask;

        private PlanetController _HostPlanet;
        private Rigidbody2D _Rigidbody;
        private Quaternion _TargetRotation;

        private Vector2 DEBUG_HitPosition = Vector2.zero;
        private Vector2 DEBUG_HitNormal = Vector2.zero;
        
        public void Awake() {
            _Rigidbody = GetComponent<Rigidbody2D>();
            _Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        public void Update() {
            _HostPlanet = Game.GetClosestPlanet((Vector2) transform.position);
            UpdateRotation();
        }

        public void FixedUpdate() {
            Vector2 right = transform.localRotation * Vector2.right;
            
            Vector2 moveVector = ControllerUtils.GetHorizontalMovement() * Speed * Time.deltaTime * right;
            _Rigidbody.AddForce(moveVector, ForceMode2D.Force);

            UpdateGravity();
        }

        public float GetAngle(Vector2 a, Vector2 b) {
            float angle = Vector2.Angle(a, b);
            Vector3 cross = Vector3.Cross(a, b);
            if (cross.z < 0f) {
                angle = -angle;
            }
            return angle;
        }

        private void UpdateGravity() {
            Vector2 gravityDirection = GetGravityDirection();
            //TODO: Gravity is constant when on the planet
            //float mag = (_HostPlanet.transform.position - transform.position).magnitude;
            //float gravitationalForce = 

            _Rigidbody.AddForce(gravityDirection * Time.deltaTime * Gravity, ForceMode2D.Force);
            _TargetRotation = Quaternion.Euler(new Vector3(0f, 0f, GetAngle(Vector2.down, gravityDirection)));
        }

        private Vector2 GetGravityDirection() {
            if (_HostPlanet == null) {
                return Vector2.zero;
            }

            Vector2 toPlanet = (Vector2) (_HostPlanet.transform.position - transform.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlanet, Mathf.Infinity, PlanetLayerMask);
            if (hit.collider != null) {
                //Debug.Log("hit: " + hit.collider.name);
                DEBUG_HitPosition = hit.point;
                DEBUG_HitNormal = hit.normal;
                return -1 * hit.normal;
            }
            return toPlanet;
        }

        private void UpdateRotation() {
            transform.rotation = Quaternion.Slerp(transform.rotation, _TargetRotation, Time.deltaTime * RotationSpeed);
        }

        public void OnDrawGizmos() {
            Vector2 normalEnd = DEBUG_HitPosition + DEBUG_HitNormal; 

            Gizmos.color = Color.red;
            DebugUtils.DrawBounds(new Bounds(DEBUG_HitPosition, Vector2.one));
            Gizmos.color = Color.cyan;
            DebugUtils.DrawBounds(new Bounds(normalEnd, Vector2.one));

            Debug.DrawLine(DEBUG_HitPosition, normalEnd, Color.white);
        }
    }
}
