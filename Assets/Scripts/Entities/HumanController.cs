using UnityEngine;
using System.Collections;

using Monster.Behaviours;
using Monster.Camera;
using Monster.Planets;
using Monster.Utils;

namespace Monster.Entities {
    public class HumanController : EntityController {
        public void Awake() {
            Behaviour = gameObject.AddComponent<IdleHumanBehaviour>();
        }
    }
}
