﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Monster.Utils;

namespace Monster.Camera {
    public class CameraController : MonoBehaviour {
        public float MinDistance = 10f;
        public float Padding = 2f;
        public float Dampening = 0.1f;
        public Bounds SearchBounds;
        public Bounds ExistenceBounds;

        public Transform[] DEBUG_PointsOfInterest;
        public Transform Player;

        private UnityEngine.Camera _Camera;
        private List<Transform> _PointsOfInterest = new List<Transform>();
        private Bounds _DebugBounds;

        public void Awake() {
            _Camera = GetComponent<UnityEngine.Camera>();

            if (_Camera == null) {
                Debug.LogError("[Camera] Could not find camera on gameobject");
            }

            //TODO: remove
            foreach (Transform t in DEBUG_PointsOfInterest) {
                RegisterPointOfInterest(t);
            }
        }

        public void LateUpdate() {
            UpdateBounds();
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

        private void UpdatePointsOfInterest() {
            Bounds poiBounds = GetBoundsFromPointsOfInterest();
            _DebugBounds = poiBounds;

            Vector3 targetPos = new Vector3(
                    poiBounds.center.x,
                    poiBounds.center.y,
                    -1f * Mathf.Max(MinDistance, GetDistance(poiBounds)));

            Vector3 tweenedPos;
            float tweenedRot;
            if (Dampening > 0f) {
                tweenedPos = _Camera.transform.position + ((targetPos - _Camera.transform.position) * (Time.deltaTime / Dampening));
                tweenedRot = _Camera.transform.rotation.eulerAngles.z + (((Player.rotation.eulerAngles.z + 90f) - _Camera.transform.rotation.eulerAngles.z) * (Time.deltaTime / Dampening));
            } else {
                tweenedPos = targetPos;
                tweenedRot = Player.rotation.eulerAngles.z + 90f;
            }
            _Camera.transform.position = tweenedPos;
            //_Camera.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, tweenedRot));
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

            bool isFirst = true;
            foreach (Transform t in _PointsOfInterest) {
                if (isFirst) {
                    min = t.position;
                    max = t.position;
                    isFirst = false;
                }
                min = new Vector2(
                        Mathf.Min(min.x, t.position.x - Padding),
                        Mathf.Min(min.y, t.position.y - Padding));
                max = new Vector2(
                        Mathf.Max(max.x, t.position.x + Padding),
                        Mathf.Max(max.y, t.position.y + Padding));
            }
            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
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
