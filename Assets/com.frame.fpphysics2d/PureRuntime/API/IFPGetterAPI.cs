using FixMath.NET;

namespace JackFrame.FPPhysics2D.API {

    public interface IFPGetterAPI {

        // - Raycast
        bool Raycast2D(FPVector2 origin, FPVector2 direction, FP64 distance, FPContactFilter2DArgs contactFilter, RaycastHit2DArgs[] hits);
        bool Segmentcast2D(FPVector2 origin, FPVector2 end, FPContactFilter2DArgs contactFilter, RaycastHit2DArgs[] hits);

    }

}