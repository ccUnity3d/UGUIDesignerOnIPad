using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetLengthHandleView : HandleView {
    /// <summary>
    /// 更改长度边
    /// </summary>
    public WallSideData data;
    public bool ForT = false;
    /// <summary>
    /// 同房间的另一边
    /// </summary>
    public WallSideData otherdata;

    public Button inputButton;
    public Button ok;
    public Text text;
    private SelectWall_StateMachine machine;
    private SelectWall_LengthState state;

    public List<Point> isParallelPointList = new List<Point>();
    private WallHelpFunc wallFunc
    {
        get { return WallHelpFunc.Instance; }
    }
    private Mode2DPrefabs prefabs
    {
        get { return Mode2DPrefabs.Instance; }
    }

    public void SetData(WallSideData wallside)
    {
        this.data = wallside;
        text.text = wallFunc.GetSideLength(wallside).ToString();
        Vector3 center = wallFunc.GetPos(wallside);
        center = prefabs.mainCamera.WorldToScreenPoint(center);
        center = (Vector2)prefabs.uiCamera.ScreenToWorldPoint((Vector2)center);
        transform.position = center;
        
    }

    void Awake()
    {
        if (inputButton == null)
        {
            inputButton = transform.GetComponent<Button>();
            ok = transform.FindChild("OK").GetComponent<Button>();
            text = transform.FindChild("Text").GetComponent<Text>();
            inputButton.onClick.AddListener(OnInput);
            ok.onClick.AddListener(OnOK);
            machine = SelectWall_StateMachine.Instance;
            state = machine.getState(EditTypeOnSelect.WallLength) as SelectWall_LengthState;
        }
        ok.gameObject.SetActive(false);
    }

    private void OnInput()
    {
        bool set = SetState(machine);
        if (set == false) return;
        ok.gameObject.SetActive(true);
    }

    private void OnOK()
    {
        state.Ok();
        ok.gameObject.SetActive(false);
        machine.setState(EditTypeOnSelect.Free);
    }

    public override bool SetState(IStateMachine selectMachine)
    {
        if (state.bringData == this as object)
        {
            return false;
        }

        machine.setState(EditTypeOnSelect.WallLength);

        state.setData(this, Exit);

        return true;
    }

    public void Exit()
    {
        ok.gameObject.SetActive(false);
    }
}
