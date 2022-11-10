using System;
using FixMath.NET;

namespace JackFrame.FPPhysics2D.API {

    public class FPGetterAPI : IFPGetterAPI {

        FPContext2D context;
        internal void Inject(FPContext2D context) {
            this.context = context;
        }

        // - Raycast
        public bool Segmentcast2D(FPVector2 origin, FPVector2 end, FPContactFilter2DArgs contactFilter, ref RaycastHit2DArgs[] hits) {

            var aPos = origin;
            var bPos = end;
            var result = false;
            var _hits = hits;

            var repo = context.RBRepo;
            // 遍历方式待优化
            repo.Foreach(value => {
                var intersectNum = 0;
                bool isIntersect = Intersect2DUtil.IsIntersect_Segment_RB(aPos, bPos, value, out FPVector2 intersectPoint, FP64.Epsilon);
                if (!isIntersect) {
                    return;
                } else {
                    var hit = new RaycastHit2DArgs();
                    hit.isHit = true;
                    hit.rigidbody = value;
                    hit.point = intersectPoint;
                    // TODO:根据入射角和平面返回2d法线向量
                    hit.normal = FPVector2.Zero;
                    _hits[intersectNum] = hit;
                    result = true;
                    System.Console.WriteLine("产生碰撞");
                    return;
                }
            });
            hits = _hits;
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