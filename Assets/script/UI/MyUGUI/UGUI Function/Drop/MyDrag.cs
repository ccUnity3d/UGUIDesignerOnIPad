using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class MyDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private GameObject GameData;
    private Vector3 org;
    public void OnBeginDrag(PointerEventData eventData)
    {
        GameData = eventData.pointerDrag;
        org = eventData.pointerDrag.transform.position;
        if (GameData.GetComponent<GraphicRaycaster>() == null)
        {
            GameData.AddComponent<GraphicRaycaster>();
            GameData.GetComponent<Canvas>().overrideSorting = true;
            GameData.GetComponent<Canvas>().sortingOrder = 1;
        }
        else
        {
            GameData.GetComponent<Canvas>().sortingOrder += 2;

        }
    }
    public void enterPoint()
    {
        Debug.Log("Enter");
    }
    public void OnDrag(PointerEventData eventData)
    {
       
        SetPositionDrag(eventData);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {

        collision.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(100,100,0);
        Debug.Log(collision.name);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector3 end = eventData.pointerDrag.transform.position;
        if (Mathf.Abs (end.x-org.x)>3f)
        {
            Debug.Log("移动");
            return;
        }
        if (Mathf.Abs(end.y- org.y) > 3f)
        {
            Debug.Log("移动");
            return;
        }
        Debug.Log("拖拽结束");
    }
    public void SetPositionDrag(PointerEventData eventdata)
    {
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(this.GetComponent<RectTransform>(), eventdata.position, eventdata.pressEventCamera, out globalMousePos))
        {
            transform.position = globalMousePos;
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

}
