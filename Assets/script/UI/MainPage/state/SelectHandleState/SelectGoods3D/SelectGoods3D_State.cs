using UnityEngine;
using System.Collections;
using System;

public class SelectGoods3D_State : MySimpleState , IReady{

    protected OriginalInputData data
    {
        get
        {
            return OriginalInputData.Instance;
        }
    }
    protected InputStateMachine inputMachine
    {
        get
        {
            return InputStateMachine.Instance;
        }
    }
    protected SelectGoods3D_StateMachine machine
    {
        get
        {
            return SelectGoods3D_StateMachine.Instance;
        }
    }

    protected MainPageUIController mainPageContr
    {
        get
        {
            return MainPageUIController.Instance;
        }
    }

    protected ProductData target
    {
        get
        {
            return machine.selectGoods3DState.target;
        }
    }
    protected Product targetProduct
    {
        get
        {
            return machine.selectGoods3DState.targetProduct;
        }
    }

    protected DefaultSettings defaultSet { get { return DefaultSettings.Instance; } }

    /// <summary>
    /// 2D墙设置视图
    /// </summary>
    protected View2D view2D { get { return View2D.Instance; } }
    protected View3D view3D { get { return View3D.Instance; } }

    //public virtual bool needCloseGoodsUpdate
    //{
    //    get {
    //        return false;
    //    }
    //}

    protected bool _saved = false;

    public virtual bool needCloseUpdate
    {
        get { return false; }
    }

    protected bool saved
    {
        get {
            return _saved;
        }
        set {
            _saved = value;
            if (_saved == true)
            {
                save();
            }
        }
    }

    public override void enter()
    {
        saved = false;
        base.enter();
    }

    public override void exit()
    {
        base.exit();
    }

    protected void setState(string name)
    {
        machine.setState(name);
    }

    protected void save()
    {
        machine.selectGoods3DState.save();
    }

    protected void RefreshView()
    {
        machine.selectGoods3DState.Refresh();
        //if (inputMachine.currentInputIs2D)
        //{
        //    view2D.RefreshView();
        //}
        //else {
        //    view3D.RefreshView();
        //}
    }

    public virtual void Ready(GameObject skin)
    {
        //throw new NotImplementedException();
    }
}

