using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class SchemeImageContent : MonoBehaviour{


    public delegate void OnInitializeItem(GameObject go,int IndexDate);
	public OnInitializeItem initializeItem;

	public enum Arrangement
	{
		Horizontal,
		Vertical
	}
	public Arrangement arrangement = Arrangement.Vertical;
	public ScrollRect scrollRect;
	public RectTransform contentTrans;
	public GameObject itemPrefab;
	private int dataCount;
	private int curScrollPerLineIndex = -1;
	private List<SchemeImageContentItem> listItem;
	private Queue<SchemeImageContentItem> useItem;

	[Range(1,50)]
    [Header("每行出现最大格子数量")]
	public int maxPerLine = 2;
	[Header("格子宽")]
	public float cellWidth = 200f;
	[Header("格子高")]
	public float cellHeigth = 200f;
	[Header("格子与格子宽间距")]
	[Range(0,50)]
	public float cellWithSpace = 10f;
	[Header("格子与格子高间距")]
	[Range(0,50)]
	public float cellHeigthSpace = 10f;
	[Header("一面格子显示总数量")]
	[Range(0,50)]
	public int ViewCount = 5;

	public void Awake()
	{
		listItem = new List<SchemeImageContentItem> ();
		useItem = new Queue<SchemeImageContentItem> ();
        contentTrans = transform.GetComponent<RectTransform>();
        itemPrefab = transform.parent.parent.FindChild("ImageItemSkin").gameObject;
    }

    public void Init(int DataCount)
	{
		if (scrollRect ==null || contentTrans == null  || itemPrefab == null) {
			Debug.LogError ("异常：Content 脚本 变量没有赋值");
		}
		if (DataCount <=0) {
			return;
		}
		SetDataCount (DataCount);
		scrollRect.onValueChanged.RemoveAllListeners ();
		scrollRect.onValueChanged.AddListener (MyValueChanged);
        listItem.Clear();
        useItem.Clear();
        SetUpdataRectItem(0);
	}
    /**
     * @des:设置更新区域内item
     * 功能:
     * 1.隐藏区域之外对象
     * 2.更新区域内数据
     */
    void SetUpdataRectItem(int scrollPerLineIndex)
    {
        if (scrollPerLineIndex < 0)
        {
            return;
        }
        curScrollPerLineIndex = scrollPerLineIndex;
        int startDataIndex = curScrollPerLineIndex * maxPerLine;
        int endDataIndex = (curScrollPerLineIndex + ViewCount) * maxPerLine;
        // 移除
        for (int i = listItem.Count - 1; i >= 0; i--)
        {
            SchemeImageContentItem item = listItem[i];
            int index = item.Index;
            if (index < startDataIndex || index >= endDataIndex)
            {
                item.Index = -1;
                listItem.Remove(item);
                useItem.Enqueue(item);
            }
        }
        // 显示
        for (int dataIndex = startDataIndex; dataIndex < endDataIndex; dataIndex++)
        {
            if (dataIndex >= dataCount)
            {
                continue;
            }
            if (IsExistDataByDataIndex(dataIndex))
            {
                continue;
            }
            CreateItem(dataIndex);
        }
    }
    // 设置容器Count 数
    public void SetDataCount(int count)
	{
		if (dataCount == count) {
			return;
		}
		dataCount = count;
        
        SetUpdateContentSize();
	}
	// 添加 ScrollRect 事件
	public  void MyValueChanged(Vector2 vt2)
	{

        switch (arrangement)
        {
            case Arrangement.Horizontal:
                {
                    float x = vt2.x;
                    if (x >= 1.0f || x <= 0.0f)
                    {
                        return;
                    }
                }
                break;
            case Arrangement.Vertical:
                {
                    float y = vt2.y;
                    if (y >= 1.0f || y <= 0.0f)
                    {
                        return;
                    }
                }
                break;
        }
        int _curScrollPerLineIndex = GetCurScrollPerLineIndex();
        if (_curScrollPerLineIndex == curScrollPerLineIndex)
        {
            return;
        }
        SetUpdataRectItem(_curScrollPerLineIndex);
	}
    // 根据Content偏移 ，计算当前开始显示所在数据列表中的行和列
    public int GetCurScrollPerLineIndex()
    {
        switch (arrangement)
        {
            case Arrangement.Horizontal:
                return Mathf.FloorToInt(Mathf.Abs(contentTrans.anchoredPosition.x)/(cellWidth+cellWithSpace));
            case Arrangement.Vertical:
                return Mathf.FloorToInt(Mathf.Abs(contentTrans.anchoredPosition.y)/(cellHeigth+cellHeigthSpace));
        }
        return 0;
    }
	// 更新容器大小
	public void SetUpdateContentSize()
	{
        int lineCount = Mathf.CeilToInt((float)dataCount/maxPerLine);
        switch (arrangement)
        {
            case Arrangement.Horizontal:
                contentTrans.sizeDelta = new Vector2(cellWidth * lineCount + cellWithSpace * (lineCount-1), contentTrans.sizeDelta.y);
                break;
            case Arrangement.Vertical:
                contentTrans.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                contentTrans.sizeDelta = new Vector2(contentTrans.sizeDelta.x,cellHeigth*lineCount + cellHeigthSpace*(lineCount-1));
                break;
        }
        
	}
	// 当前数据是否存在List 中
	bool IsExistDataByDataIndex(int dataindex)
	{
		if (listItem == null || listItem .Count <=0) {
			return false;
		}
		for (int i = 0; i < listItem.Count; i++) {
			if (listItem[i].Index == dataindex) {
				return true;
			}
		}
		return false;
	}
	// 创建 一个 对象数据
	void CreateItem (int dataIndex)
	{
        SchemeImageContentItem item;
		if (useItem.Count >0) {
			item = useItem.Dequeue ();
		} else {
            item = AddChild(itemPrefab, contentTrans).GetComponent<SchemeImageContentItem>(); 
		}
        item.MyContent = this;
        item.Index = dataIndex;
        listItem.Add(item);
    }
	// 添加孩子
	GameObject AddChild(GameObject goPrefab,Transform parent)
	{
        if (goPrefab == null || parent == null )
        {
            Debug.LogError("异常");
            return null;
        }
        GameObject goChild = GameObject.Instantiate(goPrefab) as GameObject;
        goChild.layer = parent.gameObject.layer;
        goChild.transform.SetParent(parent,false);
        return goChild;
	}
	// 获取当前Index下对应Content下的本地坐标
	public Vector3 GetLocalPositionByIndex(int index)
	{
		float x = 0f;
		float y = 0f; 
		float z = 0f; 
		switch (arrangement) {
		case Arrangement.Horizontal:
                {
                    x = (index / maxPerLine) * (cellWidth + cellWithSpace);
                    y = -(index % maxPerLine) * (cellHeigth + cellHeigthSpace);
                }
                break;
		case Arrangement.Vertical:
                {
                    x = (index % maxPerLine) * (cellWidth + cellWithSpace);
                    y = -(index / maxPerLine) * (cellHeigth + cellHeigthSpace);
                }
			break;
		}
		return new Vector3 (x,y,z);
	}
    // 添加当前数据索引数据
    public void AddItem(int dataIndex)
    {
        if (dataIndex<0 || dataIndex > dataCount)
        {
            return;
        }
        // 检测是否需要添加gameObject
        bool isNeedAdd = false;
        for (int i = listItem.Count -1; i  >=0; i--)
        {
            SchemeImageContentItem item = listItem[i];
            if (item.Index >= (dataCount -1))
            {
                isNeedAdd = true;
                break;
            }
        }
        SetDataCount(dataCount + 1);

        if (isNeedAdd)
        {
            for (int i = 0; i < listItem.Count; i++)
            {
                SchemeImageContentItem item = listItem[i];
                int oldIndex = item.Index;
                if (oldIndex >= dataIndex)
                {
                    item.Index = oldIndex + 1;
                }
                item = null;
            }
            SetUpdataRectItem(GetCurScrollPerLineIndex());
        }
        else
        {
            //重新刷新数据
            for (int i = 0; i < listItem.Count; i++)
            {
                SchemeImageContentItem item = listItem[i];
                int oldIndex = item.Index;
                if (oldIndex >= dataIndex)
                {
                    item.Index = oldIndex;
                }
                item = null;
            }
        }
    }
	// 删除当前数据索引下数据
	public void DelItem(int dataIndex)
	{
		if (dataIndex < 0 || dataIndex >= dataCount) {
			return;
		}
		bool isNeedDestroyGameObject = (listItem.Count >= dataCount);
		SetDataCount (dataCount-1);
		for (int i = listItem.Count-1; i >=0; i--) {
            SchemeImageContentItem item = listItem[i];
			int oldIndex = item.Index;
			if (oldIndex == dataIndex) {
				listItem.Remove (item);
				if (isNeedDestroyGameObject) {
					GameObject.Destroy (item.gameObject);
				} else {
					item.Index = -1;
					useItem.Enqueue (item);
				}
			}
			if (oldIndex > dataIndex) {
				item.Index = oldIndex - 1;
			}
		}
		//SetUpdateContentSize ();
	}
    // Use this for initialization
    void OnDestroy()
    {
        scrollRect = null;
        contentTrans = null;
        itemPrefab = null;
        initializeItem = null;
        listItem.Clear();
        listItem = null;
        useItem = null;
    }

 
}
