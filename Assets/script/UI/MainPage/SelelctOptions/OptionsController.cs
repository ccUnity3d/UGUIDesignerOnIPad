using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class OptionsController : UIController<OptionsController> {

    private OptionsPage optionsPage;
    private MainPageUIController mainController;
    public OptionsController()
    {
        panel = optionsPage = OptionsPage.Instance;
        mainController = MainPageUIController.Instance;
    }
    
    private IStateMachine _selectMachine = null;

    /// <summary>
    /// 选中物体的子状态管理器
    /// </summary>
    public IStateMachine selectMachine
    {
        get
        {
            //Debug.LogWarning("Get " + _selectMachine);
            return _selectMachine;
        }
        set
        {
            _selectMachine = value;
            //Debug.LogWarning("Set " + _selectMachine);
        }
    }
    
    private List<Transform> buttonItemList = new List<Transform>();

    public override void ready()
    {
        optionsPage.particulars.onClick.AddListener(onPartic);
        optionsPage.copy.onClick.AddListener(onCopy);
        optionsPage.l_r_mirroring.onClick.AddListener(onL_R_Mirror);
        optionsPage.u_d_mirroring.onClick.AddListener(onU_D_Mirror);
        optionsPage.replace.onClick.AddListener(onReplace);
        optionsPage.disWall.onClick.AddListener(onDisWall);
        optionsPage.disFloor.onClick.AddListener(onDisFloor);
        optionsPage.group.onClick.AddListener(onGroup);
        optionsPage.m_delete.onClick.AddListener(onDelete);
        optionsPage.collect.onClick.AddListener(Collect);
        optionsPage.m_show.onClick.AddListener(onHide);

        //optionsPage.selectWall.onClick.AddListener(onSelectWall);
        optionsPage.wall4.onClick.AddListener(onWall4);
        optionsPage.wall5.onClick.AddListener(onWall5);
        optionsPage.rotation.onClick.AddListener(onRotaion);

        optionsPage.wall2.onClick.AddListener(onWall2);
        optionsPage.setRoomType.onValueChanged.AddListener(onRoomTypeChanged);
        optionsPage.deletRoom.onClick.AddListener(onDelete);

        SelectPoint_StateMachine.Instance.Ready(optionsPage.skin);
        SelectWall_StateMachine.Instance.Ready(optionsPage.skin);
        SelectRoom_StateMachine.Instance.Ready(optionsPage.RectSetRoom.gameObject);
        SelectWallSide_StateMachine.Instance.Ready(optionsPage.skin);
        SelectCeiling_StateMachine.Instance.Ready(optionsPage.RectRotation.gameObject);
        SelectFloor_StateMachine.Instance.Ready(optionsPage.RectRotation.gameObject);
        SelectGoods2D_StateMachine.Instance.Ready(optionsPage.skin);
        SelectGoods3D_StateMachine.Instance.Ready(optionsPage.skin);
    }
    //private bool isOnRotation;
    private void onRotaion()
    {
        if (selectMachine != null) selectMachine.setState(EditTypeOnSelect.Rotate);
    }


    #region OptionList
    private void onPartic()
    {
        Debug.Log("详情");
        if (selectMachine != null) selectMachine.setState(EditTypeOnSelect.Detail);
    }
    private void onCopy()
    {
        Debug.Log("复制");
        if (selectMachine != null) selectMachine.setState(EditTypeOnSelect.Copy);
    }
    private void onL_R_Mirror()
    {
        Debug.Log("左右镜像");
        if (selectMachine != null) selectMachine.setState(EditTypeOnSelect.MirroredX);
    }
    private void onU_D_Mirror()
    {
        Debug.Log("上下镜像");
        if (selectMachine != null) selectMachine.setState(EditTypeOnSelect.MirroredY);
    }
    private void onReplace()
    {
        Debug.Log("替换");
        if (selectMachine != null) selectMachine.setState(EditTypeOnSelect.Replace);
    }
    private void onDisFloor()
    {
        if (selectMachine != null) selectMachine.setState(EditTypeOnSelect.DisFloor);
    }
    private bool isWall2;
    private void onWall2()
    {
        if (selectMachine != null) selectMachine.setState(EditTypeOnSelect.SetTLine);
        //isWall2 = !isWall2;
        //if (isWall2)
        //{
        //    UITool.SetActionTrue(optionsPage.wall2optionList.gameObject);
        //    return;
        //}
        //Debug.Log("onWall2");
        //UITool.SetActionFalse(optionsPage.wall2optionList.gameObject);
        //mainController.CloseInput();
    }
    //private bool isWall;
    private void onDisWall()
    {
        Debug.Log("墙距离");
        if (selectMachine != null) selectMachine.setState(EditTypeOnSelect.DisWall);
    }
    private void onGroup()
    {
        Debug.Log("群组");
        if (selectMachine != null) selectMachine.setState(EditTypeOnSelect.Group);
    }
    private void onDelete()
    {
        Debug.Log("删除");
        if (selectMachine != null) selectMachine.setState(EditTypeOnSelect.Delet);
    }
    private void Collect()
    {
        Debug.Log("收藏");
        if (selectMachine != null) selectMachine.setState(EditTypeOnSelect.Collect);
    }
    private void onHide()
    {
        Debug.Log("隐藏");
        if (selectMachine != null) selectMachine.setState(EditTypeOnSelect.Hide);
    }
    #endregion

    #region OptionFunc

    //private void onSelectWall()
    //{
    //    Debug.Log("onSelectWall");
    //}
    //private bool iswall4;
    private void onWall4()
    {
        if (selectMachine != null) selectMachine.setState(EditTypeOnSelect.SetTopLine);
        //iswall4 = !iswall4;
        //if (iswall4)
        //{
        //    UITool.SetActionTrue(optionsPage.wall4optionList.gameObject);
        //    return;
        //}
        //Debug.Log("onWall4");
        //UITool.SetActionFalse(optionsPage.wall4optionList.gameObject);
        //mainController.CloseInput();
    }
    private void onWall5()
    {
        Debug.Log("onWall5");
        if (selectMachine != null) selectMachine.setState(EditTypeOnSelect.UseToAllSide);
    }
    #endregion


    public void ShowSelect(GoodsType type = GoodsType.Default, ObjData objData = null)
    {
        //Debug.LogWarning("ShowSelect : " + type);
        buttonItemList.Clear();
        switch (type)
        {
            case GoodsType.Product:
                buttonItemList.Add(optionsPage.RectSelectWallSide);
                buttonItemList.Add(optionsPage.rotation.transform);
                buttonItemList.Add(optionsPage.RectSetRoom);
                ShowSelectItem(buttonItemList, false);
                break;
            case GoodsType.Door:
                buttonItemList.Add(optionsPage.group.transform);
                buttonItemList.Add(optionsPage.collect.transform);
                buttonItemList.Add(optionsPage.replace.transform);
                buttonItemList.Add(optionsPage.disFloor.transform);
                buttonItemList.Add(optionsPage.copy.transform);
                buttonItemList.Add(optionsPage.particulars.transform);
                ShowSelectItem(buttonItemList);
                break;
            case GoodsType.Window:
                buttonItemList.Add(optionsPage.replace.transform);
                buttonItemList.Add(optionsPage.group.transform);
                buttonItemList.Add(optionsPage.collect.transform);
                buttonItemList.Add(optionsPage.disFloor.transform);
                buttonItemList.Add(optionsPage.copy.transform);
                buttonItemList.Add(optionsPage.particulars.transform);
                ShowSelectItem(buttonItemList);
                break;
            case GoodsType.Point:
                buttonItemList.Add(optionsPage.m_delete.transform);
                buttonItemList.Add(optionsPage.backGroundThereD.transform);
                ShowSelectItem(buttonItemList, true);
                break;
            case GoodsType.Wall:
                buttonItemList.Add(optionsPage.m_delete.transform);
                buttonItemList.Add(optionsPage.backGroundThereD.transform);
                buttonItemList.Add(optionsPage.m_show.transform);
                ShowSelectItem(buttonItemList, true);
                break;
            case GoodsType.Room:
                buttonItemList.Add(optionsPage.RectSetRoom);
                buttonItemList.Add(optionsPage.backGroundThereD.transform);
                ShowSelectItem(buttonItemList, true);
                break;
            case GoodsType.WallSide:
                buttonItemList.Add(optionsPage.selectWall.transform);
                buttonItemList.Add(optionsPage.rotation.transform);
                buttonItemList.Add(optionsPage.backGroundThereD.transform);
                ShowSelectItem(buttonItemList, true);
                break;
            case GoodsType.Ceiling:
                buttonItemList.Add(optionsPage.rotation.transform);
                buttonItemList.Add(optionsPage.backGroundThereD.transform);
                ShowSelectItem(buttonItemList, true);
                break;
            case GoodsType.Floor:
                buttonItemList.Add(optionsPage.rotation.transform);
                buttonItemList.Add(optionsPage.backGroundThereD.transform);
                ShowSelectItem(buttonItemList, true);
                break;
            case GoodsType.Default:
                ShowSelectItem();
                break;
        }
    }

    /// <summary>
    /// 选择选中状态对应的功能按键
    /// </summary>
    /// <param name="buttonList"> buttonList = null 表示全部隐藏</param>
    /// <param name="listIsShow"> listIsShow = true 表示显示list项其他隐藏，反之隐藏list项其他显示</param>
    private void ShowSelectItem(List<Transform> buttonList = null, bool listIsShow = false)
    {
        if (buttonList == null)
        {
            for (int j = 0; j < optionsPage.operationList.Count; j++)
            {
                optionsPage.operationList[j].gameObject.SetActive(false);
            }
            return;
        }
        if (!listIsShow)
        {
            for (int j = 0; j < optionsPage.operationList.Count; j++)
            {
                optionsPage.operationList[j].gameObject.SetActive(true);
            }
            for (int i = 0; i < buttonList.Count; i++)
            {
                //if (optionsPage.operationList.Contains(buttonList[i]))
                //{
                buttonList[i].gameObject.SetActive(false);
                //}
            }
            return;
        }

        for (int j = 0; j < optionsPage.operationList.Count; j++)
        {
            optionsPage.operationList[j].gameObject.SetActive(false);
        }
        //Debug.Log(buttonList.Count);
        for (int i = 0; i < buttonList.Count; i++)
        {
            //if (optionsPage.operationList.Contains(buttonList[i]))
            //{
            buttonList[i].gameObject.SetActive(true);
            //}
        }
    }
    /// <summary>
    /// 取消选中的状态 隐藏不该显示的按钮
    /// </summary>
    public void HideSelect()
    {
        ShowSelect(GoodsType.Default);
    }



    private void onRoomTypeChanged(int arg0)
    {
        if (selectMachine is SelectRoom_StateMachine)
        {
            SelectRoom_StateMachine machine = selectMachine as SelectRoom_StateMachine;
            machine.roomType = optionsPage.setRoomType.options[arg0].text;
            machine.setState(EditTypeOnSelect.SetRoomType);
        }
    }

}
