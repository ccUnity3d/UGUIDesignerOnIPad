using System;
using System.Collections.Generic;
using UnityEngine;

public class Mode2DPrefabs : Singleton<Mode2DPrefabs>
{
    public readonly float size = 0.2f;

    private Transform _Scene;
    public Transform Scene
    {
        get
        {
            if (_Scene == null) _Scene = GameObject.Find("Scene").transform;
            return _Scene;
        }
    }


    private Transform _2dparentTran;
    public Transform parentTran_2D
    {
        get
        {
            if (_2dparentTran == null) _2dparentTran = new GameObject("2DGameobject").transform;
            _2dparentTran.parent = Scene;
            return _2dparentTran;
        }
    }

    private Transform _parentTran_SelectMask;
    public Transform parentTran_SelectMask
    {
        get
        {
            if (_parentTran_SelectMask == null) _parentTran_SelectMask = new GameObject("SelectMasks").transform;
            _parentTran_SelectMask.parent = Scene;
            return _parentTran_SelectMask;
        }
    }

    private Transform _3dparentTran;
    public Transform parentTran_3D
    {
        get
        {
            if (_3dparentTran == null) _3dparentTran = new GameObject("3DGameobject").transform;
            _3dparentTran.parent = Scene;
            return _3dparentTran;
        }
    }

    private Transform _sizeParentTran;
    public Transform sizeParentTran
    {
        get
        {
            if (_sizeParentTran == null) _sizeParentTran = new GameObject("sizeGameobject").transform;
            _sizeParentTran.transform.position = -Vector3.forward * 10;
            _sizeParentTran.transform.parent = _2dparentTran;
            return _sizeParentTran;
        }
    }

    private Transform _areaParentTran;
    public Transform areaParentTran
    {
        get
        {
            if (_areaParentTran == null) _areaParentTran = new GameObject("areaGameobject").transform;
            _areaParentTran.transform.position = -Vector3.forward * 20;
            _areaParentTran.transform.parent = _2dparentTran;
            return _areaParentTran;
        }
    }

    private Transform _uiParentTran;
    public Transform uiParentTran
    {
        get
        {
            if (_uiParentTran == null) _uiParentTran = GameObject.Find("UI/Canvas/uiScene").transform;
            return _uiParentTran;
        }
    }

    private Camera _uiCamera;
    public Camera uiCamera
    {
        get
        {
            if (_uiCamera == null) _uiCamera = uiParentTran.parent.GetComponent<Canvas>().worldCamera;
            return _uiCamera;
        }
    }

