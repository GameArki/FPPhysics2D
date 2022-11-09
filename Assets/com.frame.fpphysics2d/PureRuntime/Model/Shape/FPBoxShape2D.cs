using FixMath.NET;

namespace JackFrame.FPPhysics2D {

    public class FPBoxShape2D : IShape2D {

        FPVector2 size;
        public FPVector2 Size => size;
        FPCollider2DType shapeType;
        FPCollider2DType IShape2D.shapeType => shapeType;

        FP64 radius;
        public FP64 Radius => radius;

        internal FPBoxShape2D(in FPVector2 size) {

            this.size = size;
            this.radius = MathHelper.Max(size.x, size.y) * FP64.Half;

        }

        internal FPAABB2D GetAABB(in FPTransform2D tf) {
            shapeType = FPCollider2DType.AABB;
            return new FPAABB2D(tf.Pos, size);
        }

        internal FPOBB2D GetOBB(in FPTransform2D tf) {
            shapeType = FPCollider2DType.OBB;
            return new FPOBB2D(tf.Pos, size, tf.RadAngle);
        }

        public void SetSize(in FPVector2 size) {
            this.size = size;
            this.radius = MathHelper.Max(size.x, size.y);
        }

    }

}