namespace JackFrame.FPPhysics2D.API {

    public struct FPContactFilter2DArgs {
        public bool isFiltering;
        public bool useTriggers; // 默认为 false, 即不检测 Trigger
        public bool useLayerMask;
        public int layerMask;
        public bool useDepth;
        public int minDepth;
        public int maxDepth;

    }

}