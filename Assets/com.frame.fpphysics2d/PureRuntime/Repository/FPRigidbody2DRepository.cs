using System.Linq;
using System.Collections.Generic;

namespace JackFrame.FPPhysics2D {

    public class FPRigidbody2DRepository {

        Dictionary<uint, FPRigidbody2DEntity> all;
        FPRigidbody2DEntity[] arr;

        public FPRigidbody2DRepository() {
            this.all = new Dictionary<uint, FPRigidbody2DEntity>();
            arr = new FPRigidbody2DEntity[0];
        }

        public void Add(FPRigidbody2DEntity rb) {
            bool has = all.TryAdd(rb.ID, rb);
            if (has) {
                arr = all.Values.ToArray();
            }
        }

        public FPRigidbody2DEntity[] GetArray() {
            return arr;
        }

        public bool Contains(FPRigidbody2DEntity rb) {
            return all.ContainsKey(rb.ID);
        }

        public void Remove(FPRigidbody2DEntity rb) {
            bool has = all.Remove(rb.ID);
            if (has) {
                arr = all.Values.ToArray();
            }
        }

    }

}