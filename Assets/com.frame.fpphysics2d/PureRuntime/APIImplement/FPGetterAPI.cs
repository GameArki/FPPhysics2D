using System;
using FixMath.NET;

namespace JackFrame.FPPhysics2D.API {

    public class FPGetterAPI : IFPGetterAPI {

        FPContext2D context;
        internal void Inject(FPContext2D context) {
            this.context = context;
        }

        // - Segmentcast (All Raycast is Segmentcast)
        public bool Segmentcast2D(FPVector2 origin, FPVector2 end, FPContactFilter2DArgs contactFilter, RaycastHit2DArgs[] hits) {

            var aPos = origin;
            var bPos = end;
            var result = false;

            var repo = context.RBRepo;
            // 遍历效率待优化
            repo.Foreach(value => {
                var intersectNum = 0;

                // 过滤
                var isFiltering = contactFilter.isFiltering;
                var useTriggers = contactFilter.useTriggers;
                var useLayerMask = contactFilter.useLayerMask;
                var containHolder = contactFilter.containHolder;
                var containStatic = contactFilter.containStatic;

                var holderFBID = contactFilter.holderFBID;
                var layerMask = contactFilter.layerMask;

                if (isFiltering) {
                    if (!useTriggers && value.IsTrigger) {
                        return;
                    }
                    if (useLayerMask && value.Layer == layerMask) {
                        return;
                    }
                    if (!containHolder && value.ID == holderFBID) {
                        return;
                    }
                    if (!containStatic && value.IsStatic) {
                        return;
                    }
                }

                bool isIntersect = Intersect2DUtil.IsIntersectSegment_RB(aPos, bPos, value, out FPVector2 intersectPoint, FP64.Epsilon);
                if (!isIntersect) {
                    return;
                } else {
                    var hit = new RaycastHit2DArgs();
                    hit.isHit = true;
                    hit.rigidbody = value;
                    hit.point = intersectPoint;
                    // TODO:根据入射角和平面返回2d法线向量
                    hit.normal = FPVector2.Zero;
                    hits[intersectNum] = hit;
                    result = true;
                    return;
                }
            });
            return result;
        }
        // - Raycast (direction参数目前只支持单位向量)
        public bool Raycast2D(FPVector2 origin, FPVector2 direction, FP64 distance, FPContactFilter2DArgs contactFilter, RaycastHit2DArgs[] hits) {
            var end = origin + direction * distance;
            return Segmentcast2D(origin, end, contactFilter, hits);
        }

    }

}