using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SelectGoods2D_ReplaceState : SelectGoods2D_State
{
    private List<SelectProductData> selectList;
    private List<SelectProductData> selectMaterialList;

    public override void enter()
    {
        base.enter();
        //打开IOS界面选择切换的物体返回
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            MsgToIOS msg = new MsgToIOS();
            msg.code = "101002";
            UnityIOSMsg.sendToIOS(msg, IOSEvent.SelectGoods, OnSelected);
        }
        else
        {
            tempDo();
        }
    }

    private void OnSelected(MyEvent obj)
    {
        UnityIOSMsg.removeListioner(IOSEvent.SelectGoods, OnSelected);
        object[] objs = obj.data as object[];
        List<Product> products = objs[0] as List<Product>;
        List<SelectProductData> goodsList = objs[1] as List<SelectProductData>;
        for (int i = 0; i < products.Count; i++)
        {
            mainpageData.AddProduct(products[i]);
        }

        selectList.Clear();
        selectMaterialList.Clear();
        for (int i = 0; i < goodsList.Count; i++)
        {
            Product product = mainpageData.getProduct(goodsList[i].seekId);
            if (product.productType == "FloorTiles" || product.productType == "Wallpapers")
            {
                selectMaterialList.Add(goodsList[i]);
            }
            else if (product.productType == "3D")
            {
                selectList.Add(goodsList[i]);
            }
        }
        if (selectList.Count == 0)
        {
            return;
        }

        undoHelper.save();

        data.RemoveProduct(target);
        inputMachine.selectGoods = selectList;
        if (selectList.Count == 1 && selectList[0].count == 1)
        {
            if (inputMachine.currentInputIs2D)
            {
                inputMachine.setState(PlaceGoodsState2D.NAME);
            }
            else
            {
                inputMachine.setState(PlaceGoodsState3D.NAME);
            }
        }
        else
        {
            Vector3 pos = Vector3.zero;
            for (int i = 0; i < data.pointList.Count; i++)
            {
                Vector2 v2 = data.pointList[i].pos;
                if (pos.z < v2.y)
                {
                    pos.z = v2.y;
                }
                if (pos.x < v2.x)
                {
                    pos.x = v2.x;
                }
            }
            pos.x += 2;
            for (int i = 0; i < selectList.Count; i++)
            {
                SelectProductData selectProductData = selectList[i];
                GoodsVO vo = mainpageData.getGoods(selectProductData.seekId);
                Product product = mainpageData.getProduct(selectProductData.seekId);
                pos.y = product.defaultHeight;

                Vector3 temppos = pos;
                for (int k = 0; k < selectProductData.count; k++)
                {
                    temppos.x += 2;
                    ProductData productData = data.AddProduct(vo.id, temppos, 0, product, null, vo.type);
                }

                pos.z -= 1;
            }
            RefreshView();
            if (inputMachine.currentInputIs2D) inputMachine.setState(FreeState2D.NAME);
            else inputMachine.setState(FreeState3D.NAME);
        }
    }

    private void tempDo()
    {
        //throw new NotImplementedException();
    }

    public override void exit()
    {
        base.exit();
    }
}
