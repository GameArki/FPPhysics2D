using System.Collections.Generic;
using FixMath.NET;

namespace JackFrame.FPPhysics2D {

    public class FPRigidbody2DRepository {

        List<FPRigidbody2DEntity> all;
        FPRigidbody2DEntity[] arr;

        public FPRigidbody2DRepository() {
            this.all = new List<FPRigidbody2DEntity>();
            arr = new FPRigidbody2DEntity[0];
        }

        public void Add(FPRigidbody2DEntity rb) {
            all.Add(rb);
            arr = all.ToArray();
        }

        public FPRigidbody2DEntity[] GetArray() {
            return arr;
        }

        public bool Contains(FPRigidbody2DEntity rb) {
            return all.Contains(rb);
        }

        public void Remove(FPRigidbody2DEntity rb) {
            all.Remove(rb);
            arr = all.ToArray();
        }

    }

}