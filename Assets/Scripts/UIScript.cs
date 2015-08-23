using UnityEngine;
using System.Collections;

public class UIScript : MonoBehaviour {
    public Transform[] Items;
    public float Interval = 5f;

    private int _Iterator = 0;
    private float _Timer = 0f;
    private Transform _PrevTransform = null;

    public void Update() {
        if (_Timer <= 0f) {
            if (_PrevTransform != null) {
                _PrevTransform.gameObject.SetActive(false);
            }
            if (_Iterator < Items.Length) {
                Items[_Iterator].gameObject.SetActive(true);
                _PrevTransform = Items[_Iterator];
                _Timer = Interval;
                _Iterator++;
            }
        }

        _Timer -= Time.deltaTime;
    }
}
