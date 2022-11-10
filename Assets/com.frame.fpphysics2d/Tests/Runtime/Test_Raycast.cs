using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FixMath.NET;
using JackFrame.FPPhysics2D.API;

namespace JackFrame.FPPhysics2D {

    public class Test_Raycast : MonoBehaviour {

        GameObject[] allGo;
        FPRigidbody2DEntity[] allRB;

        FPSpace2D space2D;
        FPRigidbody2DEntity role => allRB[0];

        public GameObject lineStartGo;
        public GameObject lineEndGo;

        public int goCount = 6;

        private void Awake() {
            System.Console.WriteLine("开始");

            space2D = new FPSpace2D(new FPVector2(0, -981 * FP64.EN2));

            allGo = new GameObject[goCount];
            allRB = new FPRigidbody2DEntity[goCount];

            /*
            allGo[0] = new GameObject("go0");
            allRB[0] = FPRigidbody2DFactory.CreateBoxRB(new FPVector2(0, 0), 0, new FPVector2(1, 1));
            allGo[0].transform.position = allRB[0].TF.Pos.ToVector2();
            allRB[0].SetGravityScale(0);
            space2D.Add(allRB[0]);

            allGo[1] = new GameObject("go1");
            //allRB[1] = FPRigidbody2DFactory.CreateBoxRB(new FPVector2(1, 1), 45, new FPVector2(1, 1));
            allRB[1] = FPRigidbody2DFactory.CreateBoxRB(new FPVector2(1, 1), 0, new FPVector2(1, 1));
            allGo[1].transform.position = allRB[1].TF.Pos.ToVector2();
            //allGo[1].transform.rotation = Quaternion.Euler(0, 45f);
            allRB[1].SetGravityScale(0);
            space2D.Add(allRB[1]);

            allGo[2] = new GameObject("go2");
            allRB[2] = FPRigidbody2DFactory.CreateCircleRB(new FPVector2(3, 0), 1);
            allGo[2].transform.position = allRB[2].TF.Pos.ToVector2();
            allRB[2].SetGravityScale(0);
            //space2D.Add(allRB[2]);
            */

            allGo[0] = new GameObject("go2");
            allRB[0] = FPRigidbody2DFactory.CreateCircleRB(new FPVector2(3, 0), 1);
            allGo[0].transform.position = allRB[0].TF.Pos.ToVector2();
            allRB[0].SetGravityScale(0);
            space2D.Add(allRB[0]);

            lineStartGo = new GameObject("lineStartGo");
            lineStartGo.transform.position = new Vector2(-3, 3);
            lineEndGo = new GameObject("lineEndGo");
            lineEndGo.transform.position = new Vector2(3, 3);

            Debug.Log("初始化成功");

        }

        Color rayColor = Color.yellow;

        void OnDrawGizmos() {
            space2D?.GizmosDrawAllRigidbody();
            if ((lineEndGo != null) && (lineStartGo != null)) {
                Gizmos.color = rayColor;
                Gizmos.DrawLine(lineStartGo.transform.position, lineEndGo.transform.position);
            }
        }

        public void Clear() {
            for (int i = 0; i < allRB.Length; i += 1) {
                var rb = allRB[i];
                if (rb != null) {
                    space2D.Remove(rb);
                }
            }
            allRB = null;
            space2D = null;
            for (int i = 0; i < allGo.Length; i += 1) {
                DestroyImmediate(allGo[i]);
            }
            allGo = null;
            Debug.Log("回收成功");

        }

        private void Update() {

            if (Input.GetKeyDown(KeyCode.Escape)) {
                Clear();
            }

            if ((allGo != null) && (allRB != null)) {
                for (int i = 0; i < allGo.Length; i++) {
                    if ((allGo[i] != null) && (allRB[i] != null)) {
                        allRB[i].SetPos(new FPVector2((FP64)allGo[i].transform.position.x, (FP64)allGo[i].transform.position.y));
                        allRB[i].SetRotRadianAngle((FP64)allGo[i].transform.rotation.eulerAngles.z);
                        //Debug.Log("设置坐标:" + allGo[i].name + ":" + allRB[i].TF.Pos);
                        if (allRB[i].TF.RadAngle != FP64.Zero) {
                            Debug.Log("设置角度:" + allGo[i].name + ":" + allRB[i].TF.RadAngle + ";rotation=" + allGo[i].transform.rotation.eulerAngles.z);
                        }
                    }
                }
            }

            var rayStart = new FPVector2((FP64)lineStartGo.transform.position.x, (FP64)lineStartGo.transform.position.y);
            var rayEnd = new FPVector2((FP64)lineEndGo.transform.position.x, (FP64)lineEndGo.transform.position.y);
            var contactFilter = new FPContactFilter2DArgs();
            var getterApi = space2D.GetterAPI;
            var intersectResult = new RaycastHit2DArgs[10];
            var isHit = getterApi.Segmentcast2D(rayStart, rayEnd, contactFilter, ref intersectResult);
            if (isHit) {
                rayColor = Color.red;
                Debug.Log("碰撞");
            } else {
                rayColor = Color.yellow;
            }
        }

    }

}