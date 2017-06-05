using UnityEngine;
using System.Collections;

public class ParticularState : MainPageState {

    public const string Name = "ParticularState";
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
        Debug.Log("  进入详情");
        if (toggleButton.onDown == false )
        {
            Debug.Log("  详情    ");
            UITool.SetActionFalse(particular.particular.gameObject);
            setState(MainPageFreeState.Name);
            return;
        }
        UITool.SetActionTrue(particular.particular.gameObject);
        particular.produceImage = null;
        particular.price.text = string.Format("单价：{0:C}", 666);
        particular.brand.text = string.Format("品牌：{0}","");
        particular.no.text = string.Format("产品编号：{0}","");
    }

    public override void exit()
    {
        base.exit();
        //inputMachine.setState(FreeState2D.NAME);
        toggleButton.onDown = false;
        
    }

    public override void Ready()
    {
        base.Ready();
       
    }
}
