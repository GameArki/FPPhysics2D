using System;
using System.Linq;
using System.Collections.Generic;
using FixMath.NET;
using JackFrame.FPMath;

namespace JackFrame.FPPhysics2D {

    public class FPRigidbody2DRepository {

        HashSet<FPRigidbody2DEntity> all;
        FPRigidbody2DEntity[] array;

        // ==== Quadtree ====
        FPQuadTree<FPRigidbody2DEntity> tree;
        List<FPQuadTreeNode<FPRigidbody2DEntity>> candidates;

        public FPRigidbody2DRepository(FPVector2 worldSize, int maxDepth) {
            this.all = new HashSet<FPRigidbody2DEntity>();
            this.array = new FPRigidbody2DEntity[0];
            this.tree = new FPQuadTree<FPRigidbody2DEntity>(worldSize.x, worldSize.y, maxDepth);
            this.candidates = new List<FPQuadTreeNode<FPRigidbody2DEntity>>(32);
        }

        public void Add(FPRigidbody2DEntity rb) {
            bool has = all.Add(rb);
            if (has) {
                // add to tree
                var node = tree.Insert(rb, rb.GetPruneBounding());
                rb.treeNode = node;

                array = all.ToArray();
            }
        }

        public List<FPQuadTreeNode<FPRigidbody2DEntity>> GetCandidates(FPRigidbody2DEntity rb) {
            tree.GetCandidates(rb.GetPruneBounding(), candidates);
            return candidates;
        }

        public void UpdateTree(FPRigidbody2DEntity rb) {
            tree.Remove(rb.treeNode.GetFullID());
            tree.Insert(rb, rb.GetPruneBounding());
        }

        public FPRigidbody2DEntity[] GetArray() {
            return array;
        }

        public void Remove(FPRigidbody2DEntity rb) {
            bool has = all.Remove(rb);
            if (has) {
                // remove from tree
                tree.Remove(rb.treeNode.GetFullID());
                rb.treeNode = null;

                array = all.ToArray();
            }
        }

        public void Foreach(Action<FPRigidbody2DEntity> action) {
            for (int i = 0; i < array.Length; i += 1) {
                action.Invoke(array[i]);
            }
        }

    }

}