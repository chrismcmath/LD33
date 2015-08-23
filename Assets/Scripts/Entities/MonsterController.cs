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
        public Text[] DistanceLabels;

        public AudioSource DistanceSound;
        public AudioSource Voice;

        private int _Distance = 0;

        private float _Power = 0f;
        public float Power {
            get { return _Power; }
        }

        public void Awake() {
            Behaviour = gameObject.AddComponent<IdleBehaviour>();
        }

        public void PlayJump() {
            int i = Random.Range(0, 3);
            Play(string.Format("jump{0}", i));
        }

        public void PlayHitHuman() {
            int i = Random.Range(0, 2);
            Play(string.Format("hit_human{0}", i));
        }

        public void Play(string name) {
            Debug.Log("play " + name);
            Voice.Stop();
            Voice.clip = Resources.Load(string.Format(name), typeof(AudioClip)) as AudioClip;
            Voice.Play();
        }

        public void Update() {
            base.Update();

            //PowerBarTransform.sizeDelta += Vector2.right;
            PowerImage.fillAmount = _Power;

            int distance = Mathf.FloorToInt(transform.position.magnitude);
            if (_Distance != distance) {
                DistanceSound.pitch = 1f + ((distance - _Distance) / 10f);
                DistanceSound.Stop();
                DistanceSound.Play();
                _Distance = distance;
            }

            foreach (Text text in DistanceLabels) {
                text.text = string.Format("{0}ft", Mathf.FloorToInt(transform.position.magnitude));
            }
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
