using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Monster.Planets;

namespace Monster {
    public class Hex : MonoBehaviour {

        //TODO: move to own space
        public UnityEngine.Camera Cam;

        private Dictionary<int, PlanetController> _Planets = new Dictionary<int, PlanetController>();
        private List<PlanetController> _VisitedPlanets = new List<PlanetController>();

        public void Start () {
        }

        public void Update () {
        }

        public void OnDrawGizmos() {
        }
    }
}
