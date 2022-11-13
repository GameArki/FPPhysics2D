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

            var boundsCenter = (aPos + bPos) * FP64.Half;
            var boundsSize = FPVector2.Abs(aPos - bPos);

            var bounds = new FPBounds2(boundsCenter, boundsSize);
            var candidates = repo.GetCandidatesByBounds(bounds);

            foreach (var rb in candidates) {
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
                    if (!useTriggers && rb.IsTrigger) {
                        continue;
                    }
                    if (useLayerMask && rb.Layer == layerMask) {
                        continue;
                    }
                    if (!containHolder && rb.ID == holderFBID) {
                        continue;
                    }
                    if (!containStatic && rb.IsStatic) {
                        continue;
                    }
                }

                bool isIntersect = Intersect2DUtil.IsIntersectRay_RB(aPos, bPos, rb, out FPVector2 intersectPoint, FP64.Epsilon);
                if (!isIntersect) {
                    continue;
                } else {
                    var hit = new RaycastHit2DArgs();
                    hit.isHit = true;
                    hit.rigidbody = rb;
                    hit.point = intersectPoint;
                    // TODO:根据入射角和平面返回2d法线向量
                    hit.normal = FPVector2.Zero;
                    hits[intersectNum] = hit;
                    result = true;
                    continue;
                }
            }
            return result;
        }

        public bool Raycast2DByDirection(in FPVector2 origin, in FPVector2 direction, in FP64 distance, FPContactFilter2DArgs contactFilter, RaycastHit2DArgs[] hits) {
            var end = origin + direction * distance;
            return Raycast2D(origin, end, contactFilter, hits);
        }

    }

}