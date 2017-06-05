using UnityEngine;
using System.Collections;

/// <summary>
/// 忘记干啥的了
/// </summary>
public class SelectItemState3D : SelectState3D
{
    public const string NAME = "SelectItemState3D";
    public SelectItemState3D() : base(NAME)
    {

    }

    public override void enter()
    {
        base.enter();
    }

    protected override GoodsType getGoodsType()
    {
        return GoodsType.Default;
    }

    public override void mUpdate()
    {
        base.mUpdate();
    }

    public override void exit()
    {
        base.exit();
    }
}