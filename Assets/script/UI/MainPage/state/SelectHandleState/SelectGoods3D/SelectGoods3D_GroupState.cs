using UnityEngine;
using System.Collections;

public class SelectGoods3D_GroupState : SelectGoods3D_State
{

    public ToggleButton toggleButton;
    private OptionsPage optionsPage { get { return OptionsPage.Instance; } }

    private MyTweenRectPosition myTween;
    private GroupPage groupPage
    {
        get
        {
            return GroupPage.Instance;
        }
    }
    public override void enter()
    {
        Debug.Log("选择要打包成组的物体");
        base.enter();
        myTween.from = Vector2.up;
        myTween.duration = 1f;
        myTween.to = Vector2.right * (-550);
        if (toggleButton.onDown == false)
        {
            setState(EditTypeOnSelect.Free);
            myTween.SetStartToCurrentValue();
            myTween.to = Vector2.up;
            myTween.PlayForward();
            return;
        }
        myTween.SetStartToCurrentValue();
        myTween.to = Vector2.right * (-550);
        myTween.PlayForward();
        //选择要打包成组的物体

    }

    public override void exit()
    {
        base.exit();
        toggleButton.onDown = false;
    }
    public override void Ready(GameObject skin)
    {
        base.Ready(skin);
        toggleButton = optionsPage.group;
        groupPage.SetData(MainPage.Instance.SkinGroup);
        myTween = groupPage.groupRectTransform.gameObject.AddComponent<MyTweenRectPosition>();
        //groupPage.groupRectTransform.anchoredPosition3D = Vector3.up * (-136);
        groupPage.inputInfo.onValueChanged.AddListener(onInputInfo);
        groupPage.inputName.onValueChanged.AddListener(onInputName);
        groupPage.ok.onClick.AddListener(onOk);
        groupPage.cancel.onClick.AddListener(onCancel);
    }
    private void onInputName(string str)
    {
        // 输入组合名称
    }
    private void onInputInfo(string str)
    {
        // 输出组合备注信息
    }
    private void onOk()
    {
        myTween.SetStartToCurrentValue();
        myTween.to = Vector2.up;
        myTween.PlayForward();
        // 确定添加组合
    }
    private void onCancel()
    {
        myTween.SetStartToCurrentValue();
        myTween.to = Vector2.up;
        myTween.PlayForward();
        // 取消添加组合
    }
}
