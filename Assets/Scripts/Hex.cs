using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Monster.Camera;
using Monster.Planets;
using Monster.Utils;

namespace Monster {
    public class Hex : MonoBehaviour {
        private static float SQRT_3_2 = Mathf.Sqrt(3f) / 2f;

        public float Scale = 1.0f;

        private float _CachedScale;
        private float _Height = 0f;
        private float _HalfWidth = 0f;
        private float _Width = 0f;

        //TODO: move to own space
        public CameraController _CamController;

        private Dictionary<int, PlanetController> _Planets = new Dictionary<int, PlanetController>();
        private List<PlanetController> _VisitedPlanets = new List<PlanetController>();

        private List<Vector2> _PlanetPositions = new List<Vector2>();
        private List<Vector2> _NewPlanetPositions = new List<Vector2>();
        private List<Vector2> _RetiredPlanetPositions = new List<Vector2>();

        public void Start () {
        }

        public void Update() {
            if (_CachedScale != Scale) {
                _CachedScale = Scale;
                _Height = Scale;
                _Width = SQRT_3_2 * _Height;
                _HalfWidth = _Width / 2f;
            }

            UpdatePlanetPositions(_CamController.ExistenceBounds);
        }

        public void UpdatePlanetPositions(Bounds bounds) {
            List<Vector2> planetPositions = new List<Vector2>();

            float hexMinX = Mathf.Floor(bounds.min.x / _Width) + 1;
            float hexMinY = Mathf.Floor(bounds.min.y / _Height) + 1;

            for (float hexX = hexMinX; (hexX * _Height) < bounds.max.x; hexX++) {
                for (float hexY = hexMinY; (hexY * _Height) < bounds.max.y; hexY++) {
                    float x = hexX * _Width;
                    float y = hexY * _Height;

                    if (hexY % 2 != 0) {
                        x += _HalfWidth;
                    }

                    planetPositions.Add(new Vector2(x, y));
                }
            }

            _NewPlanetPositions = planetPositions.Except((IEnumerable<Vector2>) _PlanetPositions).ToList();
            _RetiredPlanetPositions = _PlanetPositions.Except((IEnumerable<Vector2>) planetPositions).ToList();
            Debug.Log("[Hex] old: " + _RetiredPlanetPositions.Count + " new: " + _NewPlanetPositions.Count);

            _PlanetPositions = planetPositions;
        }

        public void OnDrawGizmos() {
            DrawList(_PlanetPositions, Color.cyan);
            DrawList(_NewPlanetPositions, Color.white);
            DrawList(_RetiredPlanetPositions, Color.blue);
        }

        public void DrawList(List<Vector2> positions, Color color) {
            Gizmos.color = color;
            foreach (Vector2 v in positions) {
                DebugUtils.DrawBounds(new Bounds(v, Vector2.one));
            }
        }
    }
}
