using FixMath.NET;

namespace JackFrame.FPPhysics2D.API {

    public interface IFPGetterAPI {

        // - Raycast
        bool Raycast2DByDirection(in FPVector2 origin, in FPVector2 direction, in FP64 distance, FPContactFilter2DArgs contactFilter, RaycastHit2DArgs[] hits);
        bool Raycast2D(in FPVector2 origin, in FPVector2 end, FPContactFilter2DArgs contactFilter, RaycastHit2DArgs[] hits);

    }

}