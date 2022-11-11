namespace JackFrame.FPPhysics2D.API {

    public struct FPContactFilter2DArgs {
        public bool isFiltering;
        public bool useTriggers; // 默认为 false, 即不检测 Trigger
        public bool containHolder; // 默认为 true, 即会检测自身
        public int holderFBID;
        public bool containStatic; // 默认为 false, 即不检测静态物体
        public bool useLayerMask;
        public int layerMask;
        public bool useDepth; // 暂不支持深度过滤，因为目前没有使用 z 轴
        public int minDepth;
        public int maxDepth;

    }

}