using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GoodDetailScroll : UGUIScrollView {

    public int _cellWidth;
    public int _cellHeight;
    public int _cellWidthSpace;
    public int _cellHeightSpace;
    public Arrangement _arrangement = Arrangement.Vertical;
    public int _upperLimitIndex;
    public int _lowerLimitIndex;
    public int _maxPerLine;

    protected override void Init()
    {

        ItemSkin = transform.Find("detailItem").gameObject;
        ItemSkin.AddComponent<UDragScroll>();
        ItemSkin.AddComponent<GoodDetailItemFunc>();
        SetData(_cellWidth,_cellHeight,_arrangement,0,0,0,0,1);
        for (int i = 0; i < 20; i++)
        {
            msgs.Add(new ItemData());
        }
        Display(msgs);
    }
}
