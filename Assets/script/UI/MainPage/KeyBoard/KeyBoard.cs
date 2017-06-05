using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class KeyBoard : UIPage<KeyBoard>
{
    #region 定义
    public Transform keyboard;
    public Button one;
    public Button two;
    public Button there;
    public Button four;
    public Button five;
    public Button six;
    public Button seven;
    public Button eight;
    public Button nine;
    public Button cancel;
    public Button zero;
    public Button point;
    public Button exit;
    #endregion
    protected override void Ready(Object arg1)
    {
        keyboard = skin.transform;
        one = UITool.GetUIComponent<Button>(keyboard, "Keyboard/one");   
        two = UITool.GetUIComponent<Button>(keyboard, "Keyboard/two");  
        there = UITool.GetUIComponent<Button>(keyboard, "Keyboard/there");  
        four = UITool.GetUIComponent<Button>(keyboard, "Keyboard/four");  
        five = UITool.GetUIComponent<Button>(keyboard, "Keyboard/five"); 
        six = UITool.GetUIComponent<Button>(keyboard, "Keyboard/six");
        seven = UITool.GetUIComponent<Button>(keyboard, "Keyboard/seven");   
        eight = UITool.GetUIComponent<Button>(keyboard, "Keyboard/eight");  
        nine = UITool.GetUIComponent<Button>(keyboard, "Keyboard/nine");   
        cancel = UITool.GetUIComponent<Button>(keyboard, "Keyboard/cancel");  
        zero = UITool.GetUIComponent<Button>(keyboard, "Keyboard/zero");   
        point = UITool.GetUIComponent<Button>(keyboard, "Keyboard/point");
        exit = UITool.GetUIComponent<Button>(keyboard,"Keyboard/exit");
    }
}
