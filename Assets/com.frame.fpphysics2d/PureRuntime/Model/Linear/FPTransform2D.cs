using FixMath.NET;

namespace JackFrame.FPPhysics2D {

    public class FPTransform2D {

        FPVector2 pos;
        public FPVector2 Pos => pos;

        FPRotation2D rot;
        public FPRotation2D Rot => rot;

        FP64 radAngle;
        public FP64 RadAngle => radAngle;

        public FPTransform2D(in FPVector2 pos, in FP64 radAngle) {
            this.pos = pos;
            this.radAngle = radAngle;
            this.rot = new FPRotation2D(radAngle);
        }

        public void SetPos(in FPVector2 pos) {
            this.pos = pos;
        }

        public void SetRadianAngle(in FP64 angle) {
            this.radAngle = angle;
            this.rot = new FPRotation2D(angle);
        }

    }

}