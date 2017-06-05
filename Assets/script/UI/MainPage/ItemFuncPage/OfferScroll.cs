using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class OfferScroll : UGUIScrollView {

    public int _cellWidth;
    public int _cellHeight;
    public int _cellWidthSpace;
    public int _cellHeightSpace;
    public Arrangement _arrangement = Arrangement.Vertical;
    public int _upperLimitIndex;
    public int _lowerLimitIndex;
    public int _maxPerLine;

    private List<GameObject> numberText;
    private List<GameObject> editorNumber;
    private List<Toggle> toggleList; 

    protected override void Init()
    {
        ItemSkin = transform.Find("itemPictures").gameObject;
        ItemSkin.AddComponent<UDragScroll>();
        ItemSkin.AddComponent<CreateOfferItemFunc>();
        SetData(_cellWidth,_cellHeight,_arrangement,_upperLimitIndex,_lowerLimitIndex,_cellWidthSpace,_cellHeightSpace,_maxPerLine);
        // 测试
        for (int i = 0; i < 20; i++)
        {
            msgs.Add(new ItemData());
        }
        Display(msgs);
    }

    protected override void ItemAddListion(UGUIItemFunction func)
    {
        base.ItemAddListion(func);
        CreateOfferItemFunc itemFunc = func as CreateOfferItemFunc;
        //itemFunc.deleteFun = DeleFunc;
    }
    private void DeleFunc(CreateOfferItemFunc obj)
    {
        msgs.RemoveAt(obj.index);
        RefreshDisplay(null,false,true);
    }
}
