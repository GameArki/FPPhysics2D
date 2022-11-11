using FixMath.NET;

namespace JackFrame.FPPhysics2D {

    public static class Intersect2DUtil {

        public static bool IsIntersectSegment_RB(FPVector2 aPos, FPVector2 bPos, in FPRigidbody2DEntity rb, out FPVector2 intersectPoint, in FP64 epsilon) {

            IShape2D rbShape = rb.Shape;
            intersectPoint = FPVector2.Zero;

            // Segment & Box
            FPBoxShape2D box = rbShape as FPBoxShape2D;
            if (box != null) {
                return IsIntersectSegment_Box(aPos, bPos, rb.TF, box, out intersectPoint, epsilon);
            }

            // Segment & Circle
            FPCircleShape2D circle = rbShape as FPCircleShape2D;
            if (circle != null) {
                return IsIntersectSegment_Circle(aPos, bPos, rb.TF, circle, out intersectPoint, epsilon);
            }

            return false;

        }

        public static bool IsIntersectRB_RB(in FPRigidbody2DEntity a, in FPRigidbody2DEntity b, in FP64 epsilon) {

            IShape2D shapeA = a.Shape;
            IShape2D shapeB = b.Shape;

            // Circle & Circle
            FPCircleShape2D aCircle = shapeA as FPCircleShape2D;
            FPCircleShape2D bCircle = shapeB as FPCircleShape2D;
            if (aCircle != null && bCircle != null) {
                return IsIntersectCircle_Circle(a.TF, aCircle, b.TF, bCircle, epsilon);
            }

            // Box & Box
            FPBoxShape2D aBox = shapeA as FPBoxShape2D;
            FPBoxShape2D bBox = shapeB as FPBoxShape2D;
            if (aBox != null && bBox != null) {
                return IsIntersectBox_Box(a.TF, aBox, b.TF, bBox, epsilon);
            }

            // Cirlce & Box
            if (aBox != null && bCircle != null) {
                return IsIntersectCircle_Box(b.TF, bCircle, a.TF, aBox, epsilon);
            } else if (bBox != null && aCircle != null) {
                return IsIntersectCircle_Box(a.TF, aCircle, b.TF, bBox, epsilon);
            }

            // throw new System.Exception("未处理");
            return false;

        }

        // ==== Box & Box ====
        static bool IsIntersectBox_Box(in FPTransform2D aTf, in FPBoxShape2D aBox, in FPTransform2D bTf, in FPBoxShape2D bBox, in FP64 epsilon) {

            if (aTf.RadAngle == FP64.Zero && bTf.RadAngle == FP64.Zero) {

                // AABB & AABB
                FPAABB2D a_aabb = aBox.GetAABB(aTf);
                FPAABB2D b_aabb = bBox.GetAABB(bTf);
                return IsIntersectAABB_AABB(a_aabb, b_aabb, epsilon);

            } else {

                // OBB & OBB
                // Mention: AABB here is a fake OBB now;
                FPOBB2D a_obb = aBox.GetOBB(aTf);
                FPOBB2D b_obb = bBox.GetOBB(bTf);
                return IsIntersectOBB_OBB(a_obb, b_obb, epsilon);

            }

        }

        // - AABB & AABB
        static bool IsIntersectAABB_AABB(in FPAABB2D a, in FPAABB2D b, in FP64 epsilon) {
            return ((b.Max.y - a.Min.y > epsilon)
                && (a.Max.y - b.Min.y > epsilon)
                && (b.Max.x - a.Min.x > epsilon)
                && (a.Max.x - b.Min.x > epsilon));
        }

        // - OBB & OBB
        static bool IsIntersectOBB_OBB(in FPOBB2D a, in FPOBB2D b, in FP64 epsilon) {
            bool notInAX = NotIntersectInAxis_OBB(a.Vertices, b.Vertices, a.AxisX, epsilon);
            bool notInAY = NotIntersectInAxis_OBB(a.Vertices, b.Vertices, a.AxisY, epsilon);
            bool notInBX = NotIntersectInAxis_OBB(a.Vertices, b.Vertices, b.AxisX, epsilon);
            bool notInBY = NotIntersectInAxis_OBB(a.Vertices, b.Vertices, b.AxisY, epsilon);
            return !(notInAX || notInAY || notInBX || notInBY);
        }

        static bool NotIntersectInAxis_OBB(FPVector2[] verticesA, FPVector2[] verticesB, in FPVector2 axis, in FP64 epsilon) {
            FPVector2 rangeA = Project_OBB(verticesA, axis);
            FPVector2 rangeB = Project_OBB(verticesB, axis);
            return (rangeA.x - rangeB.y > epsilon) || (rangeB.x - rangeA.y > epsilon);
        }

        static FPVector2 Project_OBB(FPVector2[] vertices, in FPVector2 axis) {
            // 四个顶点在分离轴上的投影
            // 取最大值与最小值
            FPVector2 range = new FPVector2(FP64.MaxValue, FP64.MinValue);
            for (int i = 0; i < vertices.Length; i += 1) {
                var vert = vertices[i];
                FP64 dot = FPVector2.Dot(vert, axis);
                range.x = MathHelper.Min(range.x, dot);
                range.y = MathHelper.Max(range.y, dot);
            }
            return range;
        }

        // ==== Circle & Circle ====
        static bool IsIntersectCircle_Circle(in FPTransform2D aTF, in FPCircleShape2D aCircle, in FPTransform2D bTF, in FPCircleShape2D bCircle, in FP64 epsilon) {
            FPVector2 ap = aTF.Pos;
            FPVector2 bp = bTF.Pos;
            var diff = ap - bp;
            var radius = aCircle.Radius + bCircle.Radius;
            return (radius * radius) - diff.LengthSquared() > epsilon;
        }

        // ==== Circle & Box ====
        static bool IsIntersectCircle_Box(in FPTransform2D circleTF, in FPCircleShape2D circle, in FPTransform2D boxTF, in FPBoxShape2D box, in FP64 epsilon) {
            FPSphere2D circle_sphere = new FPSphere2D(circleTF.Pos, circle.Radius);
            if (boxTF.RadAngle == FP64.Zero) {
                FPAABB2D box_aabb = box.GetAABB(boxTF);
                return IsIntersectCircle_AABB(circle_sphere, box_aabb, epsilon);
            } else {
                FPOBB2D box_obb = box.GetOBB(boxTF);
                return IsIntersectCircle_OBB(circle_sphere, box_obb, epsilon);
            }
        }

        static bool IsIntersectCircle_AABB(in FPSphere2D circle, in FPAABB2D aabb, in FP64 epsilon) {
            // 1. 以 AABB 为坐标系
            // 2. 圆心映射至坐标系中
            // 3. 取得两心矢量
            FPVector2 i = aabb.Center() - circle.Center;
            FPVector2 v = FPVector2.Max(i, -i);
            FPVector2 diff = FPVector2.Max(v - aabb.Size() * FP64.Half, FPVector2.Zero);
            return (circle.Radius * circle.Radius) - diff.LengthSquared() > epsilon;
        }

        static bool IsIntersectCircle_OBB(in FPSphere2D circle, in FPOBB2D obb, in FP64 epsilon) {
            // 1. OBB 转为 AABB 坐标系
            // 2. 与 Circle & AABB 相同
            FPVector2 i = obb.Center - circle.Center;
            i = FPMath2DUtil.MulRotAndPos(new FPRotation2D(-obb.RadAngle), i);
            FPVector2 v = FPVector2.Max(i, -i);
            FPVector2 diff = FPVector2.Max(v - obb.Size * FP64.Half, FPVector2.Zero);
            return (circle.Radius * circle.Radius) - diff.LengthSquared() > epsilon;
        }

        // ==== Ray & Segment ====
        static bool IsIntersectRay_Segment(FPVector2 aPos, FPVector2 bPos, FPVector2 cPos, FPVector2 dPos, out FPVector2 intersectPoint, in FP64 epsilon) {
            var n = (aPos.x - bPos.x) * (cPos.y - dPos.y) - (aPos.y - bPos.y) * (cPos.x - dPos.x);
            if (FP64.Abs(n) < FP64.Epsilon) {
                intersectPoint = FPVector2.Zero;
                return false;
            }
            var t = ((aPos.x - cPos.x) * (cPos.y - dPos.y) - (aPos.y - cPos.y) * (cPos.x - dPos.x)) / n;
            var u = ((aPos.x - cPos.x) * (aPos.y - bPos.y) - (aPos.y - cPos.y) * (aPos.x - bPos.x)) / n;
            if (t >= 0 && u <= 1 && u >= 0) {
                var x = aPos.x + t * (bPos.x - aPos.x);
                var y = aPos.y + t * (bPos.y - aPos.y);
                intersectPoint = new FPVector2(x, y);
                return true;
            } else {
                intersectPoint = FPVector2.Zero;
                return false;
            }
        }

        // ==== Segment & Segment ====
        static bool IsIntersectSegment_Segment(FPVector2 aPos, FPVector2 bPos, FPVector2 cPos, FPVector2 dPos, out FPVector2 intersectPoint, in FP64 epsilon) {
            var n = (aPos.x - bPos.x) * (cPos.y - dPos.y) - (aPos.y - bPos.y) * (cPos.x - dPos.x);
            if (FP64.Abs(n) < FP64.Epsilon) {
                intersectPoint = FPVector2.Zero;
                return false;
            }
            var t = ((aPos.x - cPos.x) * (cPos.y - dPos.y) - (aPos.y - cPos.y) * (cPos.x - dPos.x)) / n;
            var u = ((aPos.x - cPos.x) * (aPos.y - bPos.y) - (aPos.y - cPos.y) * (aPos.x - bPos.x)) / n;
            if (t >= 0 && t <= 1 && u >= 0 && u <= 1) {
                var x = aPos.x + t * (bPos.x - aPos.x);
                var y = aPos.y + t * (bPos.y - aPos.y);
                intersectPoint = new FPVector2(x, y);
                return true;
            } else {
                intersectPoint = FPVector2.Zero;
                return false;
            }
        }

        // ==== Segment & Box ====
        static bool IsIntersectSegment_Box(FPVector2 aPos, FPVector2 bPos, in FPTransform2D boxTf, in FPBoxShape2D box, out FPVector2 intersectPoint, in FP64 epsilon) {
            intersectPoint = FPVector2.Zero;
            var v = new FPVector2[4];

            if (boxTf.RadAngle == FP64.Zero) {

                // Segment & AABB
                FPAABB2D aabb = box.GetAABB(boxTf);
                v[0] = aabb.Min;
                v[1] = new FPVector2(aabb.Min.x, aabb.Max.y);
                v[2] = aabb.Max;
                v[3] = new FPVector2(aabb.Max.x, aabb.Min.y);

            } else {

                // Segment & OBB
                FPOBB2D obb = box.GetOBB(boxTf);
                var rot = new FPRotation2D(obb.RadAngle);
                var axisY = FPMath2DUtil.MulRotAndPos(rot, FPVector2.UnitY);
                var axisX = FPMath2DUtil.MulRotAndPos(rot, FPVector2.UnitX);
                var size = obb.Size;
                var center = obb.Center;
                FPVector2 half = size * FP64.Half;
                FPVector2 ax = axisX * half.x;
                FPVector2 ay = axisY * half.y;
                v[0] = center + (-ax + -ay);
                v[1] = center + (-ax + ay);
                v[2] = center + (ax + ay);
                v[3] = center + (ax + -ay);

            }
            var intersectCount = 0;
            var intersectPoints = new FPVector2[2];
            for (int i = 0; i < v.Length; i++) {
                int j = (i + 1) % v.Length;
                if (IsIntersectSegment_Segment(aPos, bPos, v[i], v[j], out intersectPoint, in epsilon)) {
                    intersectPoints[intersectCount] = intersectPoint;
                    intersectCount += 1;
                }
            }

            if (intersectCount == 0) {
                return false;
            } else if (intersectCount == 1) {
                intersectPoint = intersectPoints[0];
                return true;
            } else {
                // 2个交点，取距离较近的一个
                var d0 = (intersectPoints[0] - aPos).LengthSquared();
                var d1 = (intersectPoints[1] - aPos).LengthSquared();
                if (d0 < d1) {
                    intersectPoint = intersectPoints[0];
                } else {
                    intersectPoint = intersectPoints[1];
                }
                return true;
            }
        }

