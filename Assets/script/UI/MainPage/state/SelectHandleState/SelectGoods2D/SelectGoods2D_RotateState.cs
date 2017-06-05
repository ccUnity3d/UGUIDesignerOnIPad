using UnityEngine;
using System.Collections;
using System;

public class SelectGoods2D_RotateState : SelectGoods2D_State {

    protected LineHelpFunc linefunc
    {
        get {
            return LineHelpFunc.Instance;
        }
    }
    //private ToggleButton toggleButton;

    private OptionsPage optionPage { get { return OptionsPage.Instance; } }
    public override bool needCloseSelectGoodsState2DUpdate
    {
        get
        {
            return true;
        }
    }

    /// <summary>
    /// 一物体位置为圆心
    /// </summary>
    private Vector2 center;
    /// <summary>
    /// 初始位置
    /// </summary>
    private Vector2 startPos;
    /// <summary>
    /// 起始角度
    /// </summary>
    private float startAngle = 0;
    
    /// <summary>
    /// 实时位置
    /// </summary>
    private Vector2 pos;
    private Vector2 oldPos = Vector2.zero;
    private ProductData productData = null;
    private GameObject rotateObj;
    private ObjView viewTarget;
    private Vector3 targetSize;
    private Vector3 targetOffset;

    public override void enter()
    {
        base.enter();

        //if (toggleButton.onDown == false)
        //{
        //    setState(EditTypeOnSelect.Free);
        //    UITool.SetActionFalse(optionPage.RectRotation.gameObject);
        //    return;
        //}
        //UITool.SetActionTrue(optionPage.RectRotation.gameObject);

        productData = machine.selectGoodsState2D.target;
        viewTarget = machine.selectGoodsState2D.viewTarget;
        rotateObj = machine.selectGoodsState2D.rotateObj;
        Vector3 size = machine.selectGoodsState2D.targetProduct.size;
        targetSize = size.x * Vector3.right + size.z * Vector3.up;
        targetOffset = size.x / viewTarget.transform.localScale.x * Vector3.right 
            + size.z / viewTarget.transform.localScale.y * Vector3.up;
        center = getV2(productData.position);
        startAngle = productData.rotate;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount < 1)
            {
                setState(EditTypeOnSelect.Free);
                return;
            }
            oldPos = startPos = inputCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
            MyTickManager.Add(phoneUpdate);
        }
        else {
            oldPos = startPos = inputCamera.ScreenToWorldPoint(Input.mousePosition);
            MyTickManager.Add(update);
        }
    }

    private Vector2 getV2(Vector3 v3)
    {
        return Vector3.right * v3.x + Vector3.up * v3.z;
    }

    private void update()
    {
        if (Input.GetMouseButton(0) == false)
        {
            setState(EditTypeOnSelect.Free);
            return;
        }
        pos = inputCamera.ScreenToWorldPoint(Input.mousePosition);
        Rotate();
    }

    private void phoneUpdate()
    {
        if (Input.touchCount == 0)
        {
            setState(EditTypeOnSelect.Free);
            return;
        }
        Touch touch = Input.GetTouch(0);
        pos = inputCamera.ScreenToWorldPoint(touch.position);
        Rotate();
    }

    private void Rotate()
    {
        if (pos != oldPos)
        {
            oldPos = pos;
            float angle = linefunc.GetClockAngle(startPos, center, pos);
            //最终使用角
            float rotate = startAngle + angle;
            if (rotate >= 360)
            {
                rotate -= 360;
            }
            else if (rotate < 0)
            {
                rotate += 360;
            }

            //45度5%误差内吸附
            float multi = rotate / 45;
            int times = (int)multi;
            float other = multi - times;
            if (other > 0.9f)
            {
                rotate = (times + 1) * 45;
            }
            else if (other < 0.1f)
            {
                rotate = times * 45;
            }

            productData.rotate = rotate;
            RefreshView();
            rotateObj.transform.position = viewTarget.transform.TransformPoint(targetOffset / 2 - Vector3.forward);
        }
    }

    public override void exit()
    {
        //toggleButton.onDown = false;
        //UITool.SetActionFalse(optionPage.RectRotation.gameObject);

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            MyTickManager.Remove(phoneUpdate);
        }
        else {
            MyTickManager.Remove(update);
        }
        base.exit();
    }
    public override void Ready(GameObject skin)
    {
        base.Ready(skin);
        //toggleButton = optionPage.rotation;
    }
}
