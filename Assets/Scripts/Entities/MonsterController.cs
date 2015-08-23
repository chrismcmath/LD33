using UnityEngine;
using System.Collections;

using Monster.Behaviours;
using Monster.Camera;
using Monster.Planets;
using Monster.Utils;

namespace Monster.Entities {
    public class MonsterController : EntityController {
        public RectTransform PowerBarTransform;

        public float Power = 0f;

        public void Awake() {
            Behaviour = gameObject.AddComponent<IdleBehaviour>();
        }

        public void Update() {
            base.Update();

            PowerBarTransform.sizeDelta += Vector2.right;
        }
    }
}
