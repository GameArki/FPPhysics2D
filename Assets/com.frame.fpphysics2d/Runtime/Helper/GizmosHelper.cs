#if UNITY_EDITOR
using UnityEngine;
using FixMath.NET;

namespace JackFrame.FPPhysics2D {

    public static class GizmosHelper {

        public static void DrawPoint(in FPVector2 pos, in Color c) {
            Gizmos.color = c;
            Gizmos.DrawSphere(pos.ToVector2(), 0.05f);
        }

        public static void DrawBox(in FPVector2 min, in FPVector2 max, in Color c) {

            Gizmos.color = c;

            var minV2 = min.ToVector2();
            var maxV2 = max.ToVector2();

            // 2 _______3
            //  |      |
            // 1_______4
            Gizmos.DrawLine(minV2, new Vector2(minV2.x, maxV2.y)); // 1 -> 2
            Gizmos.DrawLine(new Vector2(minV2.x, maxV2.y), maxV2); // 2 -> 3
            Gizmos.DrawLine(maxV2, new Vector2(maxV2.x, minV2.y)); // 3 -> 4
            Gizmos.DrawLine(new Vector2(maxV2.x, minV2.y), minV2); // 4 -> 1

        }
        public static void DrawBox(in FPVector2 a, in FPVector2 b, in FPVector2 c, in FPVector2 d, in Color color) {

            Gizmos.color = color;
            var aV2 = a.ToVector2();
            var bV2 = b.ToVector2();
            var cV2 = c.ToVector2();
            var dV2 = d.ToVector2();

            // 2 _______3
            //  |      |
            // 1_______4
            Gizmos.DrawLine(aV2, bV2); // 1 -> 2
            Gizmos.DrawLine(bV2, cV2); // 2 -> 3
            Gizmos.DrawLine(cV2, dV2); // 3 -> 4
            Gizmos.DrawLine(dV2, aV2); // 4 -> 1

        }

        public static void DrawVerticis(FPVector2[] vertices, in Color c) {
            Gizmos.color = c;

            for (int i = 0; i < vertices.Length; i += 1) {
                var cur = vertices[i];
                FPVector2 next;
                if (i < vertices.Length - 1) {
                    next = vertices[i + 1];
                } else {
                    next = vertices[0];
                }
                Gizmos.DrawLine(cur.ToVector2(), next.ToVector2());
            }

        }

        public static void DrawCube(in FPVector2 center, in FPVector2 size, in Color c) {
            Gizmos.color = c;
            Gizmos.DrawWireCube(center.ToVector2(), size.ToVector2());
        }

    }

}

#endif