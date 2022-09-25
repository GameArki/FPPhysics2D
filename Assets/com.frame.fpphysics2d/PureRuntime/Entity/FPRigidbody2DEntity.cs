using System;
using FixMath.NET;

namespace JackFrame.FPPhysics2D {

    public class FPRigidbody2DEntity {

        static uint id_record;
        uint id;
        public uint ID => id;

        // ==== Holder ====
        // 此处后续需要写个文档来说明用途
        // 总之:
        //      holderPtr 就是作为一个指针使用
        //      holderType 用于判断将这个指针强转成哪个上层的类型
        public int holderType;
        public object holderPtr;

        // ==== Transform ====
        FPTransform2D tf;
        public FPTransform2D TF => tf;

        // ==== Shape ====
        IShape2D shape;
        public IShape2D Shape => shape;

        // ==== Material ====
        FPMaterial2DModel material;
        public FPMaterial2DModel Material => material;

        // ==== Linear ====
        bool isTrigger;
        public bool IsTrigger => isTrigger;

        bool isStatic;
        public bool IsStatic => isStatic;

        FPVector2 linearVelocity;
        public FPVector2 LinearVelocity => linearVelocity;

        FP64 angularVelocity;
        public FP64 AngularVelocity => angularVelocity;

        FP64 gravityScale;
        public FP64 GravityScale => gravityScale;

        // ==== Event ====
        // - Trigger
        public event Action<Trigger2DEventModel> OnTriggerEnterHandle;
        public event Action<Trigger2DEventModel> OnTriggerStayHandle;
        public event Action<Trigger2DEventModel> OnTriggerExitHandle;

        // - Collision
        public event Action<Collision2DEventModel> OnCollisionEnterHandle;
        public event Action<Collision2DEventModel> OnCollisionStayHandle;
        public event Action<Collision2DEventModel> OnCollisionExitHandle;

        internal FPRigidbody2DEntity(in FPVector2 pos, in FP64 radAngle, IShape2D shape) {

            id_record += 1;
            this.id = id_record;
            
            this.tf = new FPTransform2D(pos, radAngle);
            
            this.shape = shape;

            this.material = new FPMaterial2DModel();

            this.linearVelocity = FPVector2.Zero;

            this.angularVelocity = FP64.Zero;

            this.gravityScale = FP64.One;

            this.isTrigger = false;
            this.isStatic = false;

        }

        // ==== Holder ====
        public void SetHolder(object holderPtr, int holderType) {
            this.holderPtr = holderPtr;
            this.holderType = holderType;
        }

        // ==== Transform ====
        public void SetPos(in FPVector2 pos) {
            tf.SetPos(pos);
        }

        public void SetLocalTR(in FPVector2 localPos, in FP64 localRadAngle) {
            tf.SetLocalPos(localPos);
            tf.SetLocalRadianAngle(localRadAngle);
        }

        public void UpdateByParentTransform(FPTransform2D parent, bool isFlipX = false) {

            // FIXME: isFlipX 应当同时改变碰撞盒的反转

            // 1. Rotate
            tf.SetRot(parent.Rot * tf.LocalRot);

            // 2. Translate
            if (!isFlipX) {
                tf.SetPos(parent.Pos + tf.Rot * tf.LocalPos);
            } else {
                tf.SetPos(parent.Pos - tf.Rot * tf.LocalPos);
            }
        }

        public void SetTR(in FPVector2 pos, in FP64 radAngle) {
            tf.SetPos(pos);
            tf.SetRadianAngle(radAngle);
        }

        public void SetRotDegreeAngle(in FP64 degAngle) {
            tf.SetRadianAngle(degAngle * FP64.Deg2Rad);
        }

        public void SetRotRadianAngle(in FP64 radAngle) {
            tf.SetRadianAngle(radAngle);
        }

        // ==== Linear ====
        public void SetTrigger(bool isTrigger) {
            this.isTrigger = isTrigger;
        }

        public void SetStatic(bool isStatic) {
            this.isStatic = isStatic;
        }

        public void SetLinearVelocity(in FPVector2 velo) {
            this.linearVelocity = velo;
        }

        public void SetAngularVelocity(in FP64 velo) {
            this.angularVelocity = velo;
        }

        public void SetGravityScale(FP64 scale) {
            this.gravityScale = scale;
        }

        // ==== Event ====
        // - Trigger
        internal void OnTriggerEnter(Trigger2DEventModel ev) {
            OnTriggerEnterHandle?.Invoke(ev);
        }

        internal void OnTriggerStay(Trigger2DEventModel ev) {
            OnTriggerStayHandle?.Invoke(ev);
        }

        internal void OnTriggerExit(Trigger2DEventModel ev) {
            OnTriggerExitHandle?.Invoke(ev);
        }

        // - Collision
        internal void OnCollisionEnter(Collision2DEventModel ev) {
            OnCollisionEnterHandle?.Invoke(ev);
        }

        internal void OnCollisionStay(Collision2DEventModel ev) {
            OnCollisionStayHandle?.Invoke(ev);
        }

        internal void OnCollisionExit(Collision2DEventModel ev) {
            OnCollisionExitHandle?.Invoke(ev);
        }

    }

}