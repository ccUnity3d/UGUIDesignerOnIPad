using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ParticularsPage : UIPage<ParticularsPage> {


    public RectTransform particular;
    public RawImage produceImage;
    public Text brand;
    public Text ProduceName;
    public Text no;
    public Text price;
    protected override void Ready(Object arg1)
    {
        particular = skin.GetComponent<RectTransform>();
        produceImage = UITool.GetUIComponent<RawImage>(particular,"Content/RawImage");
        brand = UITool.GetUIComponent<Text>(particular,"Content/Brand");
        ProduceName = UITool.GetUIComponent<Text>(particular,"Content/Name");
        no = UITool.GetUIComponent<Text>(particular,"Content/No");
        price = UITool.GetUIComponent<Text>(particular,"Content/Price");
    }
}
