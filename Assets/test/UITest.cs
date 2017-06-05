using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UITest : MonoBehaviour {




    public InputField myInputField;











    // Use this for initialization
    Action myDelegate;
    Func<int,string> myInt;
    public void Awake()
    {
        myDelegate = MyDelegate;
        myInt = FunInt;
        Debug.Log(myInt(2));
    }
	void Start () {
        myDelegate();
    }
    public void MyDelegate()
    {
        Debug.Log("Action");
    }
    public string FunInt(int x)
    {
        Debug.Log("输出INT");
        return "3333";
    }
	// Update is called once per frame
	void Update () {
	
	}
}
