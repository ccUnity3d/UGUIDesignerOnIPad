using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CamereViewItemFunc : UGUIItemFunction, IBeginDragHandler, IEndDragHandler
{

    public Action<CamereViewItemFunc> deleFun;
    public Action<int> deleClick;
    public Action<PointerEventData> deleSetDraggedPosition;
    public Button delete;

    //private Button imageButton;
    private RawImage image;
    //private Vector2 inputStartPos;
    private Vector2 offset = Vector2.zero;

    private PreScanData itemData
    {
        get
        {
            return data as PreScanData;
        }
    }

    protected override void Awake()
    {
        image = this.gameObject.GetComponent<RawImage>();
        DrapButton button = this.gameObject.AddComponent<DrapButton>();
        button.onPointerDownDele = onPointerDown;
        button.onPointerUpDele = onPointerUp;
        button.onDragDele = onPointDrag;
        delete = UITool.GetUIComponent<Button>(transform, "delete");
        //imageButton = UITool.AddUIComponent<Button>(image.gameObject);
        button.onPointerClickDele = onClick;
    }


    public void Start()
    {
        delete.onClick.AddListener(onDelete);
    }

    private void onDelete()
    {
        if (deleFun != null)
        {
            deleFun(this);
        }

        Debug.Log("移除相机视角");
    }

    private void onClick()
    {
        Debug.Log("进入 相机设置 视图界面" + index);
        if (deleClick!= null)
        {
            deleClick(index);
        }
    }
    private void onPointDrag(PointerEventData obj)
    {
        offset = obj.delta;
        if (offset == Vector2.zero)
        {
            return;
        }
        if (deleSetDraggedPosition != null) deleSetDraggedPosition(obj);

    }

    private void onPointerUp(PointerEventData obj)
    {

    }

    private void onPointerDown(PointerEventData obj)
    {

    }

    public override void Render()
    {
        Texture2D texture = new Texture2D(4, 4);
        byte[] bytes = System.Convert.FromBase64String(itemData.imagemeta);
        texture.LoadImage(bytes);
        image.texture = texture;
        //itemData.imagemeta;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }
}
