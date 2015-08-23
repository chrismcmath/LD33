using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Monster.Entities;
using Monster.Utils;

namespace Monster.Planets {
    public class PlanetController : MonoBehaviour {
        public const float ATMOSPHERE_TO_RADIUS_RATIO = 7f;
        public float PERLIN_SIZE = 10f;

        public GameObject Inner; 
        public MeshRenderer Atmosphere;

        //NOTE: Radius is like the general size
        //NOTE: Height will define mountains etc

        public float PlanetRadiusMin = 10f;
        public float PlanetRadiusMax = 20f;

        public float PlanetHeightRange = 10f;

        // trying to describe each step around a circle
        public float PlanetArcMin = 1f;
        public float PlanetArcMax = 20f;

        private float SpawnerProbability = 0.1f;

        public bool RedrawPlanet = true;

        private float _Radius;
        private MeshFilter _OuterMesh;
        private MeshFilter _InnerMesh;
        private List<Vector3> _PlanetVertices = new List<Vector3>();

        public void Start() {
            _InnerMesh = Inner.AddComponent<MeshFilter>();
            _OuterMesh = gameObject.AddComponent<MeshFilter>();
            Redraw();
        }

        public void Update() {
            if (RedrawPlanet) {
                Redraw();
                RedrawPlanet = false;
            }
        }

        public void OnDrawGizmos() {
            return;
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
            RecreateMesh(_InnerMesh, 0.99f);
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

            //TODO: hack, trying to reuse me cast
            PolygonCollider2D col = gameObject.AddComponent<PolygonCollider2D>();
            col.SetPath(0, vertices.ToArray());

            return triangulator.Triangulate();
        }

        private Vector2[] GetMeshUVs() {
            return new Vector2[0];
        }

        private void RecalculatePoints() {
            _PlanetVertices.Clear();

            _Radius = GetPerlinNumber(PlanetRadiusMin, PlanetRadiusMax);
            Color randomColor = ColorUtils.ColorFromHSV(GetPerlinNumber(0f, 360f), 1f, 1f);
            Inner.GetComponent<MeshRenderer>().material.color = randomColor;
            GetComponent<MeshRenderer>().material.color = randomColor / 2;

            float h, s, v;
            ColorUtils.ColorToHSV(randomColor, out h, out s, out v);

            h += 180f;
            h = h > 360f ? h - 360f : h;

            Color complement = ColorUtils.ColorFromHSV(h, s, v);
            Atmosphere.material.SetColor("_TintColor", complement);
            Atmosphere.transform.localScale = Vector2.one * _Radius * ATMOSPHERE_TO_RADIUS_RATIO; 

            float prevHeight = 0f;
            Vector3 prevVertex = Vector3.zero;
            for (float angle = 0f; angle < 360f;) {
                Vector2 direction = Vector2.right;

                float rotation = Mathf.Deg2Rad * angle;

                float cosTheta = Mathf.Cos(rotation);
                float sinTheta = Mathf.Sin(rotation);

                Vector2 nextDirection;
                nextDirection.x = direction.x * cosTheta - direction.y * sinTheta;
                nextDirection.y = direction.x * sinTheta + direction.y * cosTheta;
                direction = nextDirection;

                float height = prevHeight + (GetPerlinNumber(-PlanetHeightRange, PlanetHeightRange, angle) * _Radius);
                Vector3 vertex = ((Vector3) direction) * (_Radius + height);
                _PlanetVertices.Add(vertex);

                float random = Random.Range(0f, 1f); 
                Debug.Log("planet " + transform.position + " prevVertex: " + prevVertex + " random: " + random);
                if (prevVertex != Vector3.zero && Random.Range(0f, 1f) < SpawnerProbability) {
                    GameObject spawnerGO = Instantiate(Resources.Load("HumanSpawner") as GameObject);
                    spawnerGO.transform.parent = transform;
                    Vector3 pos = (vertex + (prevVertex - vertex)/2f) + transform.position;
                    spawnerGO.transform.position = pos;
                    spawnerGO.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, VectorUtils.GetAngle(Vector2.up, pos - transform.position)));
                    spawnerGO.GetComponent<SpawnerController>().RoofRenderer.material.color = randomColor / 2;
                    spawnerGO.GetComponent<SpawnerController>().HostPlanet = this;
                }


                prevVertex = vertex;

                angle += GetPerlinNumber(PlanetArcMin, PlanetArcMax, angle) * (1f/ _Radius);
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
