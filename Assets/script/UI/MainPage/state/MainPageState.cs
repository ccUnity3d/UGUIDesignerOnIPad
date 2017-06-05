using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class MainPageState : MySimpleState
{
    protected OriginalInputData originalInputData
    {
        get {
            return OriginalInputData.Instance;
        }
    }

    protected MainPageStateMachine machine
    {
        get {
            return MainPageStateMachine.Instance;
        }
    }
    protected MainPageUIController controller
    {
        get {
            return MainPageUIController.Instance;
        }
    }
    protected MainPage mainpage
    {
        get {
            return controller.mainpage;
        }
    }
    protected MainPageUIData mainpageData
    {
        get
        {
            return controller.mainpageData;
        }
    }
    protected EventSystem eventSystem
    {
        get {
            return EventSystem.current;
        }
    }

    protected UndoHelper undoHelper
    {
        get {
            return UndoHelper.Instance;
        }
    }
    protected InputStateMachine inputMachine
    {
        get {
            return InputStateMachine.Instance;
        }
    }
    protected View2D view2D
    {
        get {
            return View2D.Instance;
        }
    }
    protected View3D view3D
    {
        get {
            return View3D.Instance;
        }
    }

    //public IPage getPanel
    //{
    //    get
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public override void enter()
    {
        base.enter();
    }
    //public override void update()
    //{
    //    base.update();
    //}
    public override void exit()
    {
        base.exit();
    }

    protected void setState(string name)
    {
        machine.setState(name);
    }

    protected void showClickEffect(GameObject go)
    {
        go.SetActive(true);
        MyCallLater.Add(0.1f, hideImage, go); 
    }
    void hideImage(object data)
    {
        GameObject go = (GameObject)data;
        go.SetActive(false);
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

    /// <summary>
    /// 0取反  -1关闭  1打开
    /// </summary>
    /// <param name="pageType"></param>
    /// <param name="type"></param>
    protected void TogglePage(PageType pageType, int type)
    {
        switch (type)
        {
            case -1:
                UIManager.Close(pageType);
                break;
            case 1:
                UIManager.Open(pageType);
                break;
            default:
                UIManager.Toggle(pageType);
                break;
        }
    }

    public virtual void Ready()
    {

    }
    
}
