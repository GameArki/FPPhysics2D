namespace JackFrame.FPPhysics2D {

    internal class FPContext2D {

        // ==== Environment ====
        FPEnvironment2DModel env;
        public FPEnvironment2DModel Env => env;

        // ==== Event Center ====
        // - Trigger
        Trigger2DEventCenter triggerEventCenter;
        public Trigger2DEventCenter TriggerEventCenter => triggerEventCenter;

        // - Collision
        Collision2DEventCenter collisionEventCenter;
        public Collision2DEventCenter CollisionEventCenter => collisionEventCenter;

        // ==== Repo ====
        // - Rigidbody Repo
        FPRigidbody2DRepository rbRepo;
        public FPRigidbody2DRepository RBRepo => rbRepo;

        // - Prune Ignore Repo
        PruneIgnoreContact2DRepository pruneIgnoreContactRepo;
        public PruneIgnoreContact2DRepository PruneIgnoreContactRepo => pruneIgnoreContactRepo;

        // - Intersect Contact Repo
        IntersectContact2DRepository intersectContactRepo;
        public IntersectContact2DRepository IntersectContactRepo => intersectContactRepo;

        // - Collision Contact Repo
        CollisionContact2DRepository collisionContactRepo;
        public CollisionContact2DRepository CollisionContactRepo => collisionContactRepo;

        public FPContext2D() {

            this.env = new FPEnvironment2DModel();

            // ==== Event Center ====
            this.triggerEventCenter = new Trigger2DEventCenter();
            this.collisionEventCenter = new Collision2DEventCenter();

            // ==== Repo ====
            this.rbRepo = new FPRigidbody2DRepository();
            this.pruneIgnoreContactRepo = new PruneIgnoreContact2DRepository();
            this.intersectContactRepo = new IntersectContact2DRepository();
            this.collisionContactRepo = new CollisionContact2DRepository();

        }

    }

}