using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Image))]
//IBeginDragHandler   IEndDragHandler   IDragHandler, IEndDragHandler, IBeginDragHandler,
public class UGUIDrag : MonoBehaviour,IDragHandler,IPointerEnterHandler, IBeginDragHandler, IEndDragHandler
{
    private GameObject m_DraggingIcon;
    private RectTransform m_DraggingPlane;
    private RectTransform transf;
    public ScrollRect myScroll;

    private bool isDown = false;
    private bool startDrag = false;
    private float beginTime = 0.0f;

    private float shortPressDelay = 0.2f;
    private float loapPressDelay = 1f;
    private  List<RaycastResult> list = new List<RaycastResult> ();


    void Start()
    {
        myScroll = GetComponent<ContentItem>().MyContent.scrollRect;
        myScroll.vertical = false;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBegin");
        //    transf = GetComponent<RectTransform>();
        //    if (transf ==null)
        //    {
        //        Debug.Log("nULL");
        //    }
        //    //myScroll.vertical = false;
        //    //var canvasGroup = FindInParents<CanvasGroup>(gameObject);
        //    //if (canvasGroup == null)
        //    //    return;
        //    //m_DraggingIcon = new GameObject("Icon");
        //    //m_DraggingIcon.transform.SetParent(canvasGroup.transform, false);
        //    //m_DraggingIcon.transform.SetAsFirstSibling();
        //    //var image = m_DraggingIcon.AddComponent<Image>();
        //    //image.SetNativeSize();
        SetDraggedPosition(eventData);
    //    //Debug.Log("BeginDrag");
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // 每帧 都会执行
        Debug.Log("OnDrag");
        SetDraggedPosition(eventData);
       
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEnd");
        //        // 这里 可以 将 Icon 转换 3D Model 到 3D 世界
        //        /*
        //         */
        //        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ////        go.transform.position = m_DraggingIcon.transform.position;
        //        //if (m_DraggingIcon != null)
        //        //{
        //        //    Destroy(m_DraggingIcon);
        //        //}
        //        isDown = false;
        //        startDrag = false;
               myScroll.vertical = true;
        //        DragManager.Instance.IsScroll = true;
     }
        void SetDraggedPosition(PointerEventData data)
        {
        if (data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null )
        {
            m_DraggingPlane = data.pointerEnter.transform as RectTransform;
        }
        //transf.pivot = new Vector2(0.5f,0.5f);
        this.GetComponent<RectTransform>().anchoredPosition3D = data.position;
        //transf.transform.position = data.position;
        Debug.Log("position");
        //Vector2 globalMousePos;
        //Vector2 vec = new Vector2(data.position.x,data.position.y);
      //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_DraggingPlane, vec, data.pressEventCamera, out globalMousePos))
      //  {
      //      transf.anchoredPosition3D = new Vector3(globalMousePos.x-276, globalMousePos.y +200, 0);
      //  }
        //transf.anchoredPosition = new Vector2(transf.anchoredPosition.x+1, transf.anchoredPosition.y + 1);
        //if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane,data.position,data.pressEventCamera,out globalMousePos))
        //{
        //    transf.anchoredPosition3D = new Vector3(globalMousePos.x+272, globalMousePos.y+200,0);
        //   // m_DraggingIcon.transform.position = globalMousePos;
        //}
    }
    static public T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null)
            return null;
   
        var component = go.GetComponent<T>();

        if (component != null)
            return component;

        Transform t = go.transform.parent;
        while (t != null && component == null )
        {
            component = t.GetComponent<T>();
            t = t.parent;
        }
        return component;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }


    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    if (startDrag)
    //    {
    //        SetDraggedPosition(eventData);
    //    }
    //}
}
