using System;
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

        public void RemoveByID(uint id) {
            Span<ulong> targets = stackalloc ulong[all.Count];
            int index = 0;
            foreach (var kv in all) {
                var key = kv.Key;
                uint a = (uint)key;
                if (a == id) {
                    targets[index] = key;
                    index += 1;
                }
                uint b = (uint)(key >> 32);
                if (b == id) {
                    targets[index] = key;
                    index += 1;
                }
            }
            if (index != 0) {
                for (int i = 0; i < targets.Length; i += 1) {
                    var target = targets[i];
                    all.Remove(target);
                }
            }
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