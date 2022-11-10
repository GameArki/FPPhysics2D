using FixMath.NET;

namespace JackFrame.FPPhysics2D {

    public static class Intersect2DUtil {

        public static bool IsIntersect_Segment_RB(FPVector2 aPos, FPVector2 bPos, in FPRigidbody2DEntity rb, out FPVector2 intersectPoint, in FP64 epsilon) {

            IShape2D rbShape = rb.Shape;
            intersectPoint = FPVector2.Zero;

            // Ray & Box
            FPBoxShape2D box = rbShape as FPBoxShape2D;
            if (box != null) {
                return IsIntersect_Segment_Box(aPos, bPos, rb.TF, box, out intersectPoint, epsilon);
            }

            // Ray & Circle
            FPCircleShape2D circle = rbShape as FPCircleShape2D;
            if (circle != null) {
                return IsIntersect_Segment_Circle(aPos, bPos, rb.TF, circle, out intersectPoint, epsilon);
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
        public static bool IsIntersect_Ray_Segment(FPVector2 aPos, FPVector2 bPos, FPVector2 cPos, FPVector2 dPos, out FPVector2 intersectPoint) {
            var n = (aPos.x - bPos.x) * (cPos.y - dPos.y) - (aPos.y - bPos.y) * (cPos.x - dPos.x);
            if (n == 0) {
                intersectPoint = FPVector2.Zero;
                return false;
            }
            var t = ((aPos.x - cPos.x) * (cPos.y - dPos.y) - (aPos.y - cPos.y) * (cPos.x - dPos.x)) / n;
            var u = ((aPos.x - cPos.x) * (aPos.y - bPos.y) - (aPos.y - cPos.y) * (aPos.x - bPos.x)) / n;
            if (t >= 0 && t <= 1 && u >= 0) {
                var x = aPos.x + t * (bPos.x - aPos.x);
                var y = aPos.y + t * (bPos.y - aPos.y);
                intersectPoint = new FPVector2((int)x, (int)y);
                return true;
            } else {
                intersectPoint = FPVector2.Zero;
                return false;
            }
        }

        // ==== Segment & Segment ====
        public static bool IsIntersect_Segment_Segment(FPVector2 aPos, FPVector2 bPos, FPVector2 cPos, FPVector2 dPos, out FPVector2 intersectPoint) {
            var n = (aPos.x - bPos.x) * (cPos.y - dPos.y) - (aPos.y - bPos.y) * (cPos.x - dPos.x);
            if (n == 0) {
                intersectPoint = FPVector2.Zero;
                return false;
            }
            var t = ((aPos.x - cPos.x) * (cPos.y - dPos.y) - (aPos.y - cPos.y) * (cPos.x - dPos.x)) / n;
            var u = ((aPos.x - cPos.x) * (aPos.y - bPos.y) - (aPos.y - cPos.y) * (aPos.x - bPos.x)) / n;
            if (t >= 0 && t <= 1 && u >= 0 && u <= 1) {
                var x = aPos.x + t * (bPos.x - aPos.x);
                var y = aPos.y + t * (bPos.y - aPos.y);
                intersectPoint = new FPVector2((int)x, (int)y);
                return true;
            } else {
                intersectPoint = FPVector2.Zero;
                return false;
            }
        }

        // ==== Segment & Box ====
        public static bool IsIntersect_Segment_Box(FPVector2 aPos, FPVector2 bPos, in FPTransform2D boxTf, in FPBoxShape2D box, out FPVector2 intersectPoint, in FP64 epsilon) {
            intersectPoint = FPVector2.Zero;
            if (boxTf.RadAngle == FP64.Zero) {

                // Segment & AABB
                FPAABB2D aabb = box.GetAABB(boxTf);
                var u = aabb.Min;
                var v = new FPVector2(aabb.Min.x, aabb.Max.y);
                var w = aabb.Max;
                var x = new FPVector2(aabb.Max.x, aabb.Min.y);
                bool isIntersect = IsIntersect_Segment_Segment(aPos, bPos, u, v, out var intersectPoint1);
                var intersectCount = 0;
                var _intersectPoint = FPVector2.Zero;
                if (isIntersect) {
                    intersectCount += 1;
                    _intersectPoint = intersectPoint1;
                }
                isIntersect = IsIntersect_Segment_Segment(aPos, bPos, v, w, out var intersectPoint2);
                if (isIntersect) {
                    intersectCount += 1;
                    _intersectPoint = intersectPoint2;
                }
                isIntersect = IsIntersect_Segment_Segment(aPos, bPos, w, x, out var intersectPoint3);
                if (isIntersect) {
                    intersectCount += 1;
                    _intersectPoint = intersectPoint3;
                }
                isIntersect = IsIntersect_Segment_Segment(aPos, bPos, x, u, out var intersectPoint4);
                if (isIntersect) {
                    intersectCount += 1;
                    _intersectPoint = intersectPoint4;
                }
                // 线段没有交点
                if (intersectCount == 0) {
                    intersectPoint = FPVector2.Zero;
                    return false;
                }
                // 线段存在一个交点
                if (intersectCount == 1) {
                    intersectPoint = _intersectPoint;
                    System.Console.WriteLine("和AABB产生碰撞");
                    return true;
                }
                // 线段存在两个交点
                // 返回距离a点最近的交点
                if (intersectCount > 1) {
                    // TODO: 返回最近的点
                    intersectPoint = _intersectPoint;
                    System.Console.WriteLine("和AABB产生碰撞");
                    return true;
                }
                return false;

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
                var u = center + (-ax + -ay);
                var v = center + (-ax + ay);
                var w = center + (ax + ay);
                var x = center + (ax + -ay);
                var intersectCount = 0;
                var _intersectPoint = FPVector2.Zero;

                bool isIntersect = IsIntersect_Segment_Segment(aPos, bPos, u, v, out var intersectPoint1);
                if (isIntersect) {
                    intersectCount += 1;
                    _intersectPoint = intersectPoint1;
                }
                isIntersect = IsIntersect_Segment_Segment(aPos, bPos, v, w, out var intersectPoint2);
                if (isIntersect) {
                    intersectCount += 1;
                    _intersectPoint = intersectPoint2;
                }
                isIntersect = IsIntersect_Segment_Segment(aPos, bPos, w, x, out var intersectPoint3);
                if (isIntersect) {
                    intersectCount += 1;
                    _intersectPoint = intersectPoint3;
                }
                isIntersect = IsIntersect_Segment_Segment(aPos, bPos, x, u, out var intersectPoint4);
                if (isIntersect) {
                    intersectCount += 1;
                    _intersectPoint = intersectPoint4;
                }

                // 线段没有交点
                if (intersectCount == 0) {
                    intersectPoint = FPVector2.Zero;
                    return false;
                }
                // 线段存在一个交点
                if (intersectCount == 1) {
                    intersectPoint = _intersectPoint;
                    System.Console.WriteLine("和OBB产生碰撞");
                    return true;
                }
                // 线段存在两个交点
                // 返回距离a点最近的交点
                if (intersectCount == 2) {
                    // TODO: 返回最近的点
                    intersectPoint = _intersectPoint;
                    System.Console.WriteLine("和OBB产生碰撞");
                    return true;
                }
                return false;

            }

        }

        // ==== Segment & Circle ====
        static bool IsIntersect_Segment_Circle(FPVector2 aPos, FPVector2 bPos, in FPTransform2D circleTF, in FPCircleShape2D circle, out FPVector2 intersectPoint, in FP64 epsilon) {
            intersectPoint = FPVector2.Zero;
            FPSphere2D circle_sphere = new FPSphere2D(circleTF.Pos, circle.Radius);
            bool isIntersect = IsIntersect_Ray_Circle(aPos, bPos - aPos, circle_sphere, out var intersectPoint1, out var intersectPoint2);
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
            // 射线存在两个交点
            var oaLength = (aPos - circle_sphere.Center).Length();
            var obLength = (bPos - circle_sphere.Center).Length();

            var abLength = (bPos - aPos).Length();
            var d1 = (intersectPoint1 - aPos).Length();
            var d2 = (intersectPoint2 - aPos).Length();
            var intersectCount = 0;
            var _intersectPoint = FPVector2.Zero;
            if (d1 <= abLength) {
                intersectCount += 1;
                _intersectPoint = intersectPoint1;
            }
            if (d2 <= abLength) {
                intersectCount += 1;
                _intersectPoint = intersectPoint1;
            }
            // 线段没有交点
            if (intersectCount == 0) {
                intersectPoint = FPVector2.Zero;
                return false;
            }
            // 线段存在一个交点
            if (intersectCount == 1) {
                System.Console.WriteLine("和圆产生碰撞");
                intersectPoint = _intersectPoint;
                return true;
            }
            // 线段存在两个交点
            // 返回距离a点最近的交点
            if (intersectCount == 2) {
                if (d1 < d2) {
                    intersectPoint = intersectPoint1;
                } else {
                    intersectPoint = intersectPoint2;
                }
                System.Console.WriteLine("和圆产生碰撞");
                return true;
            }
            return false;
        }
        static bool IsIntersect_Ray_Circle(FPVector2 aPos, FPVector2 bPos, in FPTransform2D circleTF, in FPCircleShape2D circle, out FPVector2 intersectPoint) {
            FPSphere2D circle_sphere = new FPSphere2D(circleTF.Pos, circle.Radius);
            bool isIntersect = IsIntersect_Ray_Circle(aPos, bPos, circle_sphere, out var intersectPoint1, out var intersectPoint2);
            if (!isIntersect) {
                intersectPoint = FPVector2.Zero;
                return false;
            }
            if (intersectPoint1 == intersectPoint2) {
                intersectPoint = intersectPoint1;
                return true;
            }
            // 返回距离a点最近的交点
            var d1 = (intersectPoint1 - aPos).LengthSquared();
            var d2 = (intersectPoint2 - aPos).LengthSquared();
            if (d1 < d2) {
                intersectPoint = intersectPoint1;
            } else {
                intersectPoint = intersectPoint2;
            }
            return true;
        }
        static bool IsIntersect_Ray_Circle(FPVector2 aPos, FPVector2 bPos, in FPSphere2D circle, out FPVector2 intersectPoint1, out FPVector2 intersectPoint2) {
            intersectPoint1 = FPVector2.Zero;
            intersectPoint2 = FPVector2.Zero;
            var ab = bPos - aPos;
            var ac = circle.Center - aPos;
            // 单位向量
            var direction = ab / ab.Length();
            // 向量点乘以单位向量,得到投影长度
            var adLength = FPVector2.Dot(ac, ab);
            var acLengthSquared = FPVector2.Dot(ac, ac);
            var cdLengthSquared = acLengthSquared - adLength * adLength;
            var diLengthSquared = circle.Radius * circle.Radius - cdLengthSquared;
            if (diLengthSquared < 0) {
                return false;
            }
            // 投影点到交点的距离
            var diLength = FP64.Sqrt(diLengthSquared);
            var t1 = adLength - diLength;
            var t2 = adLength + diLength;
            if (t1 >= 0) {
                intersectPoint1 = aPos + direction * t1;
                intersectPoint2 = aPos + direction * t2;
                return true;
            }
            return false;

        }
    }
}