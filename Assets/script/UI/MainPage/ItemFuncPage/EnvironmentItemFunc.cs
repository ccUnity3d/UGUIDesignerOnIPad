using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
public class EnvironmentItemFunc : UGUIItemFunction {


    public Text label;
    private EnvironmentItemData itemData
    {
        get { return data as EnvironmentItemData; }

    }
    protected override void Awake()
    {
        label = UITool.GetUIComponent<Text>(this.transform,"Label");
    }
    public override void Render()
    {
        base.Render();
        label.text = itemData.label;
    }
}
