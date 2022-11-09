using FixMath.NET;

namespace JackFrame.FPPhysics2D {

    public class FPCircleShape2D : IShape2D {

        FP64 radius;
        public FP64 Radius => radius;
        FPCollider2DType IShape2D.shapeType => FPCollider2DType.Circle;

        public FPCircleShape2D(in FP64 radius) {
            this.radius = radius;
        }

        public bool IsValid() {
            return radius > FP64.Zero;
        }

        public bool IsInside(in FPVector2 centerPos, in FPVector2 otherPoint) {
            FPVector2 diff = centerPos - otherPoint;
            if (diff.LengthSquared() <= (radius * radius)) {
                return true;
            } else {
                return false;
            }
        }

    }

}