using UnityEngine;
using System.Collections;

using Monster.Utils;

namespace Monster.Behaviours {
    public class IdleHumanBehaviour : HumanBehaviour {
        public float GroundSpeed = 0f;
        public float AirSpeed = 10f;
        public float JumpSpeed = 20f;
        public float JumpForwardSpeed = 2f;
        public float BoostSpeed = 100f;
        public float MaxHorizontalGroundSpeed = 0.5f;
        public float MaxHorizontalAirSpeed = 0.6f;
        public float AirGravity = 60;
        public float GroundGravity = 100f;

        public float JumpDisableTimeout = 0.1f;

        private Quaternion _TargetRotation;

        private bool _Grounded = false;
		private float _LastJumpTime = 0f; 
		private Vector2 _GravityDirection = Vector2.zero; 

        private Vector2 DEBUG_HitPosition = Vector2.zero;
        private Vector2 DEBUG_HitNormal = Vector2.zero;

		protected override void OnSwitchIn() {
            _Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
		}

        protected override void UpdateRigidbody() {
            UpdateGravity();
            UpdateRotation();
        }

        protected override void OnAction1Down() {
            Debug.Log("OnAction1Down");
			CheckGrounded();

            if (_Grounded) {
                //PerformJump(); 
			} else {
                //PerformBoost(); 
				//_AnimationController.Fall();
			}
		}
		
		protected override void OnAction2Down() {
		}

        protected override void UpdateContinuousInput() {
            CheckGrounded();
            UpdateDirection();
		}

        private void CheckGrounded() {
            if (!_Grounded &&
                    _TimeActive - _LastJumpTime > JumpDisableTimeout &&
                    ColliderUtils.IsIntersecting(_HumanCollider, "Planet")) {
                _Grounded = true;
                OnTouchGround();
            } else if (_Grounded && !ColliderUtils.IsIntersecting(_HumanCollider, "Planet")) {
                _Grounded = false;
            }
        }

        private void UpdateDirection() {
            Vector2 right = transform.localRotation * Vector2.right;
            float axis = ControllerUtils.GetHorizontalMovement();

            if (axis > 0f) {
                _HumanController.FacingVector = Vector2.right;
            } else if (axis < 0f) {
                _HumanController.FacingVector = -1 * Vector2.right;
            }

            bool facingRight = _HumanController.FacingVector.x == 1;

            float speed = _Grounded ?
                GroundSpeed :
                AirSpeed;
            float max = _Grounded ?
                MaxHorizontalGroundSpeed :
                MaxHorizontalAirSpeed;

            speed = Mathf.Min(speed, max);
            speed *= _HumanController.FacingVector.x;

            //_Rigidbody.AddForce(right * speed, ForceMode2D.Impulse);

            if (_Grounded) {
                //_AnimationController.Running(true);
                //_SoundController.Footstep();
            }
        }
	
        private void OnTouchGround() {
            //_AnimationController.Land();
            //_SoundController.Footstep();
        }

        private void PerformJump() {
            _Grounded = false;
            _LastJumpTime = _TimeActive;

            Vector3 force = -1f *_GravityDirection * JumpSpeed;
            _Rigidbody.AddForce(force, ForceMode2D.Impulse);

            Vector2 right = transform.localRotation * Vector2.right * _HumanController.FacingVector.x;
            _Rigidbody.AddForce(right * JumpForwardSpeed, ForceMode2D.Impulse);
        }

        private void PerformBoost() {
            Vector3 force = -1f *_GravityDirection * BoostSpeed;
            _Rigidbody.AddForce(force, ForceMode2D.Impulse);
        }

        private void UpdateRotation() {
            transform.rotation = Quaternion.Slerp(transform.rotation, _TargetRotation, Time.deltaTime * _HumanController.RotationSpeed);
        }

        private void UpdateGravity() {
            _GravityDirection = GetGravityDirection();

            //TODO: Gravity is constant when on the planet
            //float mag = (HostPlanet.transform.position - transform.position).magnitude;
            //float gravitationalForce = 

            if (_Grounded) {
                _Rigidbody.AddForce(_GravityDirection * GroundGravity, ForceMode2D.Force);
            } else {
                _Rigidbody.AddForce(_GravityDirection * AirGravity, ForceMode2D.Force);
            }
            _TargetRotation = Quaternion.Euler(new Vector3(0f, 0f, VectorUtils.GetAngle(Vector2.down, _GravityDirection)));
        }

        private Vector2 GetGravityDirection() {
            Debug.Log("GetGravityDirection, controller: " + _HumanController);
            if (_HumanController.HostPlanet == null) {
                return Vector2.zero;
            }

            Vector2 toPlanet = (Vector2) (_HumanController.HostPlanet.transform.position - transform.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlanet, Mathf.Infinity, _HumanController.PlanetLayerMask);
            if (hit.collider != null) {
                //Debug.Log("hit: " + hit.collider.name);
                _HumanController.GroundPoint = hit.point;
                DEBUG_HitPosition = hit.point;
                DEBUG_HitNormal = hit.normal;
                return -1 * hit.normal;
            } else {
                _HumanController.GroundPoint = Vector2.zero;
            }
            return toPlanet;
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
