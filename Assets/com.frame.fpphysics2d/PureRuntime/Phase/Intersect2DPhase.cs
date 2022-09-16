using FixMath.NET;

namespace JackFrame.FPPhysics2D.Phases {

    internal class Intersect2DPhase {

        FPContext2D context;

        internal Intersect2DPhase() { }

        internal void Inject(FPContext2D context) {
            this.context = context;
        }

        internal void Tick(in FP64 step) {

            var arr = context.RBRepo.GetArray();
            for (int i = 0; i < arr.Length; i += 1) {
                FPRigidbody2DEntity cur = arr[i];
                if (cur.IsStatic) {
                    continue;
                }
                for (int j = 0; j < arr.Length; j += 1) {
                    if (i == j) continue;
                    FPRigidbody2DEntity tar = arr[j];
                    ApplyIntersection(cur, tar);
                }
            }

        }

        void ApplyIntersection(FPRigidbody2DEntity a, FPRigidbody2DEntity b) {

            var contactRepo = context.IntersectContactRepo;
            var pruneIgnoreContantRepo = context.PruneIgnoreContactRepo;
            var triggerEventCenter = context.TriggerEventCenter;
            var key = DictionaryKeyUtil.ComputeRBKey(a, b);

            if (contactRepo.TryGet(key, out var model)) {

                // Trigger Stay
                triggerEventCenter.EnqueueStay(new InternalTrigger2DEventModel(a, b));

                // Trigger Exit
                bool isIgnore = pruneIgnoreContantRepo.Contains(key);
                if (isIgnore) {
                    TriggerExit(model, a, b);
                } else {
                    TryExit(model, a, b);
                }

                return;

            } else {

                // Trigger Enter
                bool isIntersect = Intersect2DUtil.IsIntersectRB_RB(a, b, FP64.Zero);
                if (isIntersect) {
                    triggerEventCenter.EnqueueEnter(new InternalTrigger2DEventModel(a, b));
                    contactRepo.Add(new IntersectContact2DModel(key, a, b));
                }

            }

        }

        void TryExit(in IntersectContact2DModel model, FPRigidbody2DEntity a, FPRigidbody2DEntity b) {
            var triggerEventCenter = context.TriggerEventCenter;
            bool isIntersect = Intersect2DUtil.IsIntersectRB_RB(a, b, -FP64.Epsilon);
            if (!isIntersect) {
                TriggerExit(model, a, b);
            }
        }

        void TriggerExit(in IntersectContact2DModel model, FPRigidbody2DEntity a, FPRigidbody2DEntity b) {
            var triggerEventCenter = context.TriggerEventCenter;
            triggerEventCenter.EnqueueExit(new InternalTrigger2DEventModel(a, b));
            var contactRepo = context.IntersectContactRepo;
            contactRepo.Remove(model);
        }

    }

}