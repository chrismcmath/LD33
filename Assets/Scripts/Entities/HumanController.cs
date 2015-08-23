using UnityEngine;
using System.Collections;

using Monster.Behaviours;
using Monster.Camera;
using Monster.Planets;
using Monster.Utils;

namespace Monster.Entities {
    public class HumanController : EntityController {
        protected Rigidbody2D _Rigidbody;
        public void Awake() {
            Behaviour = gameObject.AddComponent<IdleHumanBehaviour>();
            _Rigidbody = GetComponent<Rigidbody2D>();
        }

        public void Attacked(Vector2 direction) {
            _Rigidbody.AddForce(direction * 30f, ForceMode2D.Impulse);
            RemoveMonsterBehaviour();
            Behaviour = gameObject.AddComponent<HumanHitBehaviour>();

            for (int i = 0; i < Random.Range(0, 10); i++) {
                SpawnCoin(direction);
            }
        }

        private void SpawnCoin(Vector2 direction) {
            GameObject coinGO = Instantiate(Resources.Load("Coin") as GameObject);
            coinGO.transform.parent = HostPlanet.transform;
            coinGO.transform.position = transform.position;
            coinGO.GetComponent<CoinController>().Game = Game;
            coinGO.GetComponent<CoinController>().HostPlanet = HostPlanet;
            coinGO.GetComponent<Rigidbody2D>().AddForce(direction * 30f, ForceMode2D.Impulse);
        }
    }
}
