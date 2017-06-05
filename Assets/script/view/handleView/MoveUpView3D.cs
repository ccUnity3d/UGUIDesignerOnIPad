using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class MoveUpView3D : HandleView {

    private IStateMachine selectMachine;

    public void SetData(IStateMachine selectMachine)
    {
        //RawImage image = transform.GetComponent<RawImage>();
        DrapButton button = gameObject.GetComponent<DrapButton>();
        if (button == null) button = gameObject.AddComponent<DrapButton>();
        button.onPointerDownDele = OnPointerDown;
        this.selectMachine = selectMachine;
    }

    private void OnPointerDown(PointerEventData obj)
    {
        if(selectMachine != null) SetState(selectMachine);
    }

    public override bool SetState(IStateMachine selectMachine)
    {
        selectMachine.setState(EditTypeOnSelect.MoveUp);
        return true;
    }

}
