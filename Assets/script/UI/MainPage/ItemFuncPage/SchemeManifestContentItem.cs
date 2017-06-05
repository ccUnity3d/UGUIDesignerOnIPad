using UnityEngine;
using System.Collections;

public class SchemeManifestContentItem : MonoBehaviour {

	private int index;
	private SchemeManifestContent myContent;
	void OnDestroy()
	{
		myContent = null;		
	}
	public SchemeManifestContent MyContent {
	set{
			myContent = value;
 		}
        get
        {
            return myContent;
        }
	}
	public int Index
	{
		set{ 
			index = value;
			// 设置 对象位置
			transform.localPosition = myContent.GetLocalPositionByIndex (index);
			// 设置对象名字
			gameObject.name = (index<10)?("0"+index):(""+index);
			// 给委托 赋值
			if (myContent.initializeItem != null && index >= 0 ) {
				myContent.initializeItem (gameObject,index);
			}
		}		
		get{ 
			return index;
		}
	}
    public void ButtonPrint()
    {
        Debug.Log(index);
    }
}
