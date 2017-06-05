using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class OptionsPage : UIPage<OptionsPage> {

    public List<Transform> operationList = new List<Transform>();

    /// TODO:加些备注
    public RectTransform RectSetRoom;
    public Dropdown setRoomType;
    public Button deletRoom;

    public RectTransform RectSelectWallSide;
    public Button selectWall;
    public Button wall2;
    public RectTransform RectWall2;
    public RectTransform wall2optionList;
    public Button option1;
    public Button option2;
    public Button option3;
    public Button option4;
    public Button option5;
    public Button option6;
    public Button setValue;
    public Text wallValue;

    public Button wall4;
    public RectTransform RectWall4;
    public RectTransform wall4optionList;
    public Button optionOne;
    public Button optionTwo;
    public Button optionThere;
    public Button optionFour;
    public Button optionFive;
    public Button optionSix;
    public Button optionSeven;
    public Button setValue1;
    public Text wallValue1;
    public Button setValue2;
    public Text wallValue2;
    public Button auto;
    public Button wall5;

    [Tooltip("详情")]
    public ToggleButton particulars;
    public Button copy;
    public Button l_r_mirroring;
    public Button u_d_mirroring;
    public Button replace;
    public ToggleButton disWall;
    public RectTransform distanceWall;
    public Button inputUnit;
    public Text inputValue;
    public Button cancel;
    public Button ok;
    public RectTransform RectDisFloorValue;
    public ToggleButton disFloor;
    public Text disFloorUnit;
    public Button defaultFloorDis;
    public Slider FloorSlider;
    public Text FloorTitle;
    public Button inputFloorValue;
    public Text disFloorValue;
    public ToggleButton rotation;
    public RectTransform RectRotation;
    public Button rotationDefault;
    public Button rotationAngle;
    public Slider rotationSlider;
    public Text sliderValue;
    public Button inputRotation;
    public Text rotationValue;
    public ToggleButton group;
    public Button m_delete;
    [Tooltip("收藏")]
    public Button collect;
    public Button m_show;
    public Button backGroundThereD;

    protected override void Ready(Object arg1)
    {
        base.Ready(arg1);

        RectSetRoom = UITool.GetUIComponent<RectTransform>(skin.transform, "BottomMenu/RectSetRoom");
        setRoomType = UITool.GetUIComponent<Dropdown>(RectSetRoom, "setroomDropdown");
        deletRoom = UITool.GetUIComponent<Button>(RectSetRoom, "delete");
        Transform bottomItem = UITool.GetUIComponent<Transform>(skin.transform, "BottomMenu/BottomItem");
        particulars = UITool.AddUIComponent<ToggleButton>(bottomItem, "particulars");
        copy = UITool.GetUIComponent<Button>(bottomItem, "copy");
        l_r_mirroring = UITool.GetUIComponent<Button>(bottomItem, "leftRightMirror");
        u_d_mirroring = UITool.GetUIComponent<Button>(bottomItem, "upDownMirror");
        replace = UITool.GetUIComponent<Button>(bottomItem, "replace");
        disWall = UITool.AddUIComponent<ToggleButton>(bottomItem, "disWall");
        distanceWall = UITool.GetUIComponent<RectTransform>(bottomItem, "disWallValue");
        inputUnit = UITool.GetUIComponent<Button>(distanceWall,"Button");
        inputValue = UITool.GetUIComponent<Text>(inputUnit.transform,"Text");
        cancel = UITool.GetUIComponent<Button>(distanceWall,"Cancel");
        ok = UITool.GetUIComponent<Button>(distanceWall,"Ok");
        disFloor = UITool.AddUIComponent<ToggleButton>(bottomItem, "disFloor");
        RectDisFloorValue = UITool.GetUIComponent<RectTransform>(bottomItem, "disFloorValue");
        defaultFloorDis = UITool.GetUIComponent<Button>(RectDisFloorValue, "Default");
        FloorSlider = UITool.GetUIComponent<Slider>(RectDisFloorValue, "Slider");
        FloorTitle = UITool.GetUIComponent<Text>(RectDisFloorValue, "Slider/HandleSlideArea/Handle/title");
        inputFloorValue = UITool.GetUIComponent<Button>(RectDisFloorValue, "Value");
        disFloorValue = UITool.GetUIComponent<Text>(RectDisFloorValue, "Value/value");
        disFloorUnit = UITool.GetUIComponent<Text>(RectDisFloorValue, "Value/unit");
        rotation = UITool.AddUIComponent<ToggleButton>(bottomItem, "rotation");
        RectRotation = UITool.GetUIComponent<RectTransform>(bottomItem, "rotationValue");
        rotationDefault = UITool.GetUIComponent<Button>(RectRotation,"Default");
        rotationAngle = UITool.GetUIComponent<Button>(RectRotation, "rotation/rotation");
        rotationSlider = UITool.GetUIComponent<Slider>(RectRotation,"Slider");
        sliderValue = UITool.GetUIComponent<Text>(RectRotation, "Slider/HandleSlideArea/Handle/title");
        inputRotation = UITool.GetUIComponent<Button>(RectRotation,"Value");
        rotationValue = UITool.GetUIComponent<Text>(RectRotation,"Value/value");
        group = UITool.AddUIComponent<ToggleButton>(bottomItem, "group");
        m_delete = UITool.GetUIComponent<Button>(bottomItem, "delete");
        collect = UITool.GetUIComponent<Button>(bottomItem, "collect");
        m_show = UITool.GetUIComponent<Button>(bottomItem, "show");

        Transform optionMenu = UITool.GetUIComponent<Transform>(skin.transform, "BottomMenu");
        backGroundThereD = UITool.GetUIComponent<Button>(optionMenu, "3DBackGround");
        RectSelectWallSide = UITool.GetUIComponent<RectTransform>(optionMenu, "3DSelectWall");
        selectWall = RectSelectWallSide.GetComponent<Button>();
        wall2 = UITool.GetUIComponent<Button>(RectSelectWallSide,"Wall2");
        RectWall2 = wall2.GetComponent<RectTransform>();
        wall2optionList = UITool.GetUIComponent<RectTransform>(RectWall2,"opertation");

        option1 = UITool.GetUIComponent<Button>(wall2optionList,"option1");

        option2 = UITool.GetUIComponent<Button>(wall2optionList, "option2");
        option3 = UITool.GetUIComponent<Button>(wall2optionList, "option3");
        option4 = UITool.GetUIComponent<Button>(wall2optionList, "option4");
        option5 = UITool.GetUIComponent<Button>(wall2optionList, "option5");
        option6 = UITool.GetUIComponent<Button>(wall2optionList, "option6");
        setValue = UITool.GetUIComponent<Button>(wall2optionList,"SetValue");
        wallValue = UITool.GetUIComponent<Text>(wall2optionList,"SetValue/value");
        wall4 = UITool.GetUIComponent<Button>(RectSelectWallSide, "Wall4");
        RectWall4 = wall4.GetComponent<RectTransform>();
        wall4optionList = UITool.GetUIComponent<RectTransform>(RectWall4, "opertation");
        optionOne = UITool.GetUIComponent<Button>(wall4optionList, "optionOne");
        optionTwo = UITool.GetUIComponent<Button>(wall4optionList, "optionTwo");
        optionThere = UITool.GetUIComponent<Button>(wall4optionList, "optionThere");
        optionFour = UITool.GetUIComponent<Button>(wall4optionList, "optionFour");
        optionFive = UITool.GetUIComponent<Button>(wall4optionList, "optionFive");
        optionSix = UITool.GetUIComponent<Button>(wall4optionList, "optionSix");
        optionSeven = UITool.GetUIComponent<Button>(wall4optionList, "optionSeven");
        setValue1 = UITool.GetUIComponent<Button>(wall4optionList, "SetValue");
        wallValue1 = UITool.GetUIComponent<Text>(setValue1.transform, "value");
        setValue2 = UITool.GetUIComponent<Button>(wall4optionList,"SetValue1");
        wallValue2 = UITool.GetUIComponent<Text>(setValue2.transform,"value");
        auto = UITool.GetUIComponent<Button>(wall4optionList,"Auto");
        wall5 = UITool.GetUIComponent<Button>(RectSelectWallSide, "wall5");


        operationList.Add(RectSetRoom.transform);
        operationList.Add(particulars.transform);
        operationList.Add(copy.transform);
        operationList.Add(l_r_mirroring.transform);
        operationList.Add(u_d_mirroring.transform);
        operationList.Add(replace.transform);
        operationList.Add(disWall.transform);
        operationList.Add(disFloor.transform);
        operationList.Add(group.transform);
        operationList.Add(m_delete.transform);
        operationList.Add(collect.transform);
        operationList.Add(m_show.transform);
        operationList.Add(backGroundThereD.transform);
        operationList.Add(RectSelectWallSide);
        operationList.Add(rotation.transform);
    }
}
