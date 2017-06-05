using UnityEngine;
using System.Collections;
using foundation;
using System;

public class PlaceGoodsState2D : InputState2D
{
    public const string NAME = "PlaceGoodsState2D";
    public PlaceGoodsState2D() : base(NAME)
    {

    }

    private GoodsVO targetVO;
    private Product targetProduct;

    private WallHelpFunc wallFunc = WallHelpFunc.Instance;
    private Vector2 targetSize;
    private GameObject targetObj;
    private WallData targetWall;
    private Vector2 lastMousePos = Vector2.zero;
    private SimpleOutterLoader loader;

    public override void enter()
    {
        base.enter();
        //machine.selectGoods
        targetVO = mainpagedata.getGoods(machine.selectGoods[0].seekId);
        targetProduct = mainpagedata.getProduct(targetVO.seekId);
        Vector3 size = targetProduct.size;
        targetSize = Vector2.right * size.x + Vector2.up * size.z;
        targetObj = prefabs.GetNewQuad();
        targetObj.transform.localScale = targetSize;
        targetObj.layer = LayerMask.NameToLayer("UI");
        View2D.Instance.AddProductGameObj(targetVO.id, targetObj);

        if (targetProduct.contentType.StartsWith("door/") == true)
        {
            string type = targetProduct.contentType.Replace("door/", "");
            switch (type)
            {
                case "":
                    break;
                default:
                    break;
            }
            return;
        }
        if (targetProduct.contentType.StartsWith("window/") == true)
        {
            string type = targetProduct.contentType.Replace("window/", "");
            switch (type)
            {
                case "":
                    break;
                default:
                    break;
            }
            return;
        }
        LoadTexture();
    }

    private void LoadTexture()
    {
        SetTextureTool.SetTexture(targetObj, targetVO.uri2D, targetVO.seekId);
        //string outUrl = targetVO.uri2D;// "http://midea-products.oss-cn-shanghai.aliyuncs.com/8b7df9b0-28f5-4c88-a412-324284ccc923/model.assetbundle";//10.0.21.27/yy/model.assetbundle";
        //loader = LoaderPool.OutterLoad(outUrl, SimpleLoadDataType.prefabAssetBundle, onloaded, new object[1] { targetVO });
    }
    
    public override void mUpdate()
    {
        base.mUpdate();
        if (uguiHitUI.uiHited == true)
        {
            return;
        }
        Vector2 inputPos = GetScreenToWorldPos(Input.mousePosition);
        if (lastMousePos == inputPos && Input.GetMouseButtonDown(0) == false)
        {
            return;
        }
        lastMousePos = inputPos;

        targetWall = null;
        float Angle = targetObj.transform.rotation.eulerAngles.z;
        Vector3 world = inputCamera.ScreenToWorldPoint(Input.mousePosition);
        for (int i = 0; i < data.wallList.Count; i++)
        {
            if (targetVO.type == 1)
            {
                GoodsOnWall(data.wallList[i], ref world, ref Angle);
            }
            else if(targetVO.type == 2)
            {
                GoodsInWall(data.wallList[i], ref world, ref Angle);
            }
        }
        world.z = -5 - targetProduct.defaultHeight;
        targetObj.transform.position = world;
        if (Input.GetMouseButtonDown(0))
        {
            undoHelper.save();
            Vector3 angle = targetObj.transform.rotation.eulerAngles;
            Vector3 pos = world.x * Vector3.right + world.y * Vector3.forward;
            Product product = mainpagedata.getProduct(targetVO.seekId);
            ProductData productData = data.AddProduct(targetVO.id, pos, angle.z, product, targetWall, targetVO.type);
            RefreshView();
            ToSelectGoodsState(productData);
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

        Vector2 inputPos = GetScreenToWorldPos(touch.position);
        if (lastMousePos == inputPos && touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
        {
            return;
        }
        lastMousePos = inputPos;

        targetWall = null;
        float Angle = targetObj.transform.rotation.eulerAngles.z;
        Vector3 world2d = inputCamera.ScreenToWorldPoint(touch.position);
        for (int i = 0; i < data.wallList.Count; i++)
        {
            if (targetVO.type != 2)
            {
                GoodsOnWall(data.wallList[i], ref world2d, ref Angle);
            }
            else
            {
                GoodsInWall(data.wallList[i], ref world2d, ref Angle);
            }
        }
        world2d.z = -5 - targetProduct.defaultHeight;
        targetObj.transform.position = world2d;
        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            undoHelper.save();
            Vector3 angle = targetObj.transform.rotation.eulerAngles;
            Vector3 world3d = world2d.x * Vector3.right + world2d.y * Vector3.forward;
            Product product = mainpagedata.getProduct(targetVO.seekId);
            ProductData productData = data.AddProduct(targetVO.id, world3d, angle.z, product, targetWall, targetVO.type);
            RefreshView();
            ToSelectGoodsState(productData);
        }
    }

    private void ToSelectGoodsState(ProductData productData)
    {
        GoodsView view_2 = view2D.getGoodsView(productData);
        Goods3DView view_3 = view3D.getGoodsView(productData);
        SelectState state = getSelectState(SelectGoodsState2D.NAME);
        state.viewTarget = view_2;
        SelectState3D state3D = getSelectState3D(SelectGoodsState3D.NAME);
        state3D.viewTarget = view_3;
        setState(SelectGoodsState2D.NAME);
    }

    private void GoodsOnWall(WallData wall, ref Vector3 world, ref float Angle)
    {
        float angle = wall.GetWallAngle();
        float deltaAngle = angle - Angle;
        float xifudis = targetSize.y / 2;
        int times45 = (int)(deltaAngle / 45);
        if (times45 % 2 != 0)
        {
            times45 += 1;
        }
        deltaAngle = times45 * 45;
        int times90 = times45 / 2;
        if (times90 % 2 != 0)
        {
            xifudis = targetSize.x / 2;
        }
        float dis = xifudis + wall.width / 2;
        if (wall.GetDis(world) < dis)
        {
            Vector2 pos = wall.GetDisPoint(world);
            if (wallFunc.PointOnWall(pos, wall) || (wall.point1.pos - pos).magnitude < xifudis
                || (wall.point2.pos - pos).magnitude < xifudis)
            {
                world = wall.GetDisPoint(world, dis);
                if (targetWall == null)
                {
                    Vector3 eulerAngles = targetObj.transform.rotation.eulerAngles;
                    Angle = angle - deltaAngle;
                    eulerAngles.z = Angle;
                    targetObj.transform.rotation = Quaternion.Euler(eulerAngles);
                    targetWall = wall;
                }
            }
        }
    }

    private void GoodsInWall(WallData wall, ref Vector3 world, ref float Angle)
    {
        float angle = wall.GetWallAngle();
        float deltaAngle = angle - Angle;
        float xifudis = targetSize.y / 2;
        int times90 = (int)(deltaAngle / 90);
        if (times90 % 2 != 0)
        {
            times90 += 1;
        }
        deltaAngle = times90 * 90;
        //int times180 = times90 / 2;
        float dis = xifudis + wall.width / 2;
        if (wall.GetDis(world) < dis)
        {
            Vector2 pos = wall.GetDisPoint(world);
            if (wallFunc.PointOnWall(pos, wall) || (wall.point1.pos - pos).magnitude < xifudis
                || (wall.point2.pos - pos).magnitude < xifudis)
            {
                world = pos;//wall.GetDisPoint(world, dis);
                if (targetWall == null)
                {
                    //int intvalue = deltaAngle >= 0 ? 1 : -1;
                    //times45 += bool2 == true ? 0 : intvalue;
                    Vector3 eulerAngles = targetObj.transform.rotation.eulerAngles;
                    Angle = angle - deltaAngle;//times45 * 45f;
                    eulerAngles.z = Angle;
                    //Debug.LogWarning(eulerAngles);
                    targetObj.transform.rotation = Quaternion.Euler(eulerAngles);
                    targetSize.y = wall.width;
                    targetObj.transform.localScale = targetSize;
                    targetWall = wall;
                }
            }
        }
    }

    public override void exit()
    {
        //GameObject.Destroy(targetObj);
        base.exit();
    }

}
