using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SelectGoods2D_GroupState : SelectGoods2D_State
{
    private List<ProductData> selectGoods = new List<ProductData>();

    public ToggleButton toggleButton;

    private Collider2D colli;
    private OptionsPage optionsPage { get { return OptionsPage.Instance; } }

    private MyTweenRectPosition myTween;
    private GroupPage groupPage
    {
        get
        {
            return GroupPage.Instance;
        }
    }

    private GroupScroll groupScroll;
    private List<ItemData> _uiShowDatas = new List<ItemData>();
    private List<ItemData> uiShowDatas
    {
        get {
            _uiShowDatas.Clear();
            for (int i = 0; i < selectGoods.Count; i++)
            {
                _uiShowDatas.Add(selectGoods[i]);
            }
            _uiShowDatas.Add(target);
            return _uiShowDatas;
        }
    }

    public override bool needCloseSelectGoodsState2DUpdate
    {
        get
        {
            return true;
        }
    }

    protected override bool needUpdate
    {
        get
        {
            return true;
        }
    }

    public override void Ready(GameObject skin)
    {
        base.Ready(skin);
        toggleButton = optionsPage.group;
        groupPage.SetData(MainPage.Instance.SkinGroup);
        myTween = groupPage.groupRectTransform.gameObject.AddComponent<MyTweenRectPosition>();
        //groupPage.groupRectTransform.anchoredPosition3D = Vector3.up * (-136);
        groupPage.inputInfo.onValueChanged.AddListener(onInputInfo);
        groupPage.inputName.onValueChanged.AddListener(onInputName);
        groupPage.ok.onClick.AddListener(onOk);
        groupPage.cancel.onClick.AddListener(onCancel);
        groupScroll = groupPage.collectGroup.gameObject.AddComponent<GroupScroll>();
    }

    public override void enter()
    {
        base.enter();
        if (toggleButton.onDown == false)
        {
            setState(EditTypeOnSelect.Free);
            return;
        }

        selectGoods.Clear();
        //显示UI界面
        ShowPage();
        RefreshUIShow();
    }

    public override void exit()
    {
        base.exit();

        toggleButton.onDown = false;

        selectGoods.Clear();

        RefreshShow();

        ClosePage();
    }

    private void ShowPage()
    {
        myTween.from = Vector2.up;
        myTween.duration = 1f;
        myTween.to = Vector2.right * (-550);

        myTween.SetStartToCurrentValue();
        myTween.to = Vector2.right * (-550);
        myTween.PlayForward();
    }
    private void ClosePage()
    {
        myTween.from = Vector2.up;
        myTween.duration = 1f;
        myTween.to = Vector2.right * (-550);

        myTween.SetStartToCurrentValue();
        myTween.to = Vector2.up;
        myTween.PlayForward();
    }
    private void onInputName(string str)
    {
        // 输入组合名称
    }
    private void onInputInfo(string str)
    {
        // 输出组合备注信息
    }
    private void onOk()
    {
        myTween.SetStartToCurrentValue();
        myTween.to = Vector2.up;
        myTween.PlayForward();
        // 确定添加组合
    }
    private void onCancel()
    {
        myTween.SetStartToCurrentValue();
        myTween.to = Vector2.up;
        myTween.PlayForward();
        // 取消添加组合
        
    }


    private void RefreshShow()
    {
        //刷新UI界面uiShowDatas
        RefreshUIShow();

        //下面使用selectGoods是uiShowDatas剔除target的数据
        view2D.selectPackedProductdatas = selectGoods;
        view3D.selectGroup = selectGoods;
        RefreshView();
    }

    private void RefreshUIShow()
    {
        groupScroll.Display(uiShowDatas);
    }

    private void Select(ProductData productData)
    {
        int index = selectGoods.IndexOf(productData);
        if (index == -1)
        {
            selectGoods.Add(productData);
        }
        else {
            selectGoods.RemoveAt(index);
        }

        ///刷新界面
        RefreshShow();
    }

    public override void mUpdate()
    {
        base.mUpdate();
        if (Input.GetMouseButtonDown(0) && uguiHitUI.uiHited == false)
        {
            Vector2 v2 = GetScreenToWorldPos(Input.mousePosition);
            colli = Physics2D.OverlapPoint(v2);
            if (colli == null)
            {
                setState(FreeState2D.NAME);
                return;
            }
            HandleView handle = colli.GetComponent<HandleView>();
            if (handle != null)
            {
                return;
            }

            ObjView view = colli.GetComponent<ObjView>();
            if (view == null)
            {
                return;
            }
            else if (view.objData == target)
            {
                return;
            }
            else
            {
                if (view.objData is ProductData)
                {
                    Select(view.objData as ProductData);
                }
            }
        }
    }

    public override void mPhoneUpdate()
    {
        base.mPhoneUpdate();
        if (uguiHitUI.uiHited == true || Input.touchCount != 1)
        {
            return;
        }
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            Vector2 v2 = GetScreenToWorldPos(touch.position);
            colli = Physics2D.OverlapPoint(v2);
            if (colli == null)
            {
                setState(FreeState2D.NAME);
                return;
            }
            HandleView handle = colli.GetComponent<HandleView>();
            if (handle != null)
            {
                return;
            }
            ObjView view = colli.GetComponent<ObjView>();
            if (view == null)
            {
                return;
            }
            else if (view.objData == target)
            {
                return;
            }
            else
            {
                if (view.objData is ProductData)
                {
                    Select(view.objData as ProductData);
                }
            }
        }
    }

    protected Vector3 GetScreenToWorldPos(Vector3 pos)
    {
        return inputCamera.ScreenToWorldPoint(pos);
    }

}
