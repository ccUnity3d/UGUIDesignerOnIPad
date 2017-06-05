using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SelectState3D : InputState {

    public ObjView viewTarget;
    public ObjData viewData;
    protected bool changed = false;
    public bool canMove = false;
    public bool onhandle = false;
    protected bool saved
    {
        get
        {
            return _saved;
        }
        set
        {
            _saved = value;
            if (_saved == true) save();
        }
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


    protected OptionsPage optionsPage
    {
        get {
            return OptionsPage.Instance;
        }
    }

    protected IStateMachine selectMachine;

    private bool _saved = false;
    private Collider colli;
    private Vector2 dowmPos;
    private bool moved;

    private List<RaycastHit> raycastList = new List<RaycastHit>();
    private RaycastHit[] raycastArr= new RaycastHit[20];

    private RaycastMeshFunc raycastFunc
    {
        get
        {
            return RaycastMeshFunc.Instance;
        }
    }

    public SelectState3D(string name) : base(name)
    {
        
    }

    protected virtual GoodsType getGoodsType()
    {
        return GoodsType.Default;
    }

    public override void enter()
    {
        base.enter();
        changed = false;
        ObjData data = viewData;
        if (viewTarget != null)
        {
            data = viewTarget.objData;
        }
        optionsController.ShowSelect(getGoodsType(), data);
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

    protected void onTargetChange(ObjData newtarget)
    {
        viewData = newtarget;
        if (viewTarget != null) viewTarget.objData = newtarget;
        view2D.selectObjData = newtarget;
        view3D.selectObjData = newtarget;
        RefreshView();
    }

    public override void mUpdate()
    {
        base.mUpdate();
        if (Input.GetMouseButtonDown(0) == true && uguiHitUI.uiHited == false)
        {
            canMove = false;
            onhandle = false;
            dowmPos = Input.mousePosition;

            colli = null;
            Ray ray = inputCamera.ScreenPointToRay(dowmPos);
            //RaycastHit hit;
            //bool hited = Physics.Raycast(ray, out hit);
            //if (hited == false)
            //{
            //    colli = null;
            //    //setState(FreeState3D.NAME);
            //    return;
            //}
            //colli = hit.collider;
            int count = Physics.RaycastNonAlloc(ray, raycastArr);
            if (count > 0)
            {
                raycastList.Clear();
                for (int i = 0; i < count; i++)
                {
                    raycastList.Add(raycastArr[i]);
                }
                // raycastList.Sort(
                //    delegate (RaycastHit hit1, RaycastHit hit2)
                //    {
                //        if (hit1.distance > hit2.distance) return 1;
                //        else return -1;
                //    }
                //);
                SortDis(raycastList);
                float rayhitdis = float.MaxValue;
                for (int i = 0; i < count; i++)
                {
                    if (colli != null && raycastList[i].distance > raycastList[0].distance + 5)
                    {
                        break;
                    }
                    float dis;
                    bool hit = raycastFunc.RaycastMesh(Input.mousePosition, raycastList[i].collider.gameObject, out dis);
                    if (hit == true)
                    {
                        if (dis < rayhitdis)
                        {
                            colli = raycastList[i].collider;
                            rayhitdis = dis;
                        }
                    }
                }
                if (colli == null)
                {
                    colli = raycastList[0].collider;
                }
            }
            if (colli == null)
            {
                return;
            }

            HandleView handle = colli.GetComponent<HandleView>();
            if (handle != null)
            {
                handle.SetState(selectMachine);
                onhandle = true;
                return;
            }
            ObjView view;
            if (colli.name == "colli")
            {
                view = colli.transform.parent.GetComponent<ObjView>();
            }
            else
            {
                view = colli.GetComponent<ObjView>();
            }

            if(view == null)
            {

            }
            if (view == viewTarget)
            {
                canMove = true;
            }

        }
        else if (Input.GetMouseButtonUp(0) == true )
        {
            onhandle = false;
            if (uguiHitUI.uiHited == true)
            {
                return;
            }
            Collider colli2;
            Vector2 upPos = Input.mousePosition;
            if (Vector2.Distance(dowmPos, upPos) > 5f)
            {
                return;
            }

            colli2 = null;
            Ray ray = inputCamera.ScreenPointToRay(upPos);
            //RaycastHit hit;
            //bool hited = Physics.Raycast(ray, out hit);
            //if (hited == false)
            //{
            //    colli2 = null;
            //    if (colli == colli2)
            //    {
            //        changed = true;
            //        setState(FreeState3D.NAME);
            //    }
            //    return;
            //}
            //colli2 = hit.collider;
            int count = Physics.RaycastNonAlloc(ray, raycastArr);
            if (count > 0)
            {
                raycastList.Clear();
                for (int i = 0; i < count; i++)
                {
                    raycastList.Add(raycastArr[i]);
                }
                // raycastList.Sort(
                //    delegate (RaycastHit hit1, RaycastHit hit2)
                //    {
                //        if (hit1.distance > hit2.distance) return 1;
                //        else return -1;
                //    }
                //);
                SortDis(raycastList);
                float rayhitdis = float.MaxValue;
                for (int i = 0; i < count; i++)
                {
                    if (colli != null && raycastList[i].distance > raycastList[0].distance + 5)
                    {
                        break;
                    }
                    float dis;
                    bool hit = raycastFunc.RaycastMesh(Input.mousePosition, raycastList[i].collider.gameObject, out dis);
                    if (hit == true)
                    {
                        if (dis < rayhitdis)
                        {
                            colli2 = raycastList[i].collider;
                            rayhitdis = dis;
                        }
                    }
                }
                if (colli2 == null)
                {
                    colli2 = raycastList[0].collider;
                }
            }

            if (colli2 == null)
            {
                if (colli == colli2)
                {
                    changed = true;
                    setState(FreeState3D.NAME);
                }
                return;
            }

            if (colli2 != colli)
            {
                return;
            }

            HandleView handle = colli.GetComponent<HandleView>();
            if (handle != null)
            {
                return;
            }

            ObjView view;
            if (colli2.name == "colli")
            {
                view = colli2.transform.parent.GetComponent<ObjView>();
            }
            else
            {
                view = colli2.GetComponent<ObjView>();
            }
            if (view == null)
            {
                changed = true;
                setState(FreeState3D.NAME);
                return;
            }
            else if (view == viewTarget)
            {
                //canMove = true;
                //return;
            }
            else
            {
                changed = true;
                string stateName = view.GetState();
                SelectState3D state = getSelectState3D(stateName);
                if (state == null)
                {
                    return;
                }
                state.viewTarget = view;
                setState(stateName);
            }
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
            onhandle = false;
            moved = false;
            canMove = false;

            colli = null;
            Ray ray = inputCamera.ScreenPointToRay(touch.position);
            //RaycastHit hit;
            //bool hited = Physics.Raycast(ray, out hit);
            //if (hited == false)
            //{
            //    colli = null;
            //    return;
            //}
            //colli = hit.collider;
            int count = Physics.RaycastNonAlloc(ray, raycastArr);
            if (count > 0)
            {
                raycastList.Clear();
                for (int i = 0; i < count; i++)
                {
                    raycastList.Add(raycastArr[i]);
                }
                // raycastList.Sort(
                //    delegate (RaycastHit hit1, RaycastHit hit2)
                //    {
                //        if (hit1.distance > hit2.distance) return 1;
                //        else return -1;
                //    }
                //);
                SortDis(raycastList);
                float rayhitdis = float.MaxValue;
                for (int i = 0; i < count; i++)
                {
                    if (colli != null && raycastList[i].distance > raycastList[0].distance + 5)
                    {
                        break;
                    }
                    float dis;
                    bool hit = raycastFunc.RaycastMesh(touch.position, raycastList[i].collider.gameObject, out dis);
                    if (hit == true)
                    {
                        if (dis < rayhitdis)
                        {
                            colli = raycastList[i].collider;
                            rayhitdis = dis;
                        }
                    }
                }
                if (colli == null)
                {
                    colli = raycastList[0].collider;
                }
            }
            if (colli == null)
            {
                return;
            }

            HandleView handle = colli.GetComponent<HandleView>();
            if (handle != null)
            {
                handle.SetState(selectMachine);
                onhandle = true;
                return;
            }
            ObjView view;
            if (colli.name == "colli")
            {
                view = colli.transform.parent.GetComponent<ObjView>();
            }
            else
            {
                view = colli.GetComponent<ObjView>();
            }
            if (view != null && view == viewTarget)
            {
                canMove = true;
            }
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            moved = true;
        }
        else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
        {
            onhandle = false;
            if (moved == true)
            {
                return;
            }
            if (colli == null)
            {
                changed = true;
                setState(FreeState3D.NAME);
                return;
            }

            HandleView handle = colli.GetComponent<HandleView>();
            if (handle != null)
            {
                return;
            }

            ObjView view;
            if (colli.name == "colli")
            {
                view = colli.transform.parent.GetComponent<ObjView>();
            }
            else
            {
                view = colli.GetComponent<ObjView>();
            }
            if (view == null)
            {
                changed = true;
                setState(FreeState3D.NAME);
                return;
            }
            else if (view == viewTarget)
            {

            }
            else
            {
                changed = true;
                string stateName = view.GetState();
                SelectState3D state = getSelectState3D(stateName);
                if (state == null)
                {
                    return;
                }
                state.viewTarget = view;
                setState(stateName);
            }
        }
    }

    private List<RaycastHit> sortList = new List<RaycastHit>();
    /// <summary>
    /// 
    /// </summary>
    private void SortDis(List<RaycastHit> list)
    {
        sortList.Clear();
        int len = list.Count;
        for (int i = 0; i < len; i++)
        {
            RaycastHit minDis = list[0];
            for (int k = 1; k < list.Count; k++)
            {
                if (list[k].distance < minDis.distance)
                {
                    minDis = list[k];
                }
            }
            list.Remove(minDis);
            sortList.Add(minDis);
        }
        list.Clear();
        list.AddRange(sortList);
        sortList.Clear();
    }
    
}
