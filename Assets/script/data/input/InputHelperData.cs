using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InputHelperData {

    private static InputHelperData instance;

    public static InputHelperData Instance
    {
        get {
            if(instance == null)instance = new InputHelperData();
            return instance;
        }
    }

    private Point _currentPoint = null;
    public Point currentPoint
    {
        get { return _currentPoint; }
        set { _currentPoint = value;
            View2D.Instance.selectObjData = _currentPoint;
            View2D.Instance.RefreshView();
        }
    }
    
    public List<WallData> helpLineList = new List<WallData>();

    public bool mouseLineDisable = false;

    /// <summary>
    /// 布置墙的起点是否在墙上
    /// </summary>
    public OnWallType startOnWallType = OnWallType.NotOnWall;

    /// <summary>
    /// 布置墙的终点是否在墙上
    /// </summary>
    public OnWallType endOnWallType = OnWallType.NotOnWall;

    public void sleep()
    {
        helpLineList.Clear();
        startOnWallType = OnWallType.NotOnWall;
        endOnWallType = OnWallType.NotOnWall;
        currentPoint = null;
        mouseLineDisable = false;
}

}
