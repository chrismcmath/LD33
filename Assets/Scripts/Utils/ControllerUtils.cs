using UnityEngine;
using System.Collections;

namespace Monster.Utils {
    public static class ControllerUtils {

        public static float GetHorizontalMovement() {
            return Input.GetAxis("Horizontal");
        }

        public static bool Action1Button() {
            return Input.GetButton("Action1");
        }

        public static bool Action1Down() {
            return Input.GetButtonDown("Action1");
        }

        public static bool Action1Up() {
            return Input.GetButtonUp("Action1");
        }

        public static bool Action2Button() {
            return Input.GetButton("Action2");
        }

        public static bool Action2Down() {
            return Input.GetButtonDown("Action2");
        }

        public static bool Action2Up() {
            return Input.GetButtonUp("Action2");
        }
    }
}
