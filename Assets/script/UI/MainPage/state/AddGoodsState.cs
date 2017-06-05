using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class AddGoodsState : MainPageState
{
    public const string Name = "AddGoodsState";

    /// <summary>
    /// 选中的模型 去除贴图
    /// </summary>
    private List<SelectProductData> selectList = new List<SelectProductData>();
    private List<SelectProductData> selectMaterialList = new List<SelectProductData>();

    public override void enter()
    {
        base.enter();

        //sendToIOS
        if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            TempDo();
        }
        else {
            MsgToIOS msg = new MsgToIOS();
            msg.code = "101002";
            UnityIOSMsg.sendToIOS(msg, IOSEvent.SelectGoods, OnSelected);
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
            else if(product.productType == "3D")
            {
                selectList.Add(goodsList[i]);
            }
        }
        if (selectList.Count == 0)
        {
            if (selectMaterialList.Count > 0)
            {

                //添加材质 selectMaterialList
            }
            return;
        }
        inputMachine.selectGoods = selectList;

        undoHelper.save();

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
            for (int i = 0; i < originalInputData.pointList.Count; i++)
            {
                Vector2 v2 = originalInputData.pointList[i].pos;
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
                    ProductData productData = originalInputData.AddProduct(vo.id, temppos, 0, product, null, vo.type);
                }

                pos.z -= 1;
            }
            RefreshView();
            if (inputMachine.currentInputIs2D) inputMachine.setState(FreeState2D.NAME);
            else inputMachine.setState(FreeState3D.NAME);
        }
    }

    /// <summary>
    /// 临时模拟
    /// </summary>
    private void TempDo()
    {
        TextAsset asset = Resources.Load<TextAsset>("addGoods");
        MsgFromIOS iosMsg = (MsgFromIOS)asset.text;
        List<Product> products = new List<Product>();
        List<SelectProductData> selectproducts = new List<SelectProductData>();
        if (iosMsg.code == "201002")
        {
            List<object> list = iosMsg.info.goodsArr as List<object>;
            
            for (int i = 0; i < list.Count; i++)
            {
                MsgFromIOS.GoodsInfo info = new MsgFromIOS.GoodsInfo();
                info.Deserialize(list[i] as Dictionary<string, object>);
                //goodsList.Add(info);
                SelectProductData selctproduct = new SelectProductData();
                info.WriteToSelectProductData(selctproduct);
                //info.WriteToProduct(product);
                products.Add(info.model);
                selectproducts.Add(selctproduct);
            }
        }
        {
            for (int i = 0; i < products.Count; i++)
            {
                mainpageData.AddProduct(products[i]);
            }
            selectproducts[2].count = 2;
            List<SelectProductData> list = new List<SelectProductData>() {
                 selectproducts[1],
                selectproducts[2],
                selectproducts[3],
                /*selectproducts[4],*/
                selectproducts[5],
                selectproducts[6],
                selectproducts[7],
                selectproducts[8],
                selectproducts[9],
            };
            inputMachine.selectGoods = list;

            undoHelper.save();

            if (list.Count == 1 && list[0].count == 1)
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
                for (int i = 0; i < originalInputData.pointList.Count; i++)
                {
                    Vector2 v2 = originalInputData.pointList[i].pos;
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
                for (int i = 0; i < list.Count; i++)
                {
                    SelectProductData selectProductData = list[i];
                    GoodsVO vo = mainpageData.getGoods(selectProductData.seekId);
                    Product product = mainpageData.getProduct(selectProductData.seekId);
                    pos.y = product.defaultHeight;

                    Vector3 temppos = pos;
                    for (int k = 0; k < selectProductData.count; k++)
                    {
                        temppos.x += 2;
                        ProductData productData = originalInputData.AddProduct(vo.id, temppos, 0, product, null, vo.type);
                    }

                    pos.z -= 1;
                }
                RefreshView();
                if(inputMachine.currentInputIs2D) inputMachine.setState(FreeState2D.NAME);
                else inputMachine.setState(FreeState3D.NAME);
            }
        }
    }

    public override void exit()
    {
        
        base.exit();
    }
}
