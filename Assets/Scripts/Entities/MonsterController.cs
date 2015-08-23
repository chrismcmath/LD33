using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Monster.Behaviours;
using Monster.Camera;
using Monster.Planets;
using Monster.Utils;

namespace Monster.Entities {
    public class MonsterController : EntityController {
        public Image PowerImage;

        private float _Power = 0f;
        public float Power {
            get { return _Power; }
        }

        public void Awake() {
            Behaviour = gameObject.AddComponent<IdleBehaviour>();
        }

        public void Update() {
            base.Update();

            //PowerBarTransform.sizeDelta += Vector2.right;
            PowerImage.fillAmount = _Power;
        }

        public void AddPower() {
            StartCoroutine(AddPowerAsync());
        }

        public void DrainPower() {
            StartCoroutine(DrainPowerAsync());
        }

        private IEnumerator AddPowerAsync() {
            float target = Mathf.Min(1f, _Power + 0.1f);
            while (_Power < target) {
                _Power += 0.01f;
                yield return null;
            }
        }

        private IEnumerator DrainPowerAsync() {
            while (_Power > 0f) {
                _Power -= 0.01f;
                yield return null;
            }
        }
    }
}
