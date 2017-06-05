using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CameraStateMachine:Singleton<CameraStateMachine>
{
    public CameraState CurrentState
    {
        get
        {
            return currentState;
        }
    }

    private Dictionary<string, CameraState> stateDic = new Dictionary<string, CameraState>();
    private CameraState currentState = null;
    public CameraStateMachine()
    {
    }

    //private static CameraStateMachine instance;
    //public static CameraStateMachine Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            instance = new CameraStateMachine();
    //            instance.Inject();
    //        }
    //        return instance;
    //    }
    //}
    public override void OnInstance()
    {
        Inject();
    }

    public void Inject()
    {
        addState(CameraState2D.NAME, new CameraState2D());
        addState(CameraState3D.NAME, new CameraState3D());
    }

    
    public void addState(string stateName, CameraState mode)
    {
        if (Instance.stateDic.ContainsKey(stateName) == true)
        {
            return;
        }

        Instance.stateDic.Add(stateName, mode);
    }

    public static void SetState(string stateName)
    {
        Instance.setState(stateName);
    }

    public void setState(string stateName)
    {
        if (stateDic.ContainsKey(stateName) == false)
        {
            return;
        }
        CameraState aimState = stateDic[stateName];
        if (currentState != null)//currentMode != aimMode//新旧状态可以相同
        {
            Debug.Log(currentState.GetType().Name + " Exit()");
            currentState.Exit();
        }
        currentState = aimState;
        Debug.Log(aimState.GetType().Name + " Enter()");
        aimState.Enter();
    }

    public CameraState getState(string stateName)
    {
        if (stateDic.ContainsKey(stateName) == false)
        {
            return null;
        }
        return stateDic[stateName];
    }
}
