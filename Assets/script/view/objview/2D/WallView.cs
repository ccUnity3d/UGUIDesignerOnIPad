using UnityEngine;
using System.Collections;

public class WallView : ObjView {

    public WallData data;

    public override void SetData(ObjData data)
    {
        base.SetData(data);
        this.data = data as WallData;
    }

    public override string GetState()
    {
        return SelectWallState.NAME;
    }
}
