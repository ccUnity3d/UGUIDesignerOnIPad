using UnityEngine;
using System.Collections;
using System;

public class SelectGoods2D_State : MySimpleState , IReady{

    protected OriginalInputData data
    {
        get
        {
            return OriginalInputData.Instance;
        }
    }
    protected InputStateMachine inputMachine
    {
        get {
            return InputStateMachine.Instance;
        }
    }
    protected SelectGoods2D_StateMachine machine
    {
        get {
            return SelectGoods2D_StateMachine.Instance;
        }
    }

    protected MainPageUIController mainPageContr
    {
        get
        {
            return MainPageUIController.Instance;
        }
    }
    protected MainPageUIData mainpageData
    {
        get
        {
            return mainPageContr.mainpageData;
        }
    }
    protected UndoHelper undoHelper
    {
        get {
            return UndoHelper.Instance;
        }
    }
    protected DefaultSettings defaultSet { get { return DefaultSettings.Instance; } }
    /// <summary>
    /// 2D墙设置视图
    /// </summary>
    protected View2D view2D {get { return View2D.Instance; }}
    protected View3D view3D {get { return View3D.Instance; } }
    protected Camera inputCamera = Camera.main;

    public virtual bool needCloseSelectGoodsState2DUpdate
    {
        get { return false; }
    }

    public override void enter()
    {
        base.enter();
    }

    public override void exit()
    {
        base.exit();
    }

    public void setState(string editorType)
    {
        machine.setState(editorType);
    }


    protected ProductData target
    {
        get {
            return machine.selectGoodsState2D.target;
        }
    }
    protected GoodsVO targetVO
    {
        get
        {
            return machine.selectGoodsState2D.targetVO;
        }
    }
    protected Product targetProduct
    {
        get
        {
            return machine.selectGoodsState2D.targetProduct;
        }
    }
    protected GameObject targetObj
    {
        get
        {
            return machine.selectGoodsState2D.targetObj;
        }
    }

    protected void save()
    {
        machine.selectGoodsState2D.save();
    }

    protected void RefreshView()
    {
        if (inputMachine.currentInputIs2D)
        {
            view2D.RefreshView();
        }
        else {
            view3D.RefreshView();
        }
    }

    public virtual void Ready(GameObject skin)
    {
        //throw new NotImplementedException();
    }
}
