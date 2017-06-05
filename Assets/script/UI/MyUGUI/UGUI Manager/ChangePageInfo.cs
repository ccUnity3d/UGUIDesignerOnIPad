
public class ChangePageInfo  {

    public string prefabName;
    public string tipsContent;
    public string pageName;
    public int pageIndex;
    public ChangePageInfo(string prefabName,string tipsContent,string pageName,int pageIndex)
    {
        this.prefabName = prefabName;
        this.tipsContent = tipsContent;
        this.pageName = pageName;
        this.pageIndex = pageIndex;
    }
}
