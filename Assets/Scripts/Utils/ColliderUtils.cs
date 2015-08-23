using UnityEngine;
using System.Collections;

namespace Monster.Utils {
    public static class ColliderUtils {
        public static bool IsIntersecting(Collider2D col, string layer) {
            int layerInt = LayerMask.NameToLayer(layer);
            return col.IsTouchingLayers(~layerInt);
        }
    }
}
