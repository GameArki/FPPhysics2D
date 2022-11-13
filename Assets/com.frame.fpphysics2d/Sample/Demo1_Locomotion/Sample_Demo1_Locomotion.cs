using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FixMath.NET;
using JackFrame.FPPhysics2D;
using JackFrame.FPMath;

namespace JackFrame.FPPhysics2D.Sample {

    public class Sample_Demo1_Locomotion : MonoBehaviour {

        // Logic
        FPSpace2D space;
        FPRigidbody2DEntity actor;

        void Awake() {
            space = new FPSpace2D(new FPVector2(0, -981 * FP64.EN2 * 2), new FPVector2(100, 100), 8);

            actor = FPRigidbody2DFactory.CreateBoxRB(new FPVector2(0, 5), 0, new FPVector2(1, 1));
            space.Add(actor);

            {
                const int COUNT = 20;
                for (int i = 0; i < COUNT; i += 1) {
                    var cell = FPRigidbody2DFactory.CreateBoxRB(new FPVector2(i - 10, 0), 0, new FPVector2(1, 1));
                    cell.SetStatic(true);
                    space.Add(cell);
                }
            }

            {
                var cell = FPRigidbody2DFactory.CreateBoxRB(new FPVector2(0, 3), 0, new FPVector2(1, 1));
                cell.SetStatic(true);
                space.Add(cell);
            }


        }

        void Update() {

        }

        void FixedUpdate() {

            FPVector2 velo = actor.LinearVelocity;
            float x = Input.GetAxis("Horizontal") * 5.5f;
            float y = Input.GetAxis("Jump");
            FPVector2 dir = new FPVector2(FP64.ToFP64(x), velo.y);
            if (y > 0) {
                dir.y = FP64.ToFP64(y) * 10;
            }
            actor.SetLinearVelocity(dir);

            space.Tick(FP64.ToFP64(Time.fixedDeltaTime));
        }

        void OnDrawGizmos() {
            if (space != null) {
                space.GizmosDrawAllRigidbody();
            }
        }
    }
}
