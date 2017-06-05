using UnityEngine;
using System.Collections;

public class InputPhoneHelperData {

    private static InputPhoneHelperData instance;

    public static InputPhoneHelperData Instance
    {
        get
        {
            if (instance == null) instance = new InputPhoneHelperData();
            return instance;
        }
    }


    private Point _currentPoint = null;
    public Point currentPoint
    {
        get { return _currentPoint; }
        set
        {
            _currentPoint = value;
            View2D.Instance.selectObjData = _currentPoint;
            View2D.Instance.RefreshView();
        }
    }

    public void sleep()
    {
        //_currentPoint = null;
    }
}
