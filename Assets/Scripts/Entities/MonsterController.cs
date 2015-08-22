using UnityEngine;
using System.Collections;

namespace Monster.Entities {
    public class MonsterController : MonoBehaviour {
        public float Speed = 10f;

        public void Update() {
            transform.position += new Vector3(
                    Input.GetAxis("Horizontal") * Speed * Time.deltaTime,
                    Input.GetAxis("Vertical") * Speed * Time.deltaTime,
                    0f);
        }
    }
}
