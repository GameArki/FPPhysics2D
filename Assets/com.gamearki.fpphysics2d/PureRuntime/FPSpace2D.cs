using FixMath.NET;
using GameArki.FPPhysics2D.Phases;
using GameArki.FPPhysics2D.API;

namespace GameArki.FPPhysics2D {

    public class FPSpace2D {

        FPEventTrigger events;
        public FPEventTrigger Events => events;
        FPGetterAPI getterAPI;
        public FPGetterAPI GetterAPI => getterAPI;
        FPContext2D context;

        // ==== Phases ====
        Force2DPhase forcePhase;
        Prune2DPhase prunePhase;
        TriggerExitDispatch2DPhase triggerExitDispatchPhase;
        Intersect2DPhase intersectPhase;
        TriggerEnterAndStayDispatch2DPhase triggerEnterAndStayDispatchPhase;
        CollisionExitDispatch2DPhase collisionExitDispatchPhase;
        Penetrate2DPhase penetratePhase;
        CollisionEnterAndStayDispatch2DPhase collisionEnterAndStayDispatchPhase;

        public FPSpace2D(FPVector2 gravity, FPVector2 worldSize, int maxDepth = 8) {

            events = new FPEventTrigger();
            context = new FPContext2D(worldSize, maxDepth);

            // ==== Ctor ====
            forcePhase = new Force2DPhase();
            prunePhase = new Prune2DPhase();
            triggerExitDispatchPhase = new TriggerExitDispatch2DPhase();
            intersectPhase = new Intersect2DPhase();
            triggerEnterAndStayDispatchPhase = new TriggerEnterAndStayDispatch2DPhase();
            collisionExitDispatchPhase = new CollisionExitDispatch2DPhase();
            penetratePhase = new Penetrate2DPhase();
            collisionEnterAndStayDispatchPhase = new CollisionEnterAndStayDispatch2DPhase();
            this.getterAPI = new FPGetterAPI();

            // ==== Inject ====
            context.Inject(events);
            forcePhase.Inject(context);
            prunePhase.Inject(context);
            triggerExitDispatchPhase.Inject(context);
            intersectPhase.Inject(context);
            triggerEnterAndStayDispatchPhase.Inject(context);
            collisionExitDispatchPhase.Inject(context);
            penetratePhase.Inject(context);
            collisionEnterAndStayDispatchPhase.Inject(context);
            getterAPI.Inject(context);

            // ==== Init ====
            context.Env.SetGravity(gravity);

        }

        // - Ignore
        public void Ignore(int layer1, int layer2) {
            context.Ignore.Ignore(layer1, layer2);
        }

        public void CancelIgnore(int layer1, int layer2) {
            context.Ignore.CancelIgnore(layer1, layer2);
        }

        // - RB
        public void Add(FPRigidbody2DEntity rb) {
            context.RBRepo.Add(rb);
        }

        public void Remove(FPRigidbody2DEntity rb) {
            context.RBRepo.Remove(rb);

            var contactRepo = context.IntersectContactRepo;
            contactRepo.RemoveByID(rb.ID);
        }

        public FPRigidbody2DEntity[] GetAllRigidbody() {
            return context.RBRepo.GetArray();
        }

        // Recommand: step <= 0.016f;
        public void Tick(in FP64 step) {

            forcePhase.Tick(step); // ?????? ??????
            prunePhase.Tick(step); // ?????? (??????????????????????????????)
            triggerExitDispatchPhase.Tick(); // ??????????????? Trigger Exit ??????
            intersectPhase.Tick(step); // ????????????
            triggerEnterAndStayDispatchPhase.Tick(); // ??????????????? Trigger Enter / Stay ??????
            collisionExitDispatchPhase.Tick(); // ??????????????? Collision Exit ??????
            penetratePhase.Tick(step); // ????????????
            collisionEnterAndStayDispatchPhase.Tick(); // ??????????????? Collision Enter / Stay ??????

        }

    }

}