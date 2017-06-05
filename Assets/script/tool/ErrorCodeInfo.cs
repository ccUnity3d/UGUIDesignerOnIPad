using UnityEngine;
using System.Collections;

[System.Serializable]
public class ErrorCodeInfo
{

    public int code;

    public string introduce;

    /// <summary>
    /// 1仅提示，2不消失提示，代理单独按钮OK功能，3不消失提示，代理取消及确定功能
    /// </summary>
    public int type;

    public int success;

    /// <summary>
    /// 提示title
    /// </summary>
    public string title;

    /// <summary>
    /// 取消键显示字符
    /// </summary>
    public string cancle;

    /// <summary>
    /// 确定按键显示字符
    /// </summary>
    public string sure;

}
