using UnityEngine;
using System.Collections;

public class InputState2D : InputState
{
    public InputState2D(string Name) : base(Name)
    {

    }

    protected WallInputType wallInputType = WallInputType.Center;
    /// <summary>
    /// 操作产生的临时辅助数据
    /// </summary>
    protected InputHelperData helperData
    {
        get {
            return InputHelperData.Instance;
        }
    }
    protected InputPhoneHelperData phoneHelperData
    {
        get {
            return InputPhoneHelperData.Instance;
        }
    }
    /// <summary>
    /// 辅助编辑视图
    /// </summary>
    protected HelpView2D helpView2D
    {
        get {
            return HelpView2D.Instance;
        }
    }

    protected LineHelpFunc linefunc
    {
        get {
            return LineHelpFunc.Instance;
        }
    }
    protected WallHelpFunc wallfunc
    {
        get {
            return WallHelpFunc.Instance;
        }
    }
    protected RoomHelpFunc roomfunc
    {
        get {
            return RoomHelpFunc.Instance;
        }
    }


    public override void enter()
    {
        base.enter();
    }

    public override void mUpdate()
    {
        base.mUpdate();
    }

    public override void mPhoneUpdate()
    {
        base.mPhoneUpdate();
    }

    public override void exit()
    {
        base.exit();
    }


}
