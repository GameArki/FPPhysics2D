using FixMath.NET;

namespace JackFrame.FPPhysics2D {

    internal class Force2DPhase {

        FPContext2D context;

        internal Force2DPhase() { }

        internal void Inject(FPContext2D context) {
            this.context = context;
        }

        internal void Tick(in FP64 step) {

            var rbRepo = context.RBRepo;
            var all = rbRepo.GetArray();
            for (int i = 0; i < all.Length; i += 1) {
                var rb = all[i];
                ApplyGravity(rb, step);
                ApplyVelocity(rb, step);
            }

        }

        void ApplyGravity(FPRigidbody2DEntity rb, in FP64 step) {

            if (rb.IsStatic) {
                return;
            }

            var gravityScale = rb.GravityScale;
            if (gravityScale == FP64.Zero) {
                return;
            }

            var env = context.Env;
            var gravity = env.Gravity;
            var velo = rb.LinearVelocity;
            velo += gravity * gravityScale * step;
            rb.SetLinearVelocity(velo);

        }

        void ApplyVelocity(FPRigidbody2DEntity rb, in FP64 step) {

            if (rb.IsStatic) {
                return;
            }

            var velo = rb.LinearVelocity;
            var tf = rb.TF;
            var pos = tf.Pos + velo * step;
            tf.SetPos(pos);

        }

    }
}