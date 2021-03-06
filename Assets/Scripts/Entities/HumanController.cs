﻿using UnityEngine;
using System.Collections;

using Monster.Behaviours;
using Monster.Camera;
using Monster.Planets;
using Monster.Utils;

namespace Monster.Entities {
    public class HumanController : EntityController {
        protected int _CoinCount = 0;
        protected float _CoinTimer = 0f;
        protected Vector2 _Direction = Vector2.zero;

        protected Rigidbody2D _Rigidbody;

        public AudioSource Voice;

        public void Awake() {
            Behaviour = gameObject.AddComponent<IdleHumanBehaviour>();
            _Rigidbody = GetComponent<Rigidbody2D>();
        }

        public void PlayHit() {
            int i = Random.Range(0, 3);
            Play(string.Format("enemy_hit{0}", i));
        }

        public void PlayFly() {
            int i = Random.Range(0, 3);
            Play(string.Format("enemy_fly{0}", i));
        }

        public void Play(string name) {
            Voice.Stop();
            Voice.clip = Resources.Load(string.Format(name), typeof(AudioClip)) as AudioClip;
            Voice.Play();
        }

        public void Update() {
            base.Update();

            _CoinTimer -= Time.deltaTime;

            if (_CoinCount > 0 && _CoinTimer < 0f) {
                SpawnCoin(_Direction);

                _CoinCount -= 1;
                _CoinTimer = 0.1f;
            }
        }

        public void Attacked(Vector2 direction) {
            _Rigidbody.velocity = Vector2.zero;
            _Rigidbody.AddForce(direction * 30f, ForceMode2D.Impulse);
            RemoveMonsterBehaviour();
            Behaviour = gameObject.AddComponent<HumanHitBehaviour>();

            PlayFly();

            _CoinCount = 10;
            _Direction = direction;
        }

        /*
        private IEnumerator SpawnCoins(Vector2 direction) {
            while (--_CoinCount > 0) {
                SpawnCoin(direction);
                yield return new WaitForSeconds(0.2f);
            }
        }
        */

        private void SpawnCoin(Vector2 direction) {
            if (HostPlanet == null) return;

            GameObject coinGO = Instantiate(Resources.Load("Coin") as GameObject);
            coinGO.transform.parent = HostPlanet.transform;
            coinGO.transform.position = transform.position;
            coinGO.GetComponent<CoinController>().Game = Game;
            coinGO.GetComponent<CoinController>().HostPlanet = HostPlanet;
            coinGO.GetComponent<Rigidbody2D>().AddForce(direction * 30f, ForceMode2D.Impulse);
        }
    }
}
