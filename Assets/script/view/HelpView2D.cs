using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpView2D: BaseView
{
    private InputHelperData helperData = InputHelperData.Instance;

    private List<GameObject> dashedLines = new List<GameObject>();
    private GameObject mouseLine;

    private GameObject showline;
    private GameObject showlinestart;
    private GameObject showlineend;

    private GameObject input;
    private InputField uinput;

    //辅助计算 鼠标方程的辅助类 不记录，不删除；
    private WallData tempWall = new WallData(new Point(), new Point());
    private bool canStart = false;
    private bool tempStart = false;
    private float lastDis = 0;

    private Valueable lastInputWrightValue = new Valueable();
    private string oldvalue = "";

    public HelpView2D()
    {

    }

    private static HelpView2D instance;
    public static HelpView2D Instance
    {
        get
        {
            if (instance == null) instance = new HelpView2D();
            return instance;
        }
    }

    public override void sleep()
    {
        helperData.sleep();
        for (int i = 0; i < dashedLines.Count;)
        {
            GameObject line = dashedLines[i];
            dashedLines.RemoveAt(i);
            GameObject.Destroy(line);
        }
        CloseMouse();
    }

    public void CloseMouse()
    {
        if (mouseLine != null)
        {
            GameObject.Destroy(mouseLine);
        }

        CloseShowLen();
    }

    private void OpenShowLen()
    {
        if (showline == null)
        {
            showline = prefabs.GetNewInstance_line().gameObject;
        }
        if (showlinestart == null)
        {
            showlinestart = prefabs.GetNewInstance_fork().gameObject;
        }
        if (showlineend == null)
        {
            showlineend = prefabs.GetNewInstance_fork().gameObject;
        }
        if (input == null)
        {
            input = prefabs.GetNewInstance_input().gameObject;
            uinput = input.GetComponent<InputField>();
        }

        if (canStart == true)
        {
            canStart = false;
            tempStart = true;
        }
    }
    private void CloseShowLen()
    {
        if (showline != null)
        {
            GameObject.Destroy(showline);
        }
        if (showlinestart != null)
        {
            GameObject.Destroy(showlinestart);
        }
        if (showlineend != null)
        {
            GameObject.Destroy(showlineend);
        }
        if (input != null)
        {
            GameObject.Destroy(input);
        }

        canStart = true;
    }

    public bool getInputChange()
    {
        if (uinput != null && oldvalue != uinput.text)
        {
            return true;
        }
        return false;
    }

    public void RefreshView(Vector2 v2)
    {
        RefreshGameObjs();
        for (int i = 0; i < helperData.helpLineList.Count; i++)
        {
            drawDashedLine(helperData.helpLineList[i], dashedLines[i]);
        }

        linefunc.SetLine(helperData.currentPoint.pos, v2, mouseLine, prefabs.size);

        if (Vector2.Distance(helperData.currentPoint.pos, v2) < 0.1f)
        {
            CloseShowLen();
            return;
        }
        OpenShowLen();

        tempWall.point1.pos = helperData.currentPoint.pos;
        tempWall.point2.pos = v2;
        Vector3 v2start = linefunc.GetDisPoint(tempWall.a, tempWall.b, tempWall.c5, tempWall.point1.pos.x, tempWall.point1.pos.y);
        Vector3 v2end = linefunc.GetDisPoint(tempWall.a, tempWall.b, tempWall.c5, tempWall.point2.pos.x, tempWall.point2.pos.y);
        float angle = linefunc.GetAngle(v2end - v2start);
        v2start.z = showlinestart.transform.localPosition.z;
        v2end.z = showlineend.transform.localPosition.z;
        showlinestart.transform.localPosition = v2start;
        showlineend.transform.localPosition = v2end;
        showlinestart.transform.localRotation = Quaternion.Euler(Vector3.forward * angle);
        showlineend.transform.localRotation = Quaternion.Euler(Vector3.forward * angle);
        linefunc.SetLine(v2start, v2end, showline, prefabs.size);

        Vector3 center = (v2start + v2end) / 2;
        center = prefabs.mainCamera.WorldToScreenPoint(center);

        center = (Vector2)prefabs.uiCamera.ScreenToWorldPoint((Vector2)center);
        input.transform.position = center;
        float dis = Vector2.Distance(v2start, v2end);
        dis = Mathf.RoundToInt(dis * 1000);

        if (lastDis != dis)
        {
            uinput.text = dis.ToString();
            //float value = dis * ((int)defaultSetting.DefaultUnit / 1000f);
            //try
            //{
            //    string str = value.ToString();
            //    uinput.text = str;
            //}
            //catch (Exception e)
            //{
            //    Debug.LogError(value + e.ToString());
            //}
            lastDis = dis;

            uinput.ActivateInputField();
            uinput.selectionAnchorPosition = 0;
            uinput.selectionFocusPosition = uinput.text.Length;
            //uinput.isSelected = true;
            //uinput.selectionStart = 0;
            //uinput.selectionEnd = uinput.value.Length;
        }

        //输入的长度
        float valuef;
        if (float.TryParse(uinput.text, out valuef) == false)
        {
            uinput.textComponent.color = Color.red;
            return;
        }

        if (tempStart == true)
        {
            tempStart = false;
            uinput.ActivateInputField();
            uinput.selectionAnchorPosition = 0;
            uinput.selectionFocusPosition = uinput.text.Length;
            //uinput.isSelected = true;
            //uinput.selectionStart = 0;
            //uinput.selectionEnd = uinput.value.Length;
        }

        if (valuef /** 1000f / (int)defaultSetting.DefaultUnit*/ < 10 || valuef /** 1000f / (int)defaultSetting.DefaultUnit*/ > 100000)
        {
            uinput.textComponent.color = Color.red;
            return;
        }

        if (lastInputWrightValue.disValue != valuef)
        {
            lastInputWrightValue.disValue = valuef;

            lastInputWrightValue.posValue.x = valuef / dis * (tempWall.point2.pos.x - tempWall.point1.pos.x) + tempWall.point1.pos.x;
            lastInputWrightValue.posValue.y = valuef / dis * (tempWall.point2.pos.y - tempWall.point1.pos.y) + tempWall.point1.pos.y;

            bool cross = false;
            ///检测是否与墙相交  如果刚好到墙附近取墙上的点
            for (int i = 0; i < helperData.helpLineList.Count; i++)
            {
                WallData wall = helperData.helpLineList[i];
                if (linefunc.GetCross(wall, helperData.currentPoint, lastInputWrightValue.posValue) == true)
                {
                    cross = true;
                    break;
                }
            }

            if (cross == false)
            {
                for (int i = 0; i < data.wallList.Count; i++)
                {
                    WallData wall = data.wallList[i];
                    if (linefunc.GetCross(wall, helperData.currentPoint, lastInputWrightValue.posValue) == true)
                    {
                        cross = true;
                        break;
                    }
                }
            }

            lastInputWrightValue.able = !cross;
            if (cross == true)
            {
                uinput.textComponent.color = Color.red;
            }
            else {
                uinput.textComponent.color = Color.black;
            }
        }

        if (lastInputWrightValue.able == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            uinput.DeactivateInputField();

            Point point = new Point();
            point.pos = lastInputWrightValue.posValue;
            data.AddPoint(point);

            WallData line = new WallData();
            line.point1 = helperData.currentPoint;
            line.point2 = point;
            line.height = defaultSetting.DefaultHeight;
            line.width = defaultSetting.DefaultWidth;
            helperData.helpLineList.Add(line);
            helperData.currentPoint = point;
        }
    }

    private void drawDashedLine(WallData data, GameObject obj)
    {
        linefunc.SetLine(data.point1.pos, data.point2.pos, obj, prefabs.size);
    }

    protected override void RefreshGameObjs()
    {
        base.RefreshGameObjs();
        if (mouseLine == null)
        {
            mouseLine = prefabs.GetNewInstance_dashed().gameObject;
        }
        OpenShowLen();
        SetGameObjCountEquleList(helperData.helpLineList.Count, dashedLines, prefabs.GetNewInstance_dashed);
    }

    /// <summary>
    /// 预览墙模式（预设线为墙的左边线 右边线 中线）
    /// </summary>
    /// <param name="type">-1.左边线 0.中线 1.右边线</param>
    public void ShowWallType(int type)
    {
        for (int i = 0; i < helperData.helpLineList.Count; i++)
        {
            
        }
    }


    public void SetWallType(int type)
    {
        //data.wallList变动项（比如：一个墙被分成两段）
        for (int i = 0; i < helperData.helpLineList.Count; i++)
        {
            WallData wall = helperData.helpLineList[i];
            ResetWallByPoint(wall.point1);
            ResetWallByPoint(wall.point2);
        }

        //ReSetRoom();

        //data.wallList新增项
        for (int i = 0; i < helperData.helpLineList.Count; i++)
        {
            WallData wall = helperData.helpLineList[i];
            bool contine = false;

            for (int j = 0; j < data.wallList.Count; j++)
            {
                if (data.wallList[j].equle(wall))
                {
                    contine = true;
                    break;
                }
            }
            if (contine == true)
            {
                continue;
            }
            data.AddWall(wall);
        }
        
    }

    private void ResetWallByPoint(Point point)
    {
        WallData wall = null;
        Vector2 pos = Vector2.zero;
        for (int i = 0; i < data.wallList.Count; i++)
        {
            WallData item = data.wallList[i];
            if (item.point1 == point || item.point2 == point)
            {
                return;
            }
            if (item.GetDis(point.pos) < 0.01f)
            {
                Vector2 itemPos = item.GetDisPoint(point.pos);
                if (wallfunc.PointOnWall(itemPos, item))
                {
                    wall = item;
                    pos = itemPos;
                    break;
                }
            }
        }

        //墙体被分割
        if (wall != null)
        {
            data.CombinePointWall(point, wall);
            //data.RemoveWall(wall);
            ////wall.point1.nearList.Remove(wall.point2);
            ////wall.point2.nearList.Remove(wall.point1);

            //data.AddWall(wall.point1, point, wall.height, wall.width);

            //data.AddWall(point, wall.point2, wall.height, wall.width);

        }

    }

    public void SetMouseLineable()
    {
        //Debug.LogWarning("SetMouseLineable");
        Renderer render = mouseLine.GetComponent<Renderer>();
        render.material = prefabs.dashedMaterial;
    }

    public void SetMouseLineDisable()
    {
        //Debug.LogWarning("SetMouseLineDisable");
        Renderer render = mouseLine.GetComponent<Renderer>();
        render.material = prefabs.dashedMaterial_red;
    }



    public class Valueable
    {
        public float disValue = 0;
        public Vector2 posValue = Vector2.zero;
        public bool able = true;
    }
}
