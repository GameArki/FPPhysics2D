namespace JackFrame.FPPhysics2D {

    internal class CollisionEnterAndStayDispatch2DPhase {

        FPContext2D context;

        internal CollisionEnterAndStayDispatch2DPhase() { }

        internal void Inject(FPContext2D context) {
            this.context = context;
        }

        internal void Tick() {
            var collisionEventCenter = context.CollisionEventCenter;
            while (collisionEventCenter.TryDequeueEnter(out var ev)) {
                ApplyCollisionEnter(ev);
            }

            while (collisionEventCenter.TryDequeueStay(out var ev)) {
                ApplyCollisionStay(ev);
            }
        }

        void ApplyCollisionEnter(in InternalCollision2DEventModel ev) {
            var a = ev.A;
            var b = ev.B;
            a.OnCollisionEnter(new Collision2DEventModel(b));
            b.OnCollisionEnter(new Collision2DEventModel(a));
        }

        void ApplyCollisionStay(in InternalCollision2DEventModel ev) {
            var a = ev.A;
            var b = ev.B;
            a.OnCollisionStay(new Collision2DEventModel(b));
            b.OnCollisionStay(new Collision2DEventModel(a));
        }

    }

}