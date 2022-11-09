using FixMath.NET;

namespace JackFrame.FPPhysics2D.API {

    public struct RaycastHit2DArgs {

        public bool isHit;
        public FPRigidbody2DEntity rigidbody;
        public FPVector2 point;
        public FPVector2 normal;
        public FPVector2 direction;
        public FP64 distance;
        public FP64 fraction; // 原点和命中点距离之和与射线方向矢量之比。当射线的方向是单位向量，则此值等于原点和命中点之间的距离

    }

}