﻿using UnityEngine;
using System.Collections;

using Monster.Entities;
using Monster.Utils;

namespace Monster.Behaviours {
    public class IdleBehaviour : MonsterBehaviour {
        public float GroundSpeed = 100f;
        public float AirSpeed = 10f;
        public float JumpSpeed = 20f;
        public float JumpForwardSpeed = 2f;
        public float DiveSpeed = 10f;
        public float BoostSpeed = 150f;
        private float HurtSpeed = 20f;
        public float MaxHorizontalGroundSpeed = 0.5f;
        public float MaxHorizontalAirSpeed = 0.6f;
        public float AirGravity = 60;
        public float GroundGravity = 100f;
        public float DiveDrag = 2f;
        public float NormalDrag = 1f;
        public float DiveTime = 1f;

        private float _DiveTimer = 0f;

        public float JumpDisableTimeout = 0.1f;

        private Quaternion _TargetRotation;

        private bool _Diving = false;

        private bool _Grounded = false;
		private float _LastJumpTime = 0f; 
		private Vector2 _GravityDirection = Vector2.zero; 

        private Vector2 DEBUG_HitPosition = Vector2.zero;
        private Vector2 DEBUG_HitNormal = Vector2.zero;

		protected override void OnSwitchIn() {
            _Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
		}

        protected override void UpdateRigidbody() {
            CheckGrounded();
            UpdateGravity();
            UpdateRotation();
        }

        protected override void OnAction1Down() {
			CheckGrounded();

            if (_Grounded || _Diving) {
                PerformJump(); 
			} else {
                PerformBoost(); 
			}
		}

        protected override void OnAction2Down() {
			CheckGrounded();

            if (_Diving) {
                PerformJump();
            } else if (_Grounded) {
                PerformDive(); 
			} else {
                PerformAirDive();
			}
		}
		
        protected override void UpdateContinuousInput() {
            if (_Diving && _DiveTimer < 0f) {
                PerformJump();
            }

            _DiveTimer -= Time.deltaTime;

            UpdateDirection();
		}

        private void CheckGrounded() {
            if (!_Grounded &&
                    _TimeActive - _LastJumpTime > JumpDisableTimeout &&
                    ColliderUtils.IsIntersecting(_GroundCollider, "Planet")) {
                _Grounded = true;
                _ParticleSystem.Play();
                OnTouchGround();
            } else if (_Grounded && !ColliderUtils.IsIntersecting(_GroundCollider, "Planet")) {
                _Grounded = false;
                _Diving = false;
                _ParticleSystem.Stop();
            }
        }

        private void UpdateDirection() {
            Vector2 right = transform.localRotation * Vector2.right;
            float axis = ControllerUtils.GetHorizontalMovement();

            if (axis > 0f) {
                _Controller.FacingVector = Vector2.right;
            } else if (axis < 0f) {
                _Controller.FacingVector = -1 * Vector2.right;
            }

            bool facingRight = _Controller.FacingVector.x == 1;

            float speed = _Grounded ?
                GroundSpeed :
                AirSpeed;
            float max = _Grounded ?
                MaxHorizontalGroundSpeed :
                MaxHorizontalAirSpeed;

            speed = Mathf.Min(speed, max);
            speed *= _Controller.FacingVector.x;

            _Rigidbody.AddForce(right * speed, ForceMode2D.Impulse);

            if (_Grounded) {
                //_AnimationController.Running(true);
                //_SoundController.Footstep();
            }
        }
	
        private void OnTouchGround() {
            GetComponentInChildren<Animator>().Play("Run");
            //_AnimationController.Land();
            //_SoundController.Footstep();
        }

        private void PerformJump() {
            _Diving = false;
            _Rigidbody.drag = NormalDrag;

            GetComponentInChildren<Animator>().Play("Jump");
            _Controller.PlayJump();

            _LastJumpTime = _TimeActive;

            Vector3 force = -1f *_GravityDirection * JumpSpeed;
            _Rigidbody.AddForce(force, ForceMode2D.Impulse);

            Vector2 right = transform.localRotation * Vector2.right * _Controller.FacingVector.x;
            _Rigidbody.AddForce(right * JumpForwardSpeed, ForceMode2D.Impulse);
        }

        private void PerformBoost() {
            Vector3 force = -1f *_GravityDirection * BoostSpeed * _Controller.Power;
            _Controller.DrainPower();
            _Controller.Play("boost");
            _Rigidbody.AddForce(force, ForceMode2D.Impulse);
        }

        private void PerformDive() {
            Debug.Log("PerformDive");
            _Diving = true;
            _DiveTimer = DiveTime;
            GetComponentInChildren<Animator>().Play("Dive");
            _Controller.Play("dive");

            Vector2 right = transform.localRotation * Vector2.right * _Controller.FacingVector.x;
            _Rigidbody.drag = DiveDrag;
            _Rigidbody.AddForce(right * DiveSpeed, ForceMode2D.Impulse);
        }

        private void PerformAirDive() {
        }

        private void UpdateRotation() {
            transform.rotation = Quaternion.Slerp(transform.rotation, _TargetRotation, Time.deltaTime * _Controller.RotationSpeed);
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
            if (_Controller.HostPlanet == null) {
                return Vector2.zero;
            }

            Vector2 toPlanet = (Vector2) (_Controller.HostPlanet.transform.position - transform.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlanet, Mathf.Infinity, _Controller.PlanetLayerMask);
            if (hit.collider != null) {
                //Debug.Log("hit: " + hit.collider.name);
                _Controller.GroundPoint = hit.point;
                DEBUG_HitPosition = hit.point;
                DEBUG_HitNormal = hit.normal;
                return -1 * hit.normal;
            } else {
                _Controller.GroundPoint = Vector2.zero;
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

        public void OnTriggerEnter2D(Collider2D col) {
            //Debug.Log("? ? ? OnTriggerEnter2D col " + col.name);
            switch (col.tag) {
                case "Coin":
                    col.transform.parent.GetComponent<CoinController>().Collect();
                    _Controller.AddPower();
                    break;
                case "Human":
                    //Vector2 vel = _Rigidbody.velocity.normalized + (-1f * _GravityDirection.normalized);
                    if (_Diving) {
                        Vector2 vel = ((_Rigidbody.velocity.normalized * 3) + (_GravityDirection.normalized * -1f)).normalized;
                        col.transform.parent.GetComponent<HumanController>().Attacked(vel);
                        _Controller.PlayHitHuman();
                    } else {
                        _Controller.DrainPower();
                        _Controller.FacingVector *= -1f;
                        _Controller.Game.CamController.ScreenShake = 1f;
                        Vector2 right = transform.localRotation * Vector2.right * _Controller.FacingVector.x;
                        _Rigidbody.AddForce(right * HurtSpeed, ForceMode2D.Impulse);
                        _Controller.Play("damage");
                        col.transform.parent.GetComponent<HumanController>().PlayHit();
                    }
                    break;
            }
        }

        public void OnCollisionEnter2D(Collision2D col) {
            //Debug.Log("? ? ? ? ? ? ? ? ? ? col " + col.collider.name);
        }
    }
}
