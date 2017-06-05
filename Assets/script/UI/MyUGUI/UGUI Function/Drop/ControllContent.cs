using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class ControllContent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IMoveHandler,IPointerEnterHandler,IDragHandler
{

	// Use this for initialization
	void Start () {
	
	}

    public ScrollRect sr;
    public CanvasGroup cg;
    private bool isDownItem = false;
    private bool setActive = false;
    private GameObject downObj;

    private bool startDrag = false;
    private float beginTime = 0.0f;

    private float shortPressDelay = 0.2f;
    private float loapPressDelay = 1f;
    // Update is called once per frame
    void Update () {

        if (setActive) return;
        if (isDownItem == false) return;
        if (Time.time - beginTime > loapPressDelay)
        {
            sr.vertical = false;
            setActive = true;
            Debug.Log("vertical = false");
            //cg.blocksRaycasts = true;
            //if (!GetComponent<UGUIDrag>())
            //{
            //    DragManager.Instance.IsScroll = true;
            //    this.gameObject.AddComponent<UGUIDrag>();
            //}
        }
        else
        {
           
            
            //if (GetComponent<UGUIDrag>())
            //{
            //    DragManager.Instance.IsScroll = false;
            //    Destroy(GetComponent<UGUIDrag>());
            //}
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //sr.vertical = true;
        downObj = eventData.pointerCurrentRaycast.gameObject;
        ///
        Debug.Log("Down name "+ downObj.name);
        beginTime = Time.time;
        isDownItem = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDownItem = false;
        setActive = false;
        sr.vertical = true;
        Debug.Log("vertical = true");
        //if (obj == eventData.pointerCurrentRaycast.gameObject)
        //{
        //    cg.blocksRaycasts = false;
        //}       
        //cg.blocksRaycasts = false;
        Debug.Log("up name " + eventData.pointerCurrentRaycast.gameObject.name);
    }

    public void OnMove(AxisEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (setActive)
        {

        }
        //Debug.Log(eventData.pointerDrag.gameObject.name);

    }
}
