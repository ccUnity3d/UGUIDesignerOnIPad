using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MaterailImageItemFunc : UGUIItemFunction,IBeginDragHandler,IEndDragHandler {


    protected override void Awake()
    {
        image = this.gameObject.GetComponent<RawImage>();
        DrapButton button = this.gameObject.AddComponent<DrapButton>();
        button.onPointerDownDele = onPointerDown;
        button.onPointerUpDele = onPointerUp;
        button.onDragDele = onPointDrag;
        button.onPointerUpDele = onClick;        
        MaterialDelete = UITool.GetUIComponent<Button>(transform, "delete");
    }

    private MaterialItemData materialData
    {
        get {
            return data as MaterialItemData;
        }
    }

    private Button MaterialDelete;
    private Vector2 inputStartPos;
    private Vector2 offset = Vector2.zero;

    private bool dragCopyed = false;

    public Action<MaterailImageItemFunc> deleFun;
    public Action<PointerEventData> deleFollowMouse;
    public Action<PointerEventData> deleSetDraggedPosition;
    public Action<MaterialItemData> deleSetDraggingIcon;
    public Action deleEndDragCopy;

    private RawImage image;

    //private UScrollList uscroll;
    void Start()
    {
        if (materialData.special)
        {
            //隐藏

            return;
        }
        MaterialDelete.onClick.AddListener(onDelete);
    }

    public override void Render()
    {
        base.Render();
        if (materialData.special) return;
        SetTextureTool.SetTexture(image, materialData.textureURL, materialData.seekId);
    }

    private void onDelete()
    {
        Debug.Log("删除材质球");
        if (deleFun != null) deleFun(this);
    }
    private void onPointDrag(PointerEventData obj)
    {
        if (dragCopyed)
        {
            if (deleFollowMouse != null) deleFollowMouse(obj);
            return;
        }
        offset = obj.delta;
        if (offset == Vector2.zero)
        {
            return;
        }
        if (deleSetDraggedPosition != null) deleSetDraggedPosition(obj);
    }
    
    private void onClick(PointerEventData obj)
    {

    }

    private void onPointerUp(PointerEventData obj)
    {

    }

    private void onPointerDown(PointerEventData obj)
    {
        inputStartPos = obj.position;
    }
   
    protected override void Update()
    {
        base.Update();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("BeginDrag");
        if (Mathf.Abs(eventData.delta.x)>Mathf.Abs(eventData.delta.y))
        {
            //Sprite spri = this.GetComponent<Image>().sprite; ;
            dragCopyed = true;
            scroRect.vertical = false; 
            if(deleSetDraggingIcon!=null) deleSetDraggingIcon(materialData);
        }
        //SetDraggedPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        dragCopyed = false;
        scroRect.vertical = true;

        if (deleEndDragCopy != null) deleEndDragCopy();
    }
}