    private Camera _mainCamera;
    public Camera mainCamera
    {
        get
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
            return _mainCamera;
        }
    }

    //private Camera _helpCamera;
    //public Camera helpCamera
    //{
    //    get {
    //        if (_helpCamera == null)
    //        {
    //            _helpCamera = GameObject.Find("HelpCamera").GetComponent<Camera>();
    //            helpCamera.gameObject.SetActive(false);
    //        }
    //        return _helpCamera;
    //    }
    //}

    private Transform _topParentTran;
    private Transform topParentTran
    {
        get
        {
            if (_topParentTran == null) _topParentTran = new GameObject("TopGameobject").transform;
            _topParentTran.transform.parent = _2dparentTran;
            return _topParentTran;
        }
    }

    public Mode2DPrefabs()
    {
        LoaderPool.InnerLoad("2D/prefab/iconbase2d.assetbundle", SimpleLoadDataType.prefabAssetBundle, onloaded, null);
    }

    private void onloaded(object obj)
    {
        SimpleInnerLoader loader = obj as SimpleInnerLoader;
        if (loader.state == SimpleLoadedState.Failed)
        {
            return;
        }
        GameObject go = loader.loadedData as GameObject;
        go.SetActive(false);
        point = go.transform.FindChild("point");
        grey = go.transform.FindChild("grey");
        black = go.transform.FindChild("black");
        circle = go.transform.FindChild("yuan");
        line = go.transform.FindChild("greyLine");
        dashed = go.transform.FindChild("dashedLine");
        dashedMaterial = dashed.GetComponent<MeshRenderer>().sharedMaterial;
        dashed_red = go.transform.FindChild("dashedLine_red"); 
        dashedMaterial_red = dashed_red.GetComponent<MeshRenderer>().sharedMaterial;
        text = go.transform.FindChild("text");
        area = go.transform.FindChild("area");
        fork = go.transform.FindChild("cha");
        floor = go.transform.FindChild("floor2d");
        white = go.transform.FindChild("white");
        for (int i = 0; i < 10; i++)
        {
            Transform tran = go.transform.FindChild("tBoard" + i);
            if(i == 0)tBoard = tran;
            if (tran == null)break;
            tBoardTextures.Add(tran.GetComponent<Renderer>().sharedMaterial.mainTexture as Texture2D);
        }
        for (int i = 0; i < 10; i++)
        {
            Transform tran = go.transform.FindChild("topBoard" + i);
            if (i == 0) topBoard = tran;
            if (tran == null)break;
            topBoardTextures.Add(tran.GetComponent<Renderer>().material.mainTexture as Texture2D);
        }
        floor3d = go.transform.FindChild("floor3d");
        wall3d = go.transform.FindChild("wall3d");
        walltop = go.transform.FindChild("walltop");
        quad = go.transform.FindChild("quad").gameObject;
        quad.AddComponent<GoodsView>();
        input = go.transform.FindChild("input");
        rotate = go.transform.FindChild("rotate");

        point_select = go.transform.FindChild("point_select");
        mask_select = go.transform.FindChild("mask_select");

        inputWallLength = go.transform.FindChild("inputWallLength");

        circleRotate = go.transform.FindChild("circleRotate");
        disFloor = go.transform.FindChild("disFloor");
        rotateicon = go.transform.FindChild("rotateicon");
    }

    public Material dashedMaterial;
    public Material dashedMaterial_red;


    /// <summary>
    /// 墙拐点
    /// </summary>
    private Transform point;
    /// <summary>
    /// 墙体
    /// </summary>
    private Transform grey;
    /// <summary>
    /// 墙边缘线
    /// </summary>
    private Transform black;
    /// <summary>
    /// 3D白色墙体
    /// </summary>
    private Transform white;

    public List<Texture2D> tBoardTextures = new List<Texture2D>();
    private List<Texture2D> topBoardTextures = new List<Texture2D>();
    /// <summary>
    /// 3D踢脚线
    /// </summary>
    private Transform tBoard;
    /// <summary>
    /// 3D顶角线
    /// </summary>
    private Transform topBoard;
    /// <summary>
    /// 鼠标位置图标
    /// </summary>
    private Transform cursor;
    /// <summary>
    /// 圆圈
    /// </summary>
    private Transform circle;
    /// <summary>
    ///  显示长度的叉点
    /// </summary>
    private Transform fork;
    /// <summary>
    /// 灰色实线
    /// </summary>
    private Transform line;
    /// <summary>
    /// 虚线
    /// </summary>
    private Transform dashed;
    private Transform dashed_red;
    /// <summary>
    /// 字
    /// </summary>
    private Transform text;
    /// <summary>
    /// 面积
    /// </summary>
    private Transform area;

    private Transform input;

    private Transform floor;

    private Transform floor3d;
    private Transform wall3d;
    private Transform walltop;

    /// <summary>
    /// 2D顶视图预加载图
    /// </summary>
    private GameObject quad;

    private Transform rotate;

    private Transform point_select;
    private Transform mask_select;
    private Transform inputWallLength;
    private Transform circleRotate;
    private Transform disFloor;
    private Transform rotateicon;

    public Transform GetNewInstance_point()
    {
        GameObject go = GameObject.Instantiate(point.gameObject);
        //Debug.LogWarning("122");
        go.SetActive(true);
        go.transform.parent = parentTran_2D;
        go.transform.localPosition = -Vector3.forward * 2;
        go.AddComponent<PointView>();
        return go.transform;
    }
    public Transform GetNewInstance_grey()
    {
        GameObject go = GameObject.Instantiate(grey.gameObject);
        go.SetActive(true);
        go.transform.parent = parentTran_2D;
        go.transform.localPosition = -Vector3.forward;
        go.AddComponent<WallView>();
        go.AddComponent<PolygonCollider2D>();
        return go.transform;
    }
    public Transform GetNewInstance_black()
    {
        GameObject go = GameObject.Instantiate(black.gameObject);
        go.SetActive(true);
        go.transform.parent = parentTran_2D;
        go.transform.localPosition = -Vector3.forward * 2;
        return go.transform;
    }
    public Transform GetNewInstance_WallSide()
    {
        GameObject go = GameObject.Instantiate(wall3d.gameObject);
        //go.SetActive(true);
        go.transform.parent = parentTran_3D;
        go.transform.localScale = Vector3.one;
        //go.AddComponent<BoxCollider>();
        go.AddComponent<WallSideView>();
        Mesh mesh = go.GetComponent<MeshFilter>().mesh;
        MeshCollider colli = go.GetComponentInChildren<MeshCollider>(true);
        if (colli != null)
        {
            colli.convex = false;
            colli.sharedMesh = mesh;
        }
        return go.transform;
    }

    public Transform GetNewInstance_DoorSide()
    {
        GameObject go = GameObject.Instantiate(wall3d.gameObject);
        //go.SetActive(true);
        go.transform.parent = parentTran_3D;
        go.transform.localScale = Vector3.one;
        go.AddComponent<BoxCollider>();
        //go.AddComponent<wi>();
        return go.transform;
    }
    public Transform GetNewInstance_Floor()
    {
        GameObject go = GameObject.Instantiate(floor3d.gameObject);
        //go.SetActive(true);
        go.transform.parent = parentTran_3D;
        go.transform.localScale = Vector3.one;
        go.AddComponent<FloorView>();
        return go.transform;
    }
    public Transform GetNewInstance_Ceiling()
    {
        GameObject go = GameObject.Instantiate(floor3d.gameObject);
        //go.SetActive(true);
        go.transform.parent = parentTran_3D;
        go.transform.localScale = Vector3.one;
        go.AddComponent<CeilingView>();
        return go.transform;
    }
    public Transform GetNewInstance_whiteOnly()
    {
        GameObject go = GameObject.Instantiate(white.gameObject);
        //go.SetActive(true);
        go.transform.parent = parentTran_3D;
        go.transform.localScale = Vector3.one;
        return go.transform;
    }
    public Transform GetNewInstance_tBoard()
    {
        GameObject go = GameObject.Instantiate(tBoard.gameObject);
        //go.SetActive(true);
        go.transform.parent = parentTran_3D;
        go.transform.localScale = Vector3.one;
        return go.transform;
    }
    public Texture2D GetNewTBoardTexture(bool isT, int index)
    {
        if (isT)
        {
            if (tBoardTextures.Count <= index)
            {
                return tBoardTextures[0];
            }
            return tBoardTextures[index];
        }

        if (topBoardTextures.Count <= index)
        {
            return topBoardTextures[0];
        }
        return topBoardTextures[index];
    }

    public Transform GetNewInstance_walltop()
    {
        GameObject go = GameObject.Instantiate(walltop.gameObject);
        //go.SetActive(true);
        go.transform.parent = parentTran_3D;
        go.transform.localScale = Vector3.one;
        return go.transform;
    }
    public Transform GetNewInstance_cursor()
    {
        GameObject go = GameObject.Instantiate(cursor.gameObject);
        go.SetActive(true);
        go.transform.parent = parentTran_2D;
        return go.transform;
    }
    public Transform GetNewInstance_circle()
    {
        GameObject go = GameObject.Instantiate(circle.gameObject);
        go.SetActive(true);
        go.transform.parent = parentTran_2D;
        return go.transform;
    }
    public Transform GetNewInstance_dashed()
    {
        GameObject go = GameObject.Instantiate(dashed.gameObject);
        go.SetActive(true);
        go.transform.parent = parentTran_2D;
        return go.transform;
    }
    public Transform GetNewInstance_fork()
    {
        GameObject go = GameObject.Instantiate(fork.gameObject);
        go.SetActive(true);
        go.transform.parent = sizeParentTran;
        go.transform.localPosition = Vector3.zero;
        return go.transform;
    }
    public Transform GetNewInstance_line()
    {
        GameObject go = GameObject.Instantiate(line.gameObject);
        go.SetActive(true);
        go.transform.parent = sizeParentTran;
        go.transform.localPosition = -Vector3.forward;
        return go.transform;
    }
    public Transform GetNewInstance_area()
    {
        GameObject go = GameObject.Instantiate(area.gameObject);
        go.SetActive(true);
        go.transform.parent = areaParentTran;
        go.transform.localPosition = -Vector3.forward * 4;
        return go.transform;
    }
    public Transform GetNewInstance_text()
    {
        GameObject go = GameObject.Instantiate(text.gameObject);
        go.SetActive(true);
        go.transform.parent = sizeParentTran;
        go.transform.localPosition = -Vector3.forward * 3;
        return go.transform;
    }

    public Transform GetNewInstance_input()
    {
        GameObject go = GameObject.Instantiate(input.gameObject);
        go.SetActive(true);
        go.transform.SetParent(uiParentTran);
        go.transform.localScale = Vector3.one;
        return go.transform;
    }

    public Transform GetNewInstance_inputWallLength()
    {
        GameObject go = GameObject.Instantiate(inputWallLength.gameObject);
        go.SetActive(true);
        go.transform.SetParent(uiParentTran);
        go.transform.localScale = Vector3.one;
        return go.transform;
    }

    public Transform GetNewInstance_floor()
    {
        GameObject go = GameObject.Instantiate(floor.gameObject);
        go.SetActive(true);
        go.transform.parent = topParentTran;
        go.transform.localPosition = Vector3.forward;
        go.AddComponent<RoomView>();
        go.AddComponent<PolygonCollider2D>();
        return go.transform;
    }

    public GameObject GetNewQuad()
    {
        GameObject go = GameObject.Instantiate(quad);
        go.SetActive(true);
        go.transform.parent = parentTran_2D;
        return go; 
    }

    public GameObject GetNewInstance_rotate()
    {
        GameObject go = GameObject.Instantiate(rotate.gameObject);
        go.SetActive(true);
        go.transform.parent = parentTran_2D;
        go.transform.localPosition = -Vector3.forward * 1;
        go.AddComponent<RotateView2D>();
        return go;
    }

    public Transform GetNewInstance_selectPoint()
    {
        GameObject go = GameObject.Instantiate(point_select.gameObject);
        go.SetActive(true);
        go.transform.parent = parentTran_SelectMask;
        go.transform.localPosition = -Vector3.forward * 4;
        return go.transform;
    }

    public Transform GetNewInstance_selectWall()
    {
        GameObject go = GameObject.Instantiate(mask_select.gameObject);
        go.SetActive(true);
        go.transform.parent = parentTran_SelectMask;
        go.transform.localPosition = -Vector3.forward * 4;
        return go.transform;
    }

    public Transform GetNewInstance_selectRoom()
    {
        GameObject go = GameObject.Instantiate(mask_select.gameObject);
        go.SetActive(true);
        go.transform.parent = parentTran_SelectMask;
        go.transform.localPosition = -Vector3.forward * 4;
        go.transform.localScale = Vector3.one;
        return go.transform;
    }

    public Transform GetNewInstance_selectGoods()
    {
        GameObject go = GameObject.Instantiate(mask_select.gameObject);
        go.SetActive(true);
        go.transform.parent = parentTran_SelectMask;
        go.transform.localPosition = -Vector3.forward * 4;
        go.transform.localScale = Vector3.one;
        return go.transform;
    }

    public Transform GetNewInstance_selectQuad()
    {
        GameObject go = GameObject.Instantiate(mask_select.gameObject);
        go.SetActive(true);
        go.transform.parent = parentTran_SelectMask;
        go.transform.localPosition = -Vector3.forward * 4;
        go.transform.localScale = Vector3.one;
        go.layer = LayerMask.NameToLayer("Default");
        return go.transform;
    }


    public GameObject GetNewInstance_rotate3D()
    {
        GameObject go = GameObject.Instantiate(circleRotate.gameObject);
        go.transform.parent = parentTran_3D;
        go.AddComponent<RotateView3D>();
        go.layer = LayerMask.NameToLayer("Default");
        return go;
    }

    public GameObject GetNewInstance_disFloor()
    {
        GameObject go = GameObject.Instantiate(disFloor.gameObject);
        go.transform.parent = uiParentTran;
        go.AddComponent<MoveUpView3D>();
        go.transform.localScale = Vector3.one;
        go.layer = LayerMask.NameToLayer("UI");
        return go;
    }

    public GameObject GetNewInstance_rotateicon()
    {
        GameObject go = GameObject.Instantiate(rotateicon.gameObject);
        go.transform.parent = uiParentTran;
        go.transform.localScale = Vector2.one * 128;
        go.layer = LayerMask.NameToLayer("UI");
        return go;
    }

    /// <summary>
    /// 门白色底图
    /// </summary>
    /// <returns></returns>
    public Transform GetNewInstance_doorwhiteground()
    {
        GameObject go = GameObject.Instantiate(white.gameObject);
        //go.SetActive(true);
        go.AddComponent<GoodsView>();
        go.AddComponent<BoxCollider2D>();
        go.transform.parent = parentTran_2D;
        go.transform.localScale = Vector3.one;
        return go.transform;
    }
    /// <summary>
    /// 回旋门
    /// </summary>
    /// <returns></returns>
    public GameObject GetNewInstance_swingdoor()
    {
        GameObject go = GameObject.Instantiate(grey.gameObject);
        go.transform.parent = parentTran_2D;
        go.transform.localScale = Vector3.one;
        return go;
    }
    /// <summary>
    /// 推拉门
    /// </summary>
    /// <returns></returns>
    public GameObject GetNewInstance_slidingdoor()
    {
        GameObject go = GameObject.Instantiate(grey.gameObject);
        go.transform.parent = parentTran_2D;
        go.transform.localScale = Vector3.one;
        return go;
    }
    /// <summary>
    /// 折叠门
    /// </summary>
    /// <returns></returns>
    public GameObject GetNewInstance_foldingdoor()
    {
        GameObject go = GameObject.Instantiate(grey.gameObject);
        go.transform.parent = parentTran_2D;
        go.transform.localScale = Vector3.one;
        return go;
    }
}
