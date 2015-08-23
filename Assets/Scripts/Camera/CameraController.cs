using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Monster.Entities;
using Monster.Utils;

namespace Monster.Camera {
    public class CameraController : MonoBehaviour {
        public float MinDistance = 10f;
        public float Padding = 2f;
        public float FollowSpeed = 10f;
        public float RotationSpeed = 10f;
        public float ScreenShake = 0f;
        public Bounds SearchBounds;
        public Bounds ExistenceBounds;

        public MonsterController Player;

        private UnityEngine.Camera _Camera;
        private List<Transform> _PointsOfInterest = new List<Transform>();
        private Bounds _DebugBounds;

        public void Awake() {
            _Camera = GetComponent<UnityEngine.Camera>();

            if (_Camera == null) {
                Debug.LogError("[Camera] Could not find camera on gameobject");
            }
        }

        public void Update() {
            ScreenShake *= 0.9f;
        }

        public void FixedUpdate() {
            UpdateBounds();
            CompilePointsOfInterest();
            UpdatePointsOfInterest();
        }

        public void RegisterPointOfInterest(Transform t) {
            if (!_PointsOfInterest.Contains(t)) {
                _PointsOfInterest.Add(t);
            } else {
                Debug.Log(t.name + " already registered");
            }
        }

        public void DeregisterPointOfInterest(Transform t) {
            if (_PointsOfInterest.Contains(t)) {
                _PointsOfInterest.Remove(t);
            } else {
                Debug.Log(t.name + " not found");
            }
        }

        public void UpdateBounds() {
            SearchBounds.center = transform.position;
            ExistenceBounds.center = transform.position;
        }

        private void CompilePointsOfInterest() {
            _PointsOfInterest.Clear();

            foreach (GameObject poiGO in GameObject.FindGameObjectsWithTag("POI")) {
                Vector2 pos = (Vector2) poiGO.transform.position;
                if (SearchBounds.Contains(pos)) {
                    _PointsOfInterest.Add(poiGO.transform);
                }
            }
        }

        private void UpdatePointsOfInterest() {
            Bounds poiBounds = GetBoundsFromPointsOfInterest();
            _DebugBounds = poiBounds;

            Vector3 targetPos = new Vector3(
                    //poiBounds.center.x,
                    //poiBounds.center.y,
                    Player.transform.position.x,
                    Player.transform.position.y,
                    -1f * Mathf.Max(MinDistance, GetDistance(poiBounds)));

            _Camera.transform.position = Vector3.Lerp(_Camera.transform.position, targetPos, Time.deltaTime * FollowSpeed);
            _Camera.transform.position += new Vector3(
                    Random.Range(-1f, 1f) * ScreenShake,
                    Random.Range(-1f, 1f) * ScreenShake,
                    0f);

            _Camera.transform.rotation = Quaternion.Slerp(_Camera.transform.rotation, Player.transform.rotation, Time.deltaTime * RotationSpeed);
        }

        private float GetDistance(Bounds poiBounds) {
            float height = poiBounds.size.y;
            float heightFromWidth = poiBounds.size.x / _Camera.aspect;
            float frustumHeight = Mathf.Max(height, heightFromWidth);

            //http://docs.unity3d.com/Manual/FrustumSizeAtDistance.html
            float distance = frustumHeight * 0.5f / Mathf.Tan(_Camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            return distance;
        }

        private Bounds GetBoundsFromPointsOfInterest() {
            Vector2 min = Vector2.zero;
            Vector2 max = Vector2.zero;

            UpdateBoundsFromPointOfInterest(Player.transform.position, ref min, ref max, true);
            if (Player.HostPlanet != null) {
                UpdateBoundsFromPointOfInterest(Player.GroundPoint, ref min, ref max, false);
            }

            foreach (Transform t in _PointsOfInterest) {
                UpdateBoundsFromPointOfInterest(t.position, ref min, ref max, false);
            }

            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }

        private void UpdateBoundsFromPointOfInterest(Vector2 point, ref Vector2 min, ref Vector2 max, bool isFirst) {
                if (isFirst) {
                    min = point;
                    max = point;
                }

                min = new Vector2(
                        Mathf.Min(min.x, point.x - Padding),
                        Mathf.Min(min.y, point.y - Padding));
                max = new Vector2(
                        Mathf.Max(max.x, point.x + Padding),
                        Mathf.Max(max.y, point.y + Padding));
        }

        public void OnDrawGizmos() {
            foreach (Transform transform in _PointsOfInterest) {
                Gizmos.color = Color.green;
                DebugUtils.DrawBounds(new Bounds(transform.position, Vector2.one));
            }
            if (_DebugBounds != null) {
                Gizmos.color = Color.yellow;
                DebugUtils.DrawBounds(_DebugBounds);
            }
            Gizmos.color = Color.white;
            DebugUtils.DrawBounds(SearchBounds);
            Gizmos.color = Color.red;
            DebugUtils.DrawBounds(ExistenceBounds);
        }
    }
}
