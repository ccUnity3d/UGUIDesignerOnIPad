using UnityEngine;
using System.Collections;

public class RoomView : ObjView
{
    public RoomData data;

    public override void SetData(ObjData data)
    {
        base.SetData(data);
        this.data = data as RoomData;
    }

    public override string GetState()
    {
        return SelectRoomState.NAME;
    }
}
