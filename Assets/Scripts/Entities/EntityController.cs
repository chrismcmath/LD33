using UnityEngine;
using System.Collections;

using Monster.Behaviours;
using Monster.Camera;
using Monster.Planets;
using Monster.Utils;

namespace Monster.Entities {
    public abstract class EntityController : MonoBehaviour {
        public GameController Game;

        public float RotationSpeed = 10f;
        public LayerMask PlanetLayerMask;

        public MonsterBehaviour Behaviour;

        public PlanetController HostPlanet;
        public Vector2 GroundPoint = Vector2.zero;

        protected Vector2 _FacingVector = Vector2.right;
        public Vector2 FacingVector {
            get { return _FacingVector; }
            set {
                if (_FacingVector == value || (value != -1 * Vector2.right && value != Vector2.right)) {
                    return;
                }

                _FacingVector = value;

                transform.localScale = new Vector3(
                        _FacingVector.x,
                        transform.localScale.y,
                        transform.localScale.z);
            }
        }

        public void Update() {
            HostPlanet = Game.GetClosestPlanet((Vector2) transform.position);

            if (Behaviour != null) {
                Behaviour.UpdateBehaviour();
            }
        }

        public void FixedUpdate() {
            if (Behaviour != null) {
                Behaviour.FixedUpdateBehaviour();
            }
        }

        public void RemoveMonsterBehaviour() {
            Destroy(Behaviour);
        }

    }
}
