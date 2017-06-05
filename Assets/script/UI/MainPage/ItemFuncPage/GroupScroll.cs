using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GroupScroll : UGUIScrollView {


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
        ItemSkin = transform.Find("item").gameObject;
        ItemSkin.AddComponent<UDragScroll>();
        ItemSkin.AddComponent<GroupItemFunc>();
        SetData(100, 100, _arrangement, 0, 0, 0, 20, 3);
    }
    public override void RefreshDisplay(List<ItemData> data = null, bool restPos = false, bool isChange = false)
    {
        base.RefreshDisplay(data, restPos, isChange);
    }
}
