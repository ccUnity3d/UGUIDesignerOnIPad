using UnityEngine;
using System;

public class ObjView : MonoBehaviour {

    public ObjData objData;

    public virtual void SetData(ObjData data)
    {
        objData = data;
    }

    public virtual string GetState()
    {
        return FreeState2D.NAME;
    }
}
