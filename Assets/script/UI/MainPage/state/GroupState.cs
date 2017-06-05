using UnityEngine;
using System.Collections;

public class GroupState : MainPageState {


    public static string Name = "GroupState";

    public ToggleButton toggleButton;
    private MyTweenRectPosition myTween; 
    private GroupPage groupPage
    {
        get {
            return GroupPage.Instance;
        }
    }

    private OptionsPage optionsPage { get { return OptionsPage.Instance;} }

    public override void enter()
    {


        Debug.Log("群组 状态进入");
        myTween.from = Vector2.up* groupPage.groupRectTransform.anchoredPosition.y;
        myTween.duration = 1f;
        myTween.to = -Vector2.right*550+Vector2.up* groupPage.groupRectTransform.anchoredPosition.y;
        if (toggleButton.onDown == false )
        {
            setState(EditTypeOnSelect.Free);
            myTween.SetStartToCurrentValue();
            myTween.to = Vector2.up * groupPage.groupRectTransform.anchoredPosition.y;
            myTween.PlayForward();
            return;
        }
        myTween.SetStartToCurrentValue();
        myTween.to = -Vector2.right * 550 + Vector2.up * groupPage.groupRectTransform.anchoredPosition.y;
        myTween.PlayForward();
    }

    public override void exit()
    {
        base.exit();
        toggleButton.onDown = false;
    }

    public override void Ready()
    {
        toggleButton = optionsPage.group;
        groupPage.SetData(mainpage.SkinGroup);
        myTween = groupPage.groupRectTransform.gameObject.AddComponent<MyTweenRectPosition>();
        groupPage.inputInfo.onValueChanged.AddListener(onInputInfo);
        groupPage.inputName.onValueChanged.AddListener(onInputName);
        groupPage.ok.onClick.AddListener(onOk);
        groupPage.cancel.onClick.AddListener(onCancel);
        groupPage.collectGroup.gameObject.AddComponent<GroupScroll>();
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
        // 确定添加组合
    }
    private void onCancel()
    {
        // 取消添加组合
    }
}
