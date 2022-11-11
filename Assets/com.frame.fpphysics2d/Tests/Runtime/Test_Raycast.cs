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
        FPGetterAPI getterAPI;

        public GameObject lineStartGo;
        public GameObject lineEndGo;
        public int goCount = 6;

        RaycastHit2DArgs[] hits;
        FPVector2 pointToDraw;
        FPContactFilter2DArgs filter;

        private void Awake() {
            System.Console.WriteLine("开始");

            space2D = new FPSpace2D(new FPVector2(0, -981 * FP64.EN2));
            getterAPI = space2D.GetterAPI;

            allGo = new GameObject[goCount];
            allRB = new FPRigidbody2DEntity[goCount];

            hits = new RaycastHit2DArgs[10];

            allGo[0] = new GameObject("go0");
            allRB[0] = FPRigidbody2DFactory.CreateBoxRB(new FPVector2(0, 0), 0, new FPVector2(1, 1));
            allGo[0].transform.position = allRB[0].TF.Pos.ToVector2();
            allRB[0].SetGravityScale(0);
            space2D.Add(allRB[0]);

            allGo[1] = new GameObject("go1");
            allRB[1] = FPRigidbody2DFactory.CreateBoxRB(new FPVector2(2, 0), 0, new FPVector2(1, 1));
            allGo[1].transform.position = allRB[1].TF.Pos.ToVector2();
            allRB[1].SetGravityScale(0);
            space2D.Add(allRB[1]);

            allGo[2] = new GameObject("go2");
            allRB[2] = FPRigidbody2DFactory.CreateCircleRB(new FPVector2(0, -4), 1);
            allGo[2].transform.position = allRB[2].TF.Pos.ToVector2();
            allRB[2].SetGravityScale(0);
            space2D.Add(allRB[2]);

            // Test Holder Filter
            allGo[3] = new GameObject("holderFilter");
            allRB[3] = FPRigidbody2DFactory.CreateCircleRB(new FPVector2(2, -4), 2);
            allGo[3].transform.position = allRB[3].TF.Pos.ToVector2();
            allRB[3].SetGravityScale(0);
            space2D.Add(allRB[3]);

            // Test Layer Filter
            allGo[4] = new GameObject("layerFilter");
            allRB[4] = FPRigidbody2DFactory.CreateCircleRB(new FPVector2(4, -4), 1);
            allGo[4].transform.position = allRB[4].TF.Pos.ToVector2();
            allRB[4].SetGravityScale(0);
            allRB[4].SetLayer(2);
            space2D.Add(allRB[4]);

            // Test Trigger Filter
            allGo[5] = new GameObject("triggerFilter");
            allRB[5] = FPRigidbody2DFactory.CreateCircleRB(new FPVector2(6, -4), 1);
            allGo[5].transform.position = allRB[5].TF.Pos.ToVector2();
            allRB[5].SetGravityScale(0);
            allRB[5].SetTrigger(true);
            space2D.Add(allRB[5]);

            lineStartGo = new GameObject("lineStartGo");
            lineStartGo.transform.position = new Vector2(-3, 3);
            lineEndGo = new GameObject("lineEndGo");
            lineEndGo.transform.position = new Vector2(3, 3);

            filter = new FPContactFilter2DArgs();
            filter.isFiltering = true;
            filter.useTriggers = false;
            filter.useLayerMask = true;
            filter.layerMask = 2;
            filter.containHolder = false;
            filter.containStatic = false;
            filter.holderRBID = allRB[3].ID;

            Debug.Log("初始化成功");

        }

        Color rayColor = Color.yellow;

        void OnDrawGizmos() {
            space2D?.GizmosDrawAllRigidbody();
            if ((lineEndGo != null) && (lineStartGo != null)) {
                Gizmos.color = rayColor;
                Gizmos.DrawLine(lineStartGo.transform.position, lineEndGo.transform.position);
            }
            //GizmosHelper.DrawPoint(pointToDraw, Color.cyan);
            GizmosHelper.DrawCube(pointToDraw, new FPVector2((FP64)0.2, (FP64)0.2), Color.cyan);

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



            var isHit = getterAPI.Segmentcast2D(rayStart, rayEnd, filter, hits);
            if (isHit) {
                rayColor = Color.red;
                pointToDraw = hits[0].point;
                Debug.Log("碰撞");
            } else {
                rayColor = Color.yellow;
            }
        }

    }

}