using UnityEngine;
using System.Collections;
using clayui;
using foundation;
using System.Collections.Generic;

public class FreeState3D : InputState{

    public const string NAME = "FreeState3D";
    public FreeState3D() : base(NAME)
    {

    }

    private Collider colli = null;
    private List<RaycastHit> raycastList = new List<RaycastHit>();
    private RaycastHit[] raycastArr= new RaycastHit[20];
    private RaycastMeshFunc raycastFunc
    {
        get
        {
            return RaycastMeshFunc.Instance;
        }
    }

    public override void enter()
    {
        base.enter();
        RefreshView();
    }

    public override void mUpdate()
    {
        base.mUpdate();
        
        if (Input.GetMouseButtonDown(0) && uguiHitUI.uiHited == false)
        {
            colli = null;
            //RaycastHit hit;
            Ray ray = inputCamera.ScreenPointToRay(Input.mousePosition);
            //bool hited = Physics.Raycast(ray, out hit);
            //if (hited == true)
            //{
            //    colli = hit.collider;
            //}
            int count = Physics.RaycastNonAlloc(ray, raycastArr);
            if (count > 0)
            {
                raycastList.Clear();
                for (int i = 0; i < count; i++)
                {
                    raycastList.Add(raycastArr[i]);
                }
                //raycastList.Sort(
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
            //if (colli == null)
            //{
            //    return;
            //}
        }
        if (colli != null && Input.GetMouseButtonUp(0))
        {
            Collider colli2 = null;
            //RaycastHit hit;
            Ray ray = inputCamera.ScreenPointToRay(Input.mousePosition);
            //bool hited = Physics.Raycast(ray, out hit);
            //if (hited == true)
            //{
            //    colli2 = hit.collider;
            //}
            int count = Physics.RaycastNonAlloc(ray, raycastArr);

            if (count > 0)
            {
                raycastList.Clear();
                for (int i = 0; i < count; i++)
                {
                    raycastList.Add(raycastArr[i]);
                }
                // raycastList.Sort(
                //   delegate (RaycastHit hit1, RaycastHit hit2)
                //   {
                //       if (hit1.distance > hit2.distance) return 1;
                //       else return -1;
                //   }
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
                if (count > 0 && colli2 == null)
                {
                    colli2 = raycastList[0].collider;
                }
            }

            if (colli2 == null)
            {
                return;
            }

            if (colli2 != colli)
            {
                colli = null;
                return;
            }
            ObjView view;
            if (colli.name == "colli")
            {
                view = colli.transform.parent.GetComponent<ObjView>();
            }
            else {
                view = colli.GetComponent<ObjView>();
            }
            if (view == null)
            {
                return;
            }
            string stateName = view.GetState();
            SelectState3D state = getSelectState3D(stateName);
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
                {
                    colli = null;
                    //RaycastHit hit;
                    Ray ray = inputCamera.ScreenPointToRay(touch.position);
                    //bool hited = Physics.Raycast(ray, out hit);
                    //if (hited == true)
                    //{
                    //    colli = hit.collider;
                    //}
                    int count = Physics.RaycastNonAlloc(ray, raycastArr);
                    if (count > 0)
                    {
                        raycastList.Clear();
                        for (int i = 0; i < count; i++)
                        {
                            raycastList.Add(raycastArr[i]);
                        }
                        //raycastList.Sort(
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
                }
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
            Collider colli2 = null;
            //RaycastHit hit;
            Ray ray = inputCamera.ScreenPointToRay(touch.position);
            //bool hited = Physics.Raycast(ray, out hit);
            //if (hited == true)
            //{
            //    colli2 = hit.collider;
            //}
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
                            colli2 = raycastList[i].collider;
                            rayhitdis = dis;
                        }
                    }
                }
                if (count > 0 && colli2 == null)
                {
                    colli2 = raycastList[0].collider;
                }
            }
            
            if (colli2 != colli)
            {
                colli = null;
                return;
            }
            ObjView view;
            if (colli.name == "colli")
            {
                view = colli.transform.parent.GetComponent<ObjView>();
            }
            else {
                view = colli.GetComponent<ObjView>();
            }
            if (view == null)
            {
                return;
            }
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

    public override void exit()
    {
        colli = null;
        base.exit();
    }

}
