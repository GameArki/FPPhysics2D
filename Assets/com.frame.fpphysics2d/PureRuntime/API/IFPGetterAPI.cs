using FixMath.NET;

namespace JackFrame.FPPhysics2D.API {

    public interface IFPGetterAPI {

        // - Raycast
        RaycastHit2DArgs Raycast2D(FPVector2 origin, FPVector2 direction, FP64 distance, FPContactFilter2DArgs contactFilter);
        bool Segmentcast2D(FPVector2 origin, FPVector2 end, FPContactFilter2DArgs contactFilter, ref RaycastHit2DArgs[] hits);

    }

}