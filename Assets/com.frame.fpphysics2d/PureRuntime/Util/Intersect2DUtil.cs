using FixMath.NET;

namespace JackFrame.FPPhysics2D {

    public static class Intersect2DUtil {

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

    }
}