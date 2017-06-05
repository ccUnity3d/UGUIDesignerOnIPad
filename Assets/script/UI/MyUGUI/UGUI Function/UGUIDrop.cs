using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UGUIDrop : MonoBehaviour,IDropHandler {

    public Image receivingImage;

    public void OnDrop(PointerEventData eventData)
    {
        //if (receivingImage == null)
        //    return;
        Sprite dropSprite = GetDropSprite(eventData);
        //if (dropSprite != null)
        //    receivingImage.overrideSprite = dropSprite;
        Debug.Log("sdfsdf");
    }
    Sprite GetDropSprite(PointerEventData data)
    {
        var originalObj = data.pointerDrag;
        if (originalObj == null )
            return null;

        var srcImage = originalObj.GetComponent<Image>();

        if (srcImage == null )
            return null;
        return srcImage.sprite;
    }
}
