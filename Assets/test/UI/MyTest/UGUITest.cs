using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UGUITest : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
    public Button MGO;
    public Sprite sprite;
    //public Sprite sprite2;
	// Update is called once per frame
	void Update () {

    }

    public void Enter()
    {
        Debug.Log("进入到另一个界面 ");
    }
    public void Exit()
    {
        Debug.Log("退出当前页面  进入另一个界面");
    }
    public bool IS;
    public void changeImage()
    {
        
        IS = !IS; 
        if (!IS)
        {
            this.GetComponent<Image>().sprite = sprite;
            Debug.Log(this.GetComponent<Image>().sprite.name);
        }
        else
        {

            this.GetComponent<Image>().sprite = this.GetComponent<Button>().spriteState.pressedSprite;
            Debug.Log(this.GetComponent<Image>().sprite.name);
        }
    }
}