        // ==== Segment & Circle ====
        static bool IsIntersectSegment_Circle(FPVector2 aPos, FPVector2 bPos, in FPTransform2D circleTF, in FPCircleShape2D circle, out FPVector2 intersectPoint, in FP64 epsilon) {
            System.Console.WriteLine("线段坐标:" + aPos + "," + bPos + "; 圆心坐标:" + circleTF.Pos + "; 半径:" + circle.Radius);
            intersectPoint = FPVector2.Zero;
            FPSphere2D circle_sphere = new FPSphere2D(circleTF.Pos, circle.Radius);
            var direction = (bPos - aPos) / (bPos - aPos).Length();
            // 粗筛: 射线检测
            bool isIntersect = IsIntersectRay_Circle(aPos, direction, circle_sphere.Center, circle_sphere.Radius, out var intersectPoint1, out var intersectPoint2, in epsilon);
            // 射线没有交点
            if (!isIntersect) {
                intersectPoint = FPVector2.Zero;
                return false;
            }
            // 射线存在一个交点
            if (intersectPoint1 == intersectPoint2) {
                intersectPoint = intersectPoint1;
                return true;
            }
            // 射线存在两个交点，检查该两点是否落在线段上
            var abLengthSquared = (bPos - aPos).LengthSquared();
            var d1Squared = (intersectPoint1 - aPos).LengthSquared();
            var d2Squared = (intersectPoint2 - aPos).LengthSquared();
            var intersectCount = 0;
            var _intersectPoint = FPVector2.Zero;
            if (d1Squared <= abLengthSquared) {
                _intersectPoint = intersectPoint1;
                intersectCount += 1;
            }
            if (d2Squared <= abLengthSquared) {
                _intersectPoint = intersectPoint2;
                intersectCount += 1;
            }
            // 没有点落在线段上
            if (intersectCount == 0) {
                intersectPoint = FPVector2.Zero;
                return false;
            }
            // 只有一个点落在线段上
            if (intersectCount == 1) {
                System.Console.WriteLine("和圆产生碰撞,存在1个交点");
                intersectPoint = _intersectPoint;
                return true;
            }
            // 两个点都落在线段上
            // 返回距离a点最近的交点
            if (intersectCount == 2) {
                if (d1Squared < d2Squared) {
                    intersectPoint = intersectPoint1;
                } else {
                    intersectPoint = intersectPoint2;
                }
                System.Console.WriteLine("和圆产生碰撞,存在2个交点");
                return true;
            }
            return false;
        }

        static bool IsIntersectRay_Circle(FPVector2 aPos, FPVector2 direction, FPVector2 center, FP64 radius, out FPVector2 intersectPoint1, out FPVector2 intersectPoint2, in FP64 epsilon) {

            intersectPoint1 = FPVector2.Zero;
            intersectPoint2 = FPVector2.Zero;
            var ac = center - aPos;
            // 向量点乘以单位向量,得到投影长度
            var adLength = FPVector2.Dot(ac, direction);
            System.Console.WriteLine("adLength=" + adLength);
            // adLength < 0, 射线起点在圆心后面; adLength > 0, 射线起点在圆心前面
            var acLengthSquared = FPVector2.Dot(ac, ac);
            var cdLengthSquared = acLengthSquared - adLength * adLength;
            if (cdLengthSquared < 0) {
                return false;
            }
            var diLengthSquared = radius * radius - cdLengthSquared;
            if (diLengthSquared < 0) {
                return false;
            }
            // 投影点到交点的距离
            var diLength = FP64.Sqrt(diLengthSquared);
            // 一个交点(位于切线外接点)

            if (diLength == epsilon) {
                intersectPoint1 = aPos + direction * adLength;
                intersectPoint2 = intersectPoint1;
                return true;
            }

            // t1是射线起点到交点1的距离,有可能是负数
            // 交点1是距离射线起点最近的交点
            FP64 t1 = 0;
            FP64 t2 = 0;
            t1 = adLength - diLength;
            t2 = adLength + diLength;

            if ((t1 > 0) || (t1 * t2 <= 0)) {
                intersectPoint1 = aPos + direction * t1;
                intersectPoint2 = aPos + direction * t2;
                System.Console.WriteLine("射线碰撞");
                return true;
            }
            return false;

        }
    }
}