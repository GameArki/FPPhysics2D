using System;
using FixMath.NET;

namespace JackFrame.FPPhysics2D.API {

    public class FPGetterAPI : IFPGetterAPI {

        FPContext2D context;
        internal void Inject(FPContext2D context) {
            this.context = context;
        }

        // - Raycast 
        // TODO: 针对不传hits时的优化
        public bool Raycast2D(in FPVector2 origin, in FPVector2 end, FPContactFilter2DArgs contactFilter, RaycastHit2DArgs[] hits) {

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

                var holderFBID = contactFilter.holderRBID;
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

                bool isIntersect = Intersect2DUtil.IsIntersectRay_RB(aPos, bPos, value, out FPVector2 intersectPoint, FP64.Epsilon);
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

        public bool Raycast2DByDirection(in FPVector2 origin, in FPVector2 direction, in FP64 distance, FPContactFilter2DArgs contactFilter, RaycastHit2DArgs[] hits) {
            var end = origin + direction * distance;
            return Raycast2D(origin, end, contactFilter, hits);
        }

    }

}