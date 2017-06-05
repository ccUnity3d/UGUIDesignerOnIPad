using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class FullImageItemFunc : UGUIItemFunction,IDragHandler,IEndDragHandler
{
    public Action onRestFiexdPos;

    private RawImage EffectImage;

    private RectTransform imageRect;

    private EffectImageItem itemData
    {
        get { return data as EffectImageItem; }
    }
    public void OnDrag(PointerEventData eventData)
    {

    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (onRestFiexdPos != null) onRestFiexdPos();
    }

    protected override void Awake()
    {
        EffectImage = UITool.GetUIComponent<RawImage>(this.transform, "GameObject/image");
        imageRect = GetComponent<RectTransform>();
    }
    public override void Render()
    {
        base.Render();
        //EffectImage;

      string [] strs = itemData.path.Split(new string[] { "--"}, StringSplitOptions.RemoveEmptyEntries);
        if (strs.Length == 2)
        {
            string path = strs[1];

            LoaderPool.OutterLoad(path, SimpleLoadDataType.texture2D, onLoadedSetTexture, null);
            //SetTextureTool.SetTexture(EffectImage, path, "notLocal");
        }
        else {
            Debug.Log("itemData.meta strs.Length != 2");
        }
    }

    private void onLoadedSetTexture(object obj)
    {
        SimpleLoader loader = obj as SimpleLoader;
        Texture2D texture = loader.loadedData as Texture2D;
        EffectImage.texture = texture;
        Vector3 size = Vector3.right * 2048 + Vector3.up * 1536;
        float multi = (texture.height * 1f / texture.width);
        if (multi > 3f / 4)
        {
            size = Vector2.right * 1536 / multi + Vector2.up * 1536;
        }
        else
        {
            size = Vector2.right * 2048 + Vector2.up * 2048 * multi;
        }
        EffectImage.rectTransform.sizeDelta = size;
    }
}
