using UnityEngine;
using System.Collections;
using System;

public class SetWallController : UIController<SetWallController> {

    private SetWallPage wallPage;
    private Action<float, float, float> onOk = null;

    private string starlength;
    private string starwidth;
    private string starheigth;

    private MainPageUIController mainPageController
    {
        get { return MainPageUIController.Instance; }
    }
    private DefaultSettings defaultSettings
    {
        get {
            return DefaultSettings.Instance;
        }
    }

    public SetWallController()
    {
        panel = wallPage = SetWallPage.Instance;
    }
    public override void awake()
    {
        base.awake();
    }
    public override void ready()
    {
        base.ready();
        wallPage.lenghtButton.onClick.AddListener(onLenghtButton);
        wallPage.widthButton.onClick.AddListener(onWidthButton);
        wallPage.heightButton.onClick.AddListener(onHeightButton);
        
        wallPage.lenghtText.text = defaultSettings.DefaultLength.ToString();
        wallPage.heightText.text = defaultSettings.DefaultHeight.ToString();
        wallPage.widthText.text = defaultSettings.DefaultWidth.ToString();
        wallPage.wallTwoOK.onClick.AddListener(onWallTwoOK);
    }

    public void Open(Action<float, float, float> dele, object length = null, object width = null, object hight = null, bool lengthable = true, bool widthable = true, bool heightable = true)
    {
        Debug.Log("Open SetWallController");
        onOk = dele;
        wallPage.lenghtText.color = Color.black;
        wallPage.widthText.color = Color.black;
        wallPage.heightText.color = Color.black;
        wallPage.heightUnit.text = defaultSettings.DefaultUnit.ToString();
        wallPage.lenghtUnit.text = defaultSettings.DefaultUnit.ToString();
        wallPage.widthUnit.text = defaultSettings.DefaultUnit.ToString();
        if (length == null) {
            wallPage.lenghtText.text = defaultSettings.DefaultLength.ToString();
        } else {
            float floatlength = (float)length;
            floatlength = Mathf.Round(floatlength * 1000);
            floatlength = floatlength * (int)defaultSettings.DefaultUnit / 1000f;
            starlength = floatlength.ToString();
            wallPage.lenghtText.text = starlength;
        }
        if (width == null){
            wallPage.widthText.text = defaultSettings.DefaultWidth.ToString();
        } else {
            float floatwidth = (float)width;
            floatwidth = Mathf.Round(floatwidth * 1000);
            floatwidth = floatwidth * (int)defaultSettings.DefaultUnit / 1000f;
            starwidth = floatwidth.ToString();
            wallPage.widthText.text = starwidth;
        }
        if (hight == null) {
            wallPage.heightText.text = defaultSettings.DefaultHeight.ToString();
        } else {
            float floathight = (float)hight;
            floathight = Mathf.Round(floathight * 1000);
            floathight = floathight * (int)defaultSettings.DefaultUnit / 1000f;
            starheigth = floathight.ToString();
            wallPage.heightText.text = starheigth;
        }
        UITool.SetActionTrue(wallPage.lengthRect.gameObject);
        UITool.SetActionTrue(wallPage.widthRect.gameObject);
        UITool.SetActionTrue(wallPage.heightRect.gameObject);
        //wallPage.gridLayout.sizeDelta = Vector2.zero;
        wallPage.gridLayout.sizeDelta = Vector2.right * 420 + Vector2.up * 444;
        if (lengthable == false )
        {
            wallPage.gridLayout.sizeDelta -= Vector2.up * wallPage.lengthRect.sizeDelta.y;
            UITool.SetActionFalse(wallPage.lengthRect.gameObject);
        }
        if (widthable == false)
        {
            wallPage.gridLayout.sizeDelta -= Vector2.up * wallPage.widthRect.sizeDelta.y;
            UITool.SetActionFalse(wallPage.widthRect.gameObject);
        }
        if (heightable == false)
        {
            wallPage.gridLayout.sizeDelta -= Vector2.up * wallPage.heightRect.sizeDelta.y;
            UITool.SetActionFalse(wallPage.heightRect.gameObject);
        }

        wallPage.lenghtButton.interactable = lengthable;
        wallPage.widthButton.interactable = widthable;
        wallPage.heightButton.interactable = heightable;
        panel.skin.SetActive(true);
        //默认打开长度输入
        onLenghtButton();
    }

    public void Close()
    {
        Debug.Log("Close SetWallController");
        mainPageController.CloseInput();
        panel.skin.SetActive(false);
    }

    public override void sleep()
    {
        base.sleep();
    }

    #region AddListenerFunction
    private void onLenghtButton()
    {
        mainPageController.OpenInput(wallPage.lenghtText.text, wallPage.lenghtText, onChangeL);
    }
    private void onWidthButton()
    {
        mainPageController.OpenInput(wallPage.widthText.text, wallPage.widthText, onChangeW);
    }
    private void onHeightButton()
    {
        mainPageController.OpenInput(wallPage.heightText.text, wallPage.heightText, onChangeH);
    }

    private void onChangeL()
    {
        float len;
        if (float.TryParse(wallPage.lenghtText.text, out len))
        {
            float value = Mathf.Round(len * 1000f / (int)defaultSettings.DefaultUnit);
            if (value < 10 || value > 100000)
            {
                wallPage.lenghtText.color = Color.red;
            }
            else
            {
                wallPage.lenghtText.color = Color.grey;
            }
            wallPage.lenghtText.text = (value * (int)defaultSettings.DefaultUnit / 1000f).ToString(); 
        }
        else
        {
            wallPage.lenghtText.color = Color.red;
        }
    }
    private void onChangeW()
    {
        float len;
        if (float.TryParse(wallPage.widthText.text, out len))
        {
            if (len < 10 || len > 100000)
            {
                wallPage.widthText.color = Color.red;
            }
            else
            {
                wallPage.widthText.color = Color.grey;
            }
        }
        else
        {
            wallPage.widthText.color = Color.red;
        }
    }
    private void onChangeH()
    {
        float len;
        if (float.TryParse(wallPage.heightText.text, out len))
        {
            if (len < 10 || len > 100000)
            {
                wallPage.heightText.color = Color.red;
            }
            else
            {
                wallPage.heightText.color = Color.grey;
            }
        }
        else
        {
            wallPage.heightText.color = Color.red;
        }
    }

    private void onWallTwoOK()
    {
        //UITool.SetActionFalse(wallPage.rectWallHeightOrShort.gameObject);
        mainPageController.CloseInput();
        float length;
        if(wallPage.lenghtText.color == Color.red)
        {
            length = float.Parse(starlength);
        }
        else
        {
            starlength = wallPage.lenghtText.text;
            length = float.Parse(starlength);
        }
        float width;
        if (wallPage.widthText.color == Color.red)
        {
            width = float.Parse(starwidth);
        }
        else
        {
            starwidth = wallPage.widthText.text;
            width = float.Parse(starwidth);
        }
        float height;
        if (wallPage.heightText.color == Color.red)
        {
            height = float.Parse(starwidth);
        }
        else
        {
            starheigth = wallPage.heightText.text;
            height = float.Parse(starheigth);
        }

        if (onOk != null) onOk(length, width, height);
    }
    #endregion

}
