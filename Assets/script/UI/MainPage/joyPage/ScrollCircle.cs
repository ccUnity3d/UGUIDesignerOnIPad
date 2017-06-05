using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollCircle : ScrollRect
{

    protected float mRadius;



    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        base.content.anchoredPosition = Vector3.ClampMagnitude(base.content.anchoredPosition, 120);
    }
    
}
