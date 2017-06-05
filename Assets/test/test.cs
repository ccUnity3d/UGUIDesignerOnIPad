using System.Collections.Generic;
using UnityEngine;

public class test : ScriptableObject{

    public testData data = new testData();
    //// Use this for initialization
    //void Start()
    //{
    //    Debug.LogWarning(Mathf.RoundToInt(-0.55f));
    //    Debug.LogWarning(Mathf.RoundToInt(0.55f));
    //}

    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        bool bol = EventSystem.current.IsPointerOverGameObject();
    //        Debug.LogWarning(bol);
    //    }
    //}

}

[System.Serializable]
public class testData
{
    [System.NonSerialized]
    public string t;
    public List<string> t2;

}
