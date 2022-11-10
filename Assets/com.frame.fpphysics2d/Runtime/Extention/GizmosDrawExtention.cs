using UnityEngine;
using FixMath.NET;

namespace JackFrame.FPPhysics2D {

    public static class GizmosDrawExtention {

        public static void GizmosDrawAllRigidbody(this FPSpace2D space2D) {
            var all = space2D.GetAllRigidbody();
            for (int i = 0; i < all.Length; i += 1) {
                all[i].GizmosDrawGizmos();
            }
        }

        public static void GizmosDrawGizmos(this FPRigidbody2DEntity rb) {
            Color color = Color.yellow;
            FPVector2 pos = rb.TF.Pos;
            switch (rb.Shape) {
                case FPBoxShape2D box:
                    GizmosHelper.DrawPoint(pos, color);
                    //GizmosHelper.DrawCube(pos, box.Size, color);
                    var min = pos - box.Size / 2;
                    var max = pos + box.Size / 2;
                    GizmosHelper.DrawBox(min, max, color);
                    break;
                case FPCircleShape2D circle:
                    Gizmos.color = color;
                    Gizmos.DrawWireSphere(pos.ToVector2(), circle.Radius.AsFloat());
                    break;
                default:
                    throw new System.Exception($"未实现: {rb.Shape.GetType()}");
            }
        }
    
    }
}