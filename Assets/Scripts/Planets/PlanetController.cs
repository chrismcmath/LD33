using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Monster.Utils;

namespace Monster.Planets {
    public class PlanetController : MonoBehaviour {
        public float PERLIN_SIZE = 10f;

        public GameObject Inner; 

        //NOTE: Radius is like the general size
        //NOTE: Height will define mountains etc

        public float PlanetRadiusMin = 10f;
        public float PlanetRadiusMax = 20f;

        public float PlanetHeightMin = 1f;
        public float PlanetHeightMax = 20f;

        // trying to describe each step around a circle
        public float PlanetArcMin = 1f;
        public float PlanetArcMax = 20f;

        public bool RedrawPlanet = true;

        private float _Radius;
        private MeshFilter _OuterMesh;
        private MeshFilter _InnerMesh;
        private List<Vector3> _PlanetVertices = new List<Vector3>();

        public void Awake() {
            _OuterMesh = gameObject.AddComponent<MeshFilter>();

            GameObject meshChild = new GameObject();
            _InnerMesh = Inner.AddComponent<MeshFilter>();
            Redraw();
        }

        public void Update() {
            if (true) {
                Redraw();
                RedrawPlanet = false;

            }
        }

        public void OnDrawGizmos() {
            Gizmos.color = Color.black;
            if (_PlanetVertices.Count < 1) {
                return;
            }
            Vector3 prevV = _PlanetVertices[_PlanetVertices.Count - 1];
            foreach (Vector3 vertex in _PlanetVertices) {
                DebugUtils.DrawLine(prevV, vertex);
                prevV = vertex;
                Gizmos.color += new Color(0.01f, 0.001f, 0.0005f, 0.01f);
            }
        }

        private void Redraw() {
            RecalculatePoints();
            RecreateMeshes();
        }

        private void RecreateMeshes() {
            RecreateMesh(_InnerMesh, 0.9f);
            RecreateMesh(_OuterMesh, 1.0f);
        }

        private void RecreateMesh(MeshFilter filter, float edgeWidth) {
            Mesh mesh = new Mesh();
            filter.mesh = mesh;
            mesh.vertices = GetMeshVerticies(edgeWidth);
            mesh.triangles = GetMeshTriangles();
            mesh.uv = GetMeshUVs();
        }

        private Vector3[] GetMeshVerticies(float edgeWidth) {
            List<Vector3> vertices = new List<Vector3>();
            foreach (Vector3 v in _PlanetVertices) {
                vertices.Add(v * edgeWidth);
            }
            return vertices.ToArray();
        }

        private int[] GetMeshTriangles() {
            List<Vector2> vertices = new List<Vector2>();
            foreach (Vector3 v in _PlanetVertices) {
                vertices.Add((Vector2) v);
            }
            Triangulator triangulator = new Triangulator(vertices.ToArray());
            return triangulator.Triangulate();
        }

        private Vector2[] GetMeshUVs() {
            return new Vector2[0];
        }

        private void RecalculatePoints() {
            _PlanetVertices.Clear();

            _Radius = GetPerlinNumber(PlanetRadiusMin, PlanetRadiusMax);

            for (float angle = 0f; angle < 360f;) {
                Vector2 direction = Vector2.right;

                float rotation = Mathf.Deg2Rad * angle;

                float cosTheta = Mathf.Cos(rotation);
                float sinTheta = Mathf.Sin(rotation);

                Vector2 nextDirection;
                nextDirection.x = direction.x * cosTheta - direction.y * sinTheta;
                nextDirection.y = direction.x * sinTheta + direction.y * cosTheta;
                direction = nextDirection;

                Vector3 vertex = (((Vector3) direction) * _Radius) * GetPerlinNumber(PlanetHeightMin, PlanetHeightMax, angle);
                _PlanetVertices.Add(vertex);

                angle += GetPerlinNumber(PlanetArcMin, PlanetArcMax, angle);
            }
        }

        private float GetPerlinNumber(float min, float max) {
            float fraction = Mathf.PerlinNoise((transform.position.x / PERLIN_SIZE), (transform.position.y / PERLIN_SIZE));
            float number = min + ((max - min) * fraction);
            return number;
        }

        private float GetPerlinNumber(float min, float max, float angle) {
            float fraction = Mathf.PerlinNoise((transform.position.x / PERLIN_SIZE) * angle, (transform.position.y / PERLIN_SIZE) * angle);
            float number = min + ((max - min) * fraction);
            return number;
        }
    }
}
