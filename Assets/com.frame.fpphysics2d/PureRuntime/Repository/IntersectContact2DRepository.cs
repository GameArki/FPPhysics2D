using System.Linq;
using System.Collections.Generic;

namespace JackFrame.FPPhysics2D {

    // 产生交叉的对象
    public class IntersectContact2DRepository {

        SortedDictionary<ulong, IntersectContact2DModel> all;

        public IntersectContact2DRepository() {
            this.all = new SortedDictionary<ulong, IntersectContact2DModel>();
        }

        public void Add(IntersectContact2DModel model) {
            all.Add(model.key, model);
        }

        public bool TryGet(ulong key, out IntersectContact2DModel model) {
            return all.TryGetValue(key, out model);
        }

        public void Remove(IntersectContact2DModel model) {
            all.Remove(model.key);
        }

        public void RemoveByKey(ulong key) {
            all.Remove(key);
        }

        public KeyValuePair<ulong, IntersectContact2DModel>[] GetAll() {
            return all.ToArray();
        }

        public void Clear() {
            all.Clear();
        }

    }

}