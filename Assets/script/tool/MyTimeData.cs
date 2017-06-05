using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MyTimeData
{
    public int Year;
    public int Month;
    public int Day;
    public int Hour;
    public int Minute;
    public int Second;
    public int Millisecond;

    public MyTimeData()
    {
    }
    public MyTimeData(DateTime now)
    {
        this.Year = now.Year;
        this.Month = now.Month;
        this.Day = now.Day;
        this.Hour = now.Hour;
        this.Minute = now.Minute;
        this.Second = now.Second;
        this.Millisecond = now.Millisecond;
    }

    public void DeSerialize(Dictionary<string, object> dic)
    {
        foreach (string key in dic.Keys)
        {
            switch (key)
            {
                case "Year":
                    Year = MyJsonTool.getIntValue(dic, key);
                    break;
                case "Month":
                    Month = MyJsonTool.getIntValue(dic, key);
                    break;
                case "Day":
                    Day = MyJsonTool.getIntValue(dic, key);
                    break;
                case "Hour":
                    Hour = MyJsonTool.getIntValue(dic, key);
                    break;
                case "Minute":
                    Minute = MyJsonTool.getIntValue(dic, key);
                    break;
                case "Second":
                    Second = MyJsonTool.getIntValue(dic, key);
                    break;
                case "Millisecond":
                    Millisecond = MyJsonTool.getIntValue(dic, key);
                    break;
                default:
                    break;
            }
        }
    }

    public bool isEmty
    {
        get {
            if (Year != 0)
            {
                return false;
            }
            if (Month != 0)
            {
                return false;
            }
            if (Day != 0)
            {
                return false;
            }
            if (Hour != 0)
            {
                return false;
            }
            if (Minute != 0)
            {
                return false;
            }
            if (Second != 0)
            {
                return false;
            }
            if (Millisecond != 0)
            {
                return false;
            }
            return true;
        }
    }
}
