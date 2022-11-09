using System;
using FixMath.NET;

namespace JackFrame.FPPhysics2D.API {

    internal class FPGetterAPI : IFPGetterAPI {

        FPContext2D context;
        internal void Inject(FPContext2D context) {
            this.context = context;
        }

        // - Raycast
        public RaycastHit2DArgs Segmentcast2D(FPVector2 origin, FPVector2 end, FPContactFilter2DArgs contactFilter) {

            var result = new RaycastHit2DArgs();
            result.isHit = false;
            FPVector2 aPos = origin;
            FPVector2 bPos = end;

            var repo = context.RBRepo;
            // 遍历方式待优化
            repo.Foreach(value => {
                var shapeType = value.Shape.shapeType;
                if (shapeType == FPCollider2DType.None) {
                    return;
                }
                if (shapeType == FPCollider2DType.AABB || shapeType == FPCollider2DType.OBB) {
                    var shape = value.Shape as FPBoxShape2D;
                    var tf = value.TF;
                    if (Intersect2DUtil.IsIntersect_Segment_Box(aPos, bPos, tf, shape, out FPVector2 intersectPoint, FP64.Epsilon)) {
                        result.isHit = true;
                        result.rigidbody = value;
                        result.point = intersectPoint;
                        // TODO:根据入射角和平面返回2d法线向量
                        result.normal = FPVector2.Zero;
                        return;
                    }
                }
                if (shapeType == FPCollider2DType.Circle) {
                    var shape = value.Shape as FPCircleShape2D;
                    var tf = value.TF;
                    if (Intersect2DUtil.IsIntersect_Segment_Circle(aPos, bPos, tf, shape, out FPVector2 intersectPoint, FP64.Epsilon)) {
                        result.isHit = true;
                        result.rigidbody = value;
                        result.point = intersectPoint;
                        // TODO:根据入射角和平面返回2d法线向量
                        result.normal = FPVector2.Zero;
                        return;
                    }
                }
                if (shapeType == FPCollider2DType.Capsule) {
                }
                if (shapeType == FPCollider2DType.Polygon) {
                }
            });
            return result;
        }
        // TODO
        public RaycastHit2DArgs Raycast2D(FPVector2 origin, FPVector2 direction, FP64 distance, FPContactFilter2DArgs contactFilter) {
            var result = new RaycastHit2DArgs();
            result.isHit = false;
            return result;

        }

    }

}