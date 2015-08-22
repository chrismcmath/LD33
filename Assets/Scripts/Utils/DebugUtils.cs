using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Monster.Utils {
    public static class DebugUtils {
        public static void DrawBounds(Bounds bounds) {
            DrawLine(bounds.min, new Vector2(bounds.min.x, bounds.max.y));
            DrawLine(bounds.min, new Vector2(bounds.max.x, bounds.min.y));
            DrawLine(bounds.max, new Vector2(bounds.min.x, bounds.max.y));
            DrawLine(bounds.max, new Vector2(bounds.max.x, bounds.min.y));
        }

        public static void DrawLine(Vector2 from, Vector2 to) {
            Gizmos.DrawLine(from, to);
        }

        // Adapted from AnomalusUndrdog (via http://forum.unity3d.com/threads/debug-drawarrow.85980/)
        public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * GetArrowAngle(direction, arrowHeadAngle);
            Vector3 left = Quaternion.LookRotation(direction) * GetArrowAngle(direction, -arrowHeadAngle);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void ForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
            Gizmos.color = color;
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * GetArrowAngle(direction, arrowHeadAngle);
            Vector3 left = Quaternion.LookRotation(direction) * GetArrowAngle(direction, -arrowHeadAngle);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void ForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
            Debug.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * GetArrowAngle(direction, arrowHeadAngle);
            Vector3 left = Quaternion.LookRotation(direction) * GetArrowAngle(direction, -arrowHeadAngle);
            Debug.DrawRay(pos + direction, right * arrowHeadLength);
            Debug.DrawRay(pos + direction, left * arrowHeadLength);
        }
        public static void ForDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
            Debug.DrawRay(pos, direction, color);

            Vector3 right = Quaternion.LookRotation(direction) * GetArrowAngle(direction, arrowHeadAngle);
            Vector3 left = Quaternion.LookRotation(direction) * GetArrowAngle(direction, -arrowHeadAngle);
            Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
            Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
        }

        private static Vector3 GetArrowAngle(Vector3 direction, float arrowHeadAngle) {
            return Quaternion.LookRotation(direction) * Quaternion.Euler(0,0,270+arrowHeadAngle) * new Vector3(0,1,0);
        }
    }
}
