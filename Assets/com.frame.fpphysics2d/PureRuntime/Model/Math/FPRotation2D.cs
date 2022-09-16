using FixMath.NET;

namespace JackFrame.FPPhysics2D {

    public struct FPRotation2D {

        FP64 radAngle;
        public FP64 RadAngle => radAngle;

        FP64 sinValue;
        public FP64 SinValue => sinValue;

        FP64 cosValue;
        public FP64 CosValue => cosValue;

        public FPRotation2D(in FP64 radAngle) {
            this.radAngle = radAngle;
            sinValue = FP64.Sin(radAngle);
            cosValue = FP64.Cos(radAngle);
        }

    }

}