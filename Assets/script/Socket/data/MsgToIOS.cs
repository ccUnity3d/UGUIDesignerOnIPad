[System.Serializable]
public class MsgToIOS {

    /// <summary>
    /// "101"+"001"
    /// </summary>
    public string code = "101001";
    public InfoToIOS info;

    [System.Serializable]
    public class InfoToIOS
    {
        /// <summary>
        /// 0上传新方案 1保存已有方案
        /// </summary>
        public int type;
        /// <summary>
        /// 设计方案数据
        /// </summary>
        public OriginalProjectData projectData;
        /// <summary>
        /// 报价数据
        /// </summary>
        public PriceData priceData;
        /// <summary>
        /// 一个物品的展示图
        /// </summary>
        public string image;
        /// <summary>
        /// 方案id
        /// </summary>
        public string projectId;
        /// <summary>
        /// 物品id
        /// </summary>
        public string goodsId;
        /// <summary>
        /// 携带信息
        /// </summary>
        public string msg;
        /// <summary>
        /// 分享的标题
        /// </summary>
        public string title;
        /// <summary>
        /// 分享的描述
        /// </summary>
        public string content;
        /// <summary>
        /// 分享的图片名
        /// </summary>
        public string photo;

        /// <summary>
        /// 0.取消收藏 1.收藏
        /// </summary>
        public int collectType;

        /// <summary>
        /// 0.分享方案 1.分享报价 2.分享效果图 3.单个商品
        /// </summary>
        public int shareType;

        /// <summary>
        /// IOS去服务器申请 
        /// </summary>
        public string HT5url;

        public string templateId;
    }

}
