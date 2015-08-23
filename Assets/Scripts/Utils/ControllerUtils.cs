using UnityEngine;
using System.Collections;

namespace Monster.Utils {
    public static class ControllerUtils {

        public static float GetHorizontalMovement() {
            return Input.GetAxis("Horizontal");
        }

        public static bool ActionButton() {
            return Input.GetButton("Action");
        }

        public static bool ActionDown() {
            return Input.GetButtonDown("Action");
        }

        public static bool ActionUp() {
            return Input.GetButtonUp("Action");
        }
    }
}
