using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpritePacker : MonoBehaviour {


    public Dictionary<string, Texture> loadTextureDic = new Dictionary<string, Texture>();

    public Dictionary<string, Sprite> commonSpriteDic = new Dictionary<string, Sprite>();

    public Dictionary<string, Sprite> mainPageSpriteDic = new Dictionary<string, Sprite>();

    private void Awake()
    {
        init();
    }

    private void init()
    {
        LoadTexture();
        LoadCommonSprite();
        LoadMainPageSprite();
    }
    [Tooltip("加载Texture")]
    public List<Texture> mLoadTextureList;
    private void LoadTexture()
    {
        foreach (Texture item in mLoadTextureList)
        {
            if (!loadTextureDic.ContainsValue(item))
                loadTextureDic.Add(item.name,item);
        }
    }
    [Tooltip("加载共同使用的Sprite")]
    public List<Sprite> mLoadCommonSpriteList;
    private void LoadCommonSprite()
    {
        foreach (Sprite item in mLoadCommonSpriteList)
        {
            if (!commonSpriteDic.ContainsValue(item))
                commonSpriteDic.Add(item.name,item);
        }
    }
    [Tooltip("加载主页面Sprite")]
    public List<Sprite> mLoadMainPageSpriteList;
    private void LoadMainPageSprite()
    {
        foreach (Sprite item in mLoadMainPageSpriteList)
        {
            if (!mainPageSpriteDic.ContainsValue(item))
                mainPageSpriteDic.Add(item.name,item);
        }
    }
}
