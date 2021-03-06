﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Monster.Camera;
using Monster.Planets;
using Monster.Utils;

namespace Monster {
    public class GameController : MonoBehaviour {
        private static float SQRT_3_2 = Mathf.Sqrt(3f) / 2f;

        public float Scale = 1.0f;
        public float EventHorizon = 50f;

        public Transform Canvas;
        public RectTransform PointsEnd;

        private float _CachedScale;
        private float _Height = 0f;
        private float _HalfWidth = 0f;
        private float _Width = 0f;

        //TODO: move to own space
        public CameraController CamController;

        private Dictionary<Vector2, PlanetController> _ActivePlanets = new Dictionary<Vector2, PlanetController>();
        private List<Vector2> _VisitedPlanets = new List<Vector2>();

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

            UpdatePlanetPositions(CamController.ExistenceBounds);
        }

        public PlanetController GetClosestPlanet(Vector2 pos, bool useEventHorizon) {
            List<Vector2> keys = _ActivePlanets.Keys.ToList();

            PlanetController closestPlanet = null;
            float closestMagnitude = Mathf.Infinity;
            foreach (KeyValuePair<Vector2, PlanetController> p in _ActivePlanets) {

                if ((pos - p.Key).magnitude < closestMagnitude) {
                    closestPlanet = p.Value;
                    closestMagnitude = (pos - p.Key).magnitude;
                }
            }
            if (useEventHorizon && closestMagnitude > EventHorizon) {
                return null;
            } else {
                return closestPlanet;
            }
        }

        /*
        public Vector2 GetClosestPlanetPoint(Vector2 pos) {
            List<Vector2> keys = _ActivePlanets.Keys.ToList();

            Vector2 closest = Vector2.zero;
            float closestMagnitude = Mathf.Infinity;
            foreach (Vector2 p in keys) {
                if ((pos - p).magnitude < closestMagnitude) {
                    closest = p;
                    closestMagnitude = (pos - p).magnitude;
                }
            }
            return closest;
        }
        */

        private void UpdatePlanetPositions(Bounds bounds) {
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

            foreach (Vector2 pos in _RetiredPlanetPositions) {
                PlanetController pc = _ActivePlanets[pos];
                Destroy(pc.gameObject);
                _ActivePlanets.Remove(pos);
            }

            foreach (Vector2 pos in _NewPlanetPositions) {
                PlanetController pc = CreatePlanet(pos);
                _ActivePlanets.Add(pos, pc);
                _VisitedPlanets.Add(pos);
            }

            //Debug.Log("[Hex] old: " + _RetiredPlanetPositions.Count + " new: " + _NewPlanetPositions.Count);
            //Debug.Log("[Game] Planets: " + _ActivePlanets.Count + " Visited " + _VisitedPlanets.Count);

            _PlanetPositions = planetPositions;
        }

        private PlanetController CreatePlanet(Vector2 pos) {
            GameObject planetGO = Instantiate(Resources.Load("Planet") as GameObject);
            planetGO.transform.position = pos;
            return planetGO.GetComponent<PlanetController>();
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
