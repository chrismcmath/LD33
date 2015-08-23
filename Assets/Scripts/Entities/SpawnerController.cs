using UnityEngine;
using System.Collections;

using Monster.Behaviours;
using Monster.Camera;
using Monster.Planets;
using Monster.Utils;

namespace Monster.Entities {
    public class SpawnerController : MonoBehaviour {
        public MeshRenderer RoofRenderer;

        public float SpawnTimerMin = 2f;
        public float SpawnTimerMax = 30f;
        public PlanetController HostPlanet;
        public GameController Game;

        private int _SpawnCount = 1;
        private float _SpawnTimer;

        public void Awake() {
            ResetTimer();
        }

        public void Update() {
            if (_SpawnCount <= 0) return;

            _SpawnTimer -= Time.deltaTime;
            if (_SpawnTimer < 0f ) {
                Spawn();
                ResetTimer();
            }
        }

        private void ResetTimer() {
            _SpawnTimer = Random.Range(SpawnTimerMin, SpawnTimerMax);
        }

        private void Spawn() {
            GameObject humanGO = Instantiate(Resources.Load("Human") as GameObject);
            humanGO.transform.parent = HostPlanet.transform;
            humanGO.transform.position = transform.position;
            // May whatever god there is forgive me.
            humanGO.GetComponent<HumanController>().Game = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<MonsterController>().Game;
            _SpawnCount--;
        }
    }
}
