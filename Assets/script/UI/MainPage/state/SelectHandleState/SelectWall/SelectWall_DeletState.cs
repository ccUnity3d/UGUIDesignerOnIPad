using System.Collections.Generic;

public class SelectWall_DeletState : SelectWall_State
{
    public override void enter()
    {
        base.enter();
        if (target != null)
        {
            undoHelper.save();
            data.RemoveWall(target);
            List<Point> list1 = data.GetNearestPoints(target.point1);
            if (list1.Count == 2)
            {
                WallData wall0 = data.GetWall(list1[0], target.point1);
                WallData wall1 = data.GetWall(list1[1], target.point1);
                bool isParallel = wallfunc.IsParallel(wall0, wall1);
                if (isParallel)
                {
                    data.RemovePoint(target.point1);
                    data.AddWall(list1[0], list1[1], wall0.height, wall0.width);
                }
            }
            List<Point> list2 = data.GetNearestPoints(target.point2);
            if (list2.Count == 2)
            {
                WallData wall0 = data.GetWall(list2[0], target.point2);
                WallData wall1 = data.GetWall(list2[1], target.point2);
                bool isParallel = wallfunc.IsParallel(wall0, wall1);
                if (isParallel)
                {
                    data.RemovePoint(target.point2);
                    data.AddWall(list2[0], list2[1], wall0.height, wall0.width);
                }
            }
            roomfunc.ForceRefreshRoomData(data);
            RefreshView();
            inputMachine.setState(FreeState2D.NAME);
            //machine.setState(EditTypeOnSelect.Free);
        }
    }

    public override void exit()
    {
        base.exit();
    }

}
