using UnityEngine;
using System.Collections;

public class SetMaterialState : InputState
{
    public const string NAME = "SetMaterialState";
    public SetMaterialState() : base(NAME)
    {

    }

    public MaterialItemData material;
    private Collider colli = null;
    private IMaterialView _overView = null;
    private IMaterialView overView
    {
        get { return _overView; }
        set {
            if (_overView == value)
            {
                return;
            }
            _overView = value;
            if (value == null)
            {
                view3D.selectObjData = null;
            }
            RefreshView();
        }
    }

    public override void enter()
    {
        base.enter();
        colli = null;
        overView = null;
    }

    public override void exit()
    {
        view3D.selectObjData = null;
        RefreshView();
        base.exit();
    }

    public override void mUpdate()
    {
        base.mUpdate();
        if (uguiHitUI.uiHited == true)
        {
            //Debug.Log("On uiHit");
            colli = null;
            overView = null;
            return;
        }
        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("On GetMouseButtonUp");
            if (overView != null)
            {
                undoHelper.save();
                MaterialData materialData = overView.getdata();
                materialData.Reset();
                materialData.seekId = material.seekId;
                materialData.textureURI = material.textureURL;
                RefreshView();
            }
            setState(FreeState3D.NAME);
            return;
        }
        Ray ray = inputCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hited = Physics.Raycast(ray, out hit);
        if (hited == false)
        {
            colli = null;
            overView = null;
            return;
        }
        if (colli == hit.collider)
        {
            return;
        }
        colli = hit.collider;
        overView = null;
        ObjView view;
        if (hit.collider.name == "colli")
        {
            view = hit.collider.transform.parent.GetComponent<ObjView>();
        }
        else
        {
            view = hit.collider.GetComponent<ObjView>();
        }
        if (view == null)
        {
            return;
        }
        else if (view is IMaterialView)
        {
            //Debug.Log("On "+ view);
            view3D.selectObjData = view.objData;
            overView = view as IMaterialView;
        }
    }

    public override void mPhoneUpdate()
    {
        base.mPhoneUpdate();
        if (uguiHitUI.uiHited == true)
        {
            //Debug.Log("On uiHit");
            colli = null;
            overView = null;
            return;
        }
        if (Input.touchCount == 0)
        {
            Debug.LogError("此状态不应该出现 Input.touchCount == 0");
            setState(FreeState3D.NAME);
            return;
        }
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
        {
            //Debug.Log("On GetMouseButtonUp");
            if (overView != null)
            {
                undoHelper.save();
                MaterialData materialData = overView.getdata();
                materialData.Reset();
                materialData.seekId = material.seekId;
                materialData.textureURI = material.textureURL;
                RefreshView();
            }
            setState(FreeState3D.NAME);
            return;
        }
        Ray ray = inputCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;
        bool hited = Physics.Raycast(ray, out hit);
        if (hited == false)
        {
            colli = null;
            overView = null;
            return;
        }
        if (colli == hit.collider)
        {
            return;
        }
        colli = hit.collider;
        overView = null;
        ObjView view;
        if (hit.collider.name == "colli")
        {
            view = hit.collider.transform.parent.GetComponent<ObjView>();
        }
        else
        {
            view = hit.collider.GetComponent<ObjView>();
        }
        if (view == null)
        {
            return;
        }
        else if (view is IMaterialView)
        {
            //Debug.Log("On "+ view);
            view3D.selectObjData = view.objData;
            overView = view as IMaterialView;
        }
    }

}
