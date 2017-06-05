using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RenderPage : UIPage<RenderPage> {


    #region Render
    public RectTransform RectRender;
    public Button aspect;
    public RectTransform RectAspect;
    public Button fourToThere;
    public Image fourToThereBG;
    public Text fourToThereText;
    public Button sixteenToNine;
    public Image sixteenToNineBG;
    public Text sixteenToNineText;
    public Toggle perview;
    public Toggle common;
    public Toggle hight;
    public Button renderScene;
    public Button environment;
    public Button exit;
    public RectTransform RectEnvironment;
    public Toggle toggle0;
    public Toggle toggle1;
    public Toggle toggle2;
    public Toggle toggle3;
    public Button rotation;
    public RectTransform progress;
    public Slider progressBar;
    public Text value;
    public Text Loadtime;
    public Button colse;
    public Button effectImage;
    public Button startRender;
    public RectTransform messageBox;
    public Button cancel;
    public Button confirm;

    #endregion
    protected override void Ready(Object arg1)
    {
        RectRender = skin.GetComponent<RectTransform>();
        //aspect = UITool.GetUIComponent<Button>(RectRender, "BottomMenu/Aspect");
        //RectAspect = UITool.GetUIComponent<RectTransform>(aspect.transform, "aspectItem");
        fourToThere = UITool.GetUIComponent<Button>(RectRender, "BottomMenu/fourTothere");
        fourToThereBG = UITool.GetUIComponent<Image>(RectRender, "BottomMenu/fourTothere/background");
        fourToThereText = UITool.GetUIComponent<Text>(RectRender, "BottomMenu/fourTothere/name");
        sixteenToNine = UITool.GetUIComponent<Button>(RectRender, "BottomMenu/sixteenTonine");
        startRender = UITool.GetUIComponent<Button>(RectRender,"BottomMenu/StartRender");
        messageBox = UITool.GetUIComponent<RectTransform>(RectRender,"MessageBox");
        cancel = UITool.GetUIComponent<Button>(messageBox, "Render/cancel");
        confirm = UITool.GetUIComponent<Button>(messageBox, "Render/ok");

        sixteenToNineBG = UITool.GetUIComponent<Image>(RectRender, "BottomMenu/sixteenTonine/background");
        sixteenToNineText = UITool.GetUIComponent<Text>(RectRender, "BottomMenu/sixteenTonine/name");
        common = UITool.GetUIComponent<Toggle>(RectRender, "BottomMenu/Common");
        hight = UITool.GetUIComponent<Toggle>(RectRender, "BottomMenu/Hight");
        exit = UITool.GetUIComponent<Button>(RectRender, "Exit");
    }
}
