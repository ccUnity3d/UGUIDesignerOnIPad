using UnityEngine;
using System.Collections;
using foundation;
using System;

public class SelectState : InputState2D{

    public SelectState(string Name) : base(Name)
    {

    }

    public ObjView viewTarget;
    protected bool changed = false;

    private bool _canMove = false;
    protected bool canMove
    {
        get { return _canMove; }
        set {
            _canMove = value;
            //Debug.Log(value);
        }
    }

    protected bool saved
    {
        get {
            return _saved;
        }
        set {
            //Debug.LogWarning("_saved"+ value);
            _saved = value;
            if (_saved == true) save();
        }
    }
    protected IStateMachine selectMachine;
    protected virtual GoodsType getGoodsType()
    {
        return GoodsType.Default;
    }

    private Collider2D colli;
    private ObjView tempColiiView;
    private bool _saved = false;
    public override void enter()
    {
        base.enter();
        changed = false;
        optionsController.ShowSelect(getGoodsType(), viewTarget.objData);
    }

    protected void onTargetChange(ObjData newtarget)
    {
        viewTarget.objData = newtarget;
        view2D.selectObjData = newtarget;
        view3D.selectObjData = newtarget;
        RefreshView();
    }

    public override void exit()
    {
        view2D.selectObjData = null;
        view3D.selectObjData = null;
        RefreshView();
        optionsController.HideSelect();
        if (selectMachine != null) selectMachine.setState(EditTypeOnSelect.Free);
        base.exit();
    }
    
    public void save()
    {
        if (viewTarget == null || viewTarget.objData == null)
        {
            Debug.LogWarning("viewTarget == null || viewTarget.objData == null");
            return;
        }
        undoHelper.save();
    }

    public override void mUpdate()
    {
        base.mUpdate();

        if (Input.GetMouseButtonDown(0) && uguiHitUI.uiHited == false)
        {
            canMove = false;
            Vector2 v2 = GetScreenToWorldPos(Input.mousePosition);
            colli = Physics2D.OverlapPoint(v2);
            if (colli == null)
            {
                setState(FreeState2D.NAME);
                return;
            }
            HandleView handle = colli.GetComponent<HandleView>();
            if (handle != null)
            {
                handle.SetState(selectMachine);
                return;
            }

            ObjView view = colli.GetComponent<ObjView>();
            if (view == null)
            {
                setState(FreeState2D.NAME);
                return;
            }
            else if (view == viewTarget)
            {
                canMove = true;
                return;
            }
            else
            {
                changed = true;
                string stateName = view.GetState();
                SelectState state = getSelectState(stateName);
                if (state == null)
                {
                    return;
                }
                state.viewTarget = view;
                setState(stateName);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            canMove = false;
        }
    }

    public override void mPhoneUpdate()
    {
        base.mPhoneUpdate();
        if (uguiHitUI.uiHited == true || Input.touchCount != 1)
        {
            return;
        }
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            canMove = false;
            Vector2 v2 = GetScreenToWorldPos(touch.position);
            colli = Physics2D.OverlapPoint(v2);
            if (colli == null)
            {
                setState(FreeState2D.NAME);
                return;
            }
            HandleView handle = colli.GetComponent<HandleView>();
            if (handle != null)
            {
                handle.SetState(selectMachine);
                return;
            }
            ObjView view = colli.GetComponent<ObjView>();
            if (view == null)
            {
                setState(FreeState2D.NAME);
                return;
            }
            else if (view == viewTarget)
            {
                canMove = true;
                return;
            }
            else
            {
                changed = true;
                tempColiiView = view;
            }
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            tempColiiView = null;
        }
        else if(touch.phase == TouchPhase.Ended)
        {
            if (canMove == true)
            {
                canMove = false;
                return;
            }

            if (tempColiiView == null)
            {
                setState(FreeState2D.NAME);
                return;
            }

            string stateName = tempColiiView.GetState();
            SelectState state = getSelectState(stateName);
            if (state == null)
            {
                return;
            }
            state.viewTarget = tempColiiView;
            setState(stateName);
        }
    }
}
