using UnityEngine;
using System.Collections;

namespace Monster.Entities {
    public class MonsterController : MonoBehaviour {
        public float Speed = 10f;

        public void Update() {
            transform.position += new Vector3(
                    Input.GetAxis("Horizontal") * Speed,
                    Input.GetAxis("Vertical") * Speed,
                    0f);
        }
    }
}
