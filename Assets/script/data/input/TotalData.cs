using System.Collections.Generic;

public class TotalData {
    //上传服务器数据
    //户型所有墙体数据（uv mesh transform 贴图 shader）
    //地板的（uv mesh transform 贴图 shader）
    //天花板的（uv mesh transform 贴图 shader）
    ///
    //家具的transform
    //相机transform

    //设计师原始输入数据（OriginalInputData）
    public OriginalInputData data = new OriginalInputData();


    public SchemeManifest schemeManifest = new SchemeManifest();

    /// <summary>
    /// 变动的版本号
    /// </summary>
    [System.NonSerialized]
    public int SaveId;

    //操作辅助数据InputHelperData等
    //
}
