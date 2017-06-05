using UnityEngine;
using System.Collections;
using System;

public class ShowState : MainPageState {

    public const string Name = "ShowState";
    private GameObject go;
    private ToggleButton toggleButton;
    private RectTransform showRect;
    private ShowOrHide show { get { return ShowOrHide.Instance; } }
    private OriginalInputData data {get { return OriginalInputData.Instance; } }
    private Mode2DPrefabs prefabs {get { return Mode2DPrefabs.Instance; } }

    public override void enter()
    {
        base.enter();

        if (inputMachine.currentInputIs2D)
        {
            show.RectShow.anchoredPosition = Vector3.zero;
        }
        else
        {
            show.RectShow.anchoredPosition = Vector3.left * 100;
        }

        if (toggleButton.onDown == false)
        {
            setState(MainPageFreeState.Name);
            return;
        }
        show.skin.SetActive(true);
    }

    public override void Ready()
    {
        base.Ready();
        toggleButton = mainpage.show;
        show.SetData(mainpage.SkinShow);
        show.Showsize.onValueChanged.AddListener(OnSize);
        show.Showtop.onValueChanged.AddListener(OnTop);
        show.halfHighWall.onValueChanged.AddListener(OnHalfHighWall);
        show.showObject.onValueChanged.AddListener(OnShowObject);
    }
    
    public override void exit()
    {
        toggleButton.onDown = false;
        if (inputMachine.currentInputIs2D) inputMachine.setState(FreeState2D.NAME); else inputMachine.setState(FreeState3D.NAME);
        if (machine.nextIsCurrent == false) show.skin.SetActive(false);
        base.exit();
    }

    private void OnSize(bool value)
    {
        prefabs.sizeParentTran.gameObject.SetActive(value);
        prefabs.areaParentTran.gameObject.SetActive(value);
    }
    private void OnTop(bool value)
    {
        view3D.topHide = !value;
        undoHelper.save();
        for (int i = 0; i < data.productDataList.Count; i++)
        {
            ProductData productData = data.productDataList[i];
            if (productData.type == 3 || productData.type == 5)
            {
                data.productDataList[i].hide = view3D.topHide;
            }
        }
        if(view3D.topHide == true) CancleSelectTop();
        RefreshView();
    }

    private void OnHalfHighWall(bool value)
    {
        view3D.halfWall = value;
        //if(view3D.halfWall == true) CancleSelectTop();
        view3D.RefreshView();
        //Debug.Log("半高墙");
    }

    private void CancleSelectTop()
    {
        ObjData selectObjData = view2D.selectObjData;
        if (selectObjData is ProductData)
        {
            ProductData productData = selectObjData as ProductData;
            if (productData.type == 3 || productData.type == 5)
            {
                if (inputMachine.currentInputIs2D) inputMachine.setState(FreeState2D.NAME);
                else inputMachine.setState(FreeState3D.NAME);
            }
        }
    }

    public void OnShowObject(bool value)
    {
        undoHelper.save();
        for (int i = 0; i < data.productDataList.Count; i++)
        {
            data.productDataList[i].hide = false;
        }
        for (int i = 0; i < data.wallList.Count; i++)
        {
            data.wallList[i].hide = false;
        }
        RefreshView();
    }
    


}
