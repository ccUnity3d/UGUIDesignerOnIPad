using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class ScrollViewTest : MonoBehaviour {

    private List<Item> mySprite;
    private Content myContent;

    void Start()
    {
        mySprite = new List<Item>();
        for (int i = 0; i < 30; i++)
        {
            mySprite.Add(new Item("家居：" + Random.Range(1, 100)));
        }
        myContent = gameObject.transform.GetComponentInChildren<Content>();
        myContent.initializeItem = OnInitializeItem;
        myContent.Init(mySprite.Count);
    }
    private void OnInitializeItem(GameObject go, int dataIndex)
    {
        Text text = go.transform.FindChild("Text").GetComponent<Text>();
        text.text = dataIndex + mySprite[dataIndex].name;
        go.GetComponent<Button>().onClick.AddListener(()=>{ });
        //go.AddComponent<UGUIDrag>().myScroll = myContent.scrollRect;
        /*
        //添加功能
        Button addButton = go.transform.FindChild("Add").GetComponent<Button>();
        addButton.onClick.RemoveAllListeners();
        addButton.onClick.AddListener(delegate() {
            mySprite.Insert(dataIndex+1,new Item("Insert"+Random.Range(1,100)));
            myContent.AddItem(dataIndex+1);
        });
        // 删除功能
        Button subButton = go.transform.FindChild("Sub").GetComponent<Button>();
        subButton.onClick.RemoveAllListeners();
        subButton.onClick.AddListener(delegate()
        {
            mySprite.RemoveAt(dataIndex);
            myContent.DelItem(dataIndex);
        });
        */
    }

    // Use this for initialization

    // Update is called once per frame
    void Update () {
	    
	}
   

    public class Item
    {
        public string name;
        public Item(string name)
        {
            this.name = name;
        }
        public void Destroy()
        {
            name = null;
        }
    }
}

