using UnityEngine;
using System.Collections;

public class SelectGoods2D_DetailState : SelectGoods2D_State
{
    private ParticularsPage particular
    {
        get
        {
            return ParticularsPage.Instance;
        }
    }

    private ToggleButton toggleButton;
    public override void enter()
    {
        base.enter();
        //在物体附近打开详情面板
        if (toggleButton.onDown == false)
        {
            UITool.SetActionFalse(particular.particular.gameObject);
            setState(EditTypeOnSelect.Free);
            return;
        }
        UITool.SetActionTrue(particular.particular.gameObject);
        SetTextureTool.SetTexture(particular.produceImage, targetProduct.tempTexture, targetProduct.seekId);
        particular.ProduceName.text = string.Format("产品名称：{0}",targetProduct.name);
        particular.price.text = string.Format("单价：{0:C}", 0);
        particular.brand.text = string.Format("品牌：{0}", targetProduct.vendor);
        particular.no.text = string.Format("产品编号：{0}", targetProduct.seekId);
    }

    public override void exit()
    {
        base.exit();
        UITool.SetActionFalse(particular.particular.gameObject);
        toggleButton.onDown = false;
    }
    public override void Ready(GameObject skin)
    {
        base.Ready(skin);
        particular.SetData(MainPage.Instance.SkinParticular);
        toggleButton = OptionsPage.Instance.particulars;
    }
}
