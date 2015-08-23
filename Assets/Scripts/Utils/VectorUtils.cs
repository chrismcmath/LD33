using UnityEngine;
using System.Collections;

namespace Monster.Utils {
    public static class VectorUtils {
        public static float GetAngle(Vector2 a, Vector2 b) {
            float angle = Vector2.Angle(a, b);
            Vector3 cross = Vector3.Cross(a, b);
            if (cross.z < 0f) {
                angle = -angle;
            }
            return angle;
        }
    }
}
