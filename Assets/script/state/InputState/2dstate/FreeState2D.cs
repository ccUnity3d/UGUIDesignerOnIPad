
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 自由状态
/// </summary>
public class FreeState2D : InputState2D {

    public const string NAME = "FreeState";
    private Collider2D colli = null;

    public FreeState2D() : base(NAME)
    {

    }

    public override void enter()
    {
        helpView2D.sleep();
        base.enter();
        RefreshView();
    }

    public override void mUpdate()
    {
        base.mUpdate();
        if (Input.GetMouseButtonDown(0) && uguiHitUI.uiHited == false)
        {
            Vector2 v2 = GetScreenToWorldPos(Input.mousePosition);
            colli = Physics2D.OverlapPoint(v2);
        }
        if (colli != null && Input.GetMouseButtonUp(0))
        {
            Vector2 v2 = GetScreenToWorldPos(Input.mousePosition);
            Collider2D colli2 = Physics2D.OverlapPoint(v2);
            if (colli2 != colli)
            {
                colli = null;
                return;
            }
            ObjView view = colli.GetComponent<ObjView>();
            if (view == null)
            {
                return;
            }
            string stateName = view.GetState();
            SelectState state = getSelectState(stateName);
            if (state == null)
            {
                Debug.LogError("stateName = {0}未注册！" + stateName);
                return;
            }
            state.viewTarget = view;
            setState(stateName);
        }
    }

    public override void mPhoneUpdate()
    {
        base.mPhoneUpdate();
        if (Input.touchCount != 1 || uguiHitUI.uiHited == true)
        {
            return;
        }
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                Vector2 v2 = GetScreenToWorldPos(touch.position);
                colli = Physics2D.OverlapPoint(v2);
                return;
            case TouchPhase.Moved:
                if (colli != null) colli = null;
                return;
            case TouchPhase.Ended:
                break;
            case TouchPhase.Stationary:
            case TouchPhase.Canceled:
            default:
                return;

        }
        
        if (colli != null)
        {
            Vector2 v2 = GetScreenToWorldPos(Input.mousePosition);
            Collider2D colli2 = Physics2D.OverlapPoint(v2);
            if (colli2 != colli)
            {
                colli = null;
                return;
            }
            ObjView view = colli.GetComponent<ObjView>();
            if (view == null)
            {
                return;
            }
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
    
    public override void exit()
    {
        colli = null;
        base.exit();
    }

}
