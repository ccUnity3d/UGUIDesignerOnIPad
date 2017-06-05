using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class MaterialState : MainPageState
{
    public const string Name = "MaterialState";
    private MaterialImageScroll materialScroll;
    private List<SelectProductData> selectList = new List<SelectProductData>();
    private List<SelectProductData> selectMaterialList = new List<SelectProductData>();

    private MaterialPage material { get { return MaterialPage.Instance; } }


    public override void Ready()
    {
        base.Ready();
        material.SetData(mainpage.SkinMaterialScorll);
        materialScroll = material.materialScroll;
        materialScroll.deleDropMaterial = DropMarerial;
        Button materialAdd = UITool.GetUIComponent<Button>(materialScroll.transform, "Controller/Add");
        materialAdd.onClick.AddListener(AddMaterial);
        materialScroll.addEventListener(MaterialEvent.RemoveMaterial, RemoveMaterial);
    }
    public override void enter()
    {
        base.enter();
        if (material.skin.activeSelf == true)
        {
            setState(MainPageFreeState.Name);
            return;
        }
        material.RectController.sizeDelta = new Vector2(160, 780);
        material.skin.SetActive(true);

        List<ItemData> list = new List<ItemData>();
        //list.Add(new MaterialItemData());
        for (int i = 0; i < mainpageData.uimaterialList.Count; i++)
        {
            list.Add(mainpageData.uimaterialList[i]);
        }        
        materialScroll.Display(list);
    }

    private void AddMaterial()
    {
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
        if (selectMaterialList.Count == 0)
        {
            return;
        }
        List<MaterialItemData> list = new List<MaterialItemData>();
        for (int i = 0; i < selectMaterialList.Count; i++)
        {
            SelectProductData item = selectMaterialList[i];
            bool contine = false;
            for (int k = 0; k < list.Count; k++)
            {
                if (list[k].seekId == item.seekId)
                {
                    contine = true;
                    break;
                }
            }
            for (int k = 0; k < mainpageData.uimaterialList.Count; k++)
            {
                if (mainpageData.uimaterialList[k].seekId == item.seekId)
                {
                    contine = true;
                    break;
                }
            }
            if (contine == true)
            {
                continue;
            }
            Product product = mainpageData.getProduct(item.seekId);
            MaterialItemData data = new MaterialItemData(product.id, product.seekId, product.modelTexture);
            Debug.LogWarning("Add " + data.stringId);
            list.Add(data);

        }
        if (list.Count != 0)
        {
            mainpageData.uimaterialList.AddRange(list);
            materialScroll.AddMaterial(list);
        }
    }

    private void tempDo()
    {
        List<MaterialItemData> list = new List<MaterialItemData>();
        list.Add(new MaterialItemData());
        //list.Add(new MaterialItemData("1", "local", "3D/texture2D/wallfloor.assetbundle"));
        //list.Add(new MaterialItemData("2", "local", "3D/texture2D/wallfloor2.assetbundle"));
        //list.Add(new MaterialItemData("3", "local", "3D/texture2D/wallfloor3.assetbundle"));
        //list.Add(new MaterialItemData("4", "local", "3D/texture2D/wallfloor4.assetbundle"));
        //list.Add(new MaterialItemData("5", "local", "3D/texture2D/wallfloor5.assetbundle"));
        for (int i = 0; i < list.Count; i++)
        {
            MaterialItemData item = list[i];
            Product product = new Product(item.seekId, item.stringId, item.textureURL);
            mainpageData.AddProduct(product);
        }
        mainpageData.uimaterialList.AddRange(list);
        materialScroll.AddMaterial(list);
    }

    private void RemoveMaterial(MyEvent obj)
    {
        MaterialEvent e = obj as MaterialEvent;
        MaterialItemData data = e.data as MaterialItemData;
        Debug.LogWarning("Remove " + data.stringId);
        //int count = originalInputData.uimaterialList.Count;
        mainpageData.uimaterialList.Remove(data);
        //if(count == originalInputData.uimaterialList.Count){
        //    Debug.LogWarning("33#3");
        //}
    }


    private void DropMarerial(MaterialItemData obj)
    {
        SetMaterialState state = inputMachine.getState(SetMaterialState.NAME) as SetMaterialState;
        state.material = obj;
        inputMachine.setState(SetMaterialState.NAME);
    }

    public override void exit()
    {
        inputMachine.setState(FreeState3D.NAME);
        if(machine.nextIsCurrent == false) material.skin.SetActive(false);
        base.exit();
    }
}