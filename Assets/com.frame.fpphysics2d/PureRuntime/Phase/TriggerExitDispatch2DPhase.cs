namespace JackFrame.FPPhysics2D {

    internal class TriggerExitDispatch2DPhase {

        FPContext2D context;

        public TriggerExitDispatch2DPhase() { }

        internal void Inject(FPContext2D context) {
            this.context = context;
        }

        internal void Tick() {
            var triggerEventCenter = context.TriggerEventCenter;
            while (triggerEventCenter.TryDequeueExit(out var ev)) {
                ApplyTriggerExit(ev);
            }
        }

        void ApplyTriggerExit(in InternalTrigger2DEventModel ev) {
            var a = ev.A;
            var b = ev.B;
            if (a.IsTrigger) {
                a.OnTriggerExit(new Trigger2DEventModel(b));
            }
            if (b.IsTrigger) {
                b.OnTriggerExit(new Trigger2DEventModel(a));
            }

            // Public Trigger
            if (a.IsTrigger || b.IsTrigger) {
                var events = context.Events;
                events.TriggerExit(new API.TriggerEventArgs(a, b));
            }

            // Remove From Collision
            var collisionRepo = context.CollisionContactRepo;
            var collisionEventCenter = context.CollisionEventCenter;
            ulong key = DictionaryKeyUtil.ComputeRBKey(a, b);
            if (collisionRepo.Remove(key)) {
                collisionEventCenter.EnqueueExit(new InternalCollision2DEventModel(a, b));
            }

        }

    }
}