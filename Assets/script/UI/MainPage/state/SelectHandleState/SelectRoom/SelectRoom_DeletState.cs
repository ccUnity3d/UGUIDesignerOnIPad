using UnityEngine;
using System.Collections;

public class SelectRoom_DeletState : SelectRoom_State
{
    public override void enter()
    {
        base.enter();
        if (target == null)
        {
            inputMachine.setState(FreeState2D.NAME);
            return;
        }
        undoHelper.save();
        for (int i = 0; i < target.sideList.Count; i++)
        {
            WallSideData side = target.sideList[i];
            WallData wall = side.targetWall;
            RoomData room = data.WallSideOnRoom(wall.point1To2Data == side ? wall.point2To1Data : wall.point1To2Data);
            if (room == null)
            {
                data.RemoveWall(wall);
            }
        }
        roomfunc.ForceRefreshRoomData(data);
        RefreshView();
        inputMachine.setState(FreeState2D.NAME);
    }
    
    public override void exit()
    {
        base.exit();
    }
}
