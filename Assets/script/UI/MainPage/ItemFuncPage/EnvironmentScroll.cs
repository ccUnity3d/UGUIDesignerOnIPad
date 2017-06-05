using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EnvironmentScroll : UGUIScrollView
{



    List<ItemData> itemList = new List<ItemData>();

    private Arrangement _arrangement = Arrangement.Vertical;
    protected override void Init()
    {
        ItemSkin = transform.Find("itemSkin").gameObject;
        ItemSkin.AddComponent<EnvironmentItemFunc>();
        UDragScroll uscr = ItemSkin.AddComponent<UDragScroll>();
        uscr.scroRect = ScroRect;
        SetData(262, 80, _arrangement, 6, 20, 0, 20);

        itemList.Add(new EnvironmentItemData("草色青青"));
        itemList.Add(new EnvironmentItemData("高楼林立"));
        itemList.Add(new EnvironmentItemData("街道夜色"));
        itemList.Add(new EnvironmentItemData("无外景"));
        itemList.Add(new EnvironmentItemData("碧水蓝天"));
        itemList.Add(new EnvironmentItemData("绵绵小溪"));

        RefreshDisplay(itemList);


    }
}
