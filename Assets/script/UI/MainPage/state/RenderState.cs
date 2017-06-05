using UnityEngine;
using System.Collections;
using System;

public class RenderState : MainPageState {

    public const string Name = "RenderState";

    private RenderPage render { get { return  RenderPage.Instance; } }
    private SchemePageController schemeControll { get { return SchemePageController.Instance; } }

    private SchemeManifest schemeManifest { get { return SchemeManifest.Instance; } }

    public override void enter()
    {
        base.enter();
        if (inputMachine.currentInputIs2D)
        {
            inputMachine.setState(HoldOnState2D.NAME);
        }
        else
        {
            inputMachine.setState(HoldOnState3D.NAME);
        }

        for (int i = 0; i < mainpage.ChildTransfrom.Count; i++)
        {
            mainpage.ChildTransfrom[i].gameObject.SetActive(false);
        }
        Debug.Log("Render Enter");
        mainpage.ChildNode.gameObject.SetActive(true);
        render.skin.SetActive(true);
    }

    public override void Ready()
    {
        base.Ready();
        render.SetData(mainpage.SkinRender);
        //render.aspect.onClick.AddListener(onRenderAspect);
        //render.renderScene.onClick.AddListener(onRenderRender);
        //render.environment.onClick.AddListener(onRenderEnvironment);
        //render.rotation.onClick.AddListener(onRenderRotation);
        //render.RectEnvironment.gameObject.AddComponent<EnvironmentScroll>();
        render.fourToThere.onClick.AddListener(onFourToThere);
        render.sixteenToNine.onClick.AddListener(onSixteenToNine);
        //render.perview.onValueChanged.AddListener(onPreview);
        render.common.onValueChanged.AddListener(onCommon);
        render.hight.onValueChanged.AddListener(onHight);
        //render.toggle0.onValueChanged.AddListener(onToggleOne);
        //render.toggle1.onValueChanged.AddListener(onToggleTwo);
        //render.toggle2.onValueChanged.AddListener(onToggleThere);
        //render.toggle3.onValueChanged.AddListener(onToggleFour);
        render.startRender.onClick.AddListener(onStartRender);
        render.exit.onClick.AddListener(exit);
        //render.colse.onClick.AddListener(onColse);
        //render.effectImage.onClick.AddListener(onEffectImage);
        //render.progressBar.onValueChanged.AddListener(onProgressBar);
        render.cancel.onClick.AddListener(onCancel);
        render.confirm.onClick.AddListener(onConfirm);
    }

    private void onStartRender()
    {
        UITool.SetActionTrue(render.messageBox.gameObject);
    }

    private void onConfirm()
    {
        setState(MainPageFreeState.Name);

        //EffextImage增加一项数据 - 有个开始时间
        if (schemeManifest.tempEffectMetas.Count > 0)
        {
            DateTime nowTime = DateTime.Now;
            int Year = nowTime.Year;
            int Month = nowTime.Month;
            int Day = nowTime.Day;
            int Hour = nowTime.Hour;
            int Minute = nowTime.Minute;
            int Second = nowTime.Second;
            string time = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", Year, Month, Day, Hour, Minute, Second);
            schemeManifest.images.Add(string.Format("{0}--{1}", time,schemeManifest.tempEffectMetas[0]));
        }
        schemeControll.OpenScheme();
        UITool.SetActionFalse(render.messageBox.gameObject);
    }

    private void onCancel()
    {
        schemeControll.onOpenEffect();
        UITool.SetActionFalse(render.messageBox.gameObject);
    }

    #region Render
    private void onRenderAspect()
    {
        render.RectAspect.gameObject.SetActive(!render.RectAspect.gameObject.activeSelf);
        Debug.Log("在3D 中进行分辨率调整");
    }
    private void onRenderRender()
    {
        UITool.SetActionTrue(render.progress.gameObject);
        Debug.Log("在3D中进行渲染");
    }
    private void onRenderEnvironment()
    {
        render.RectEnvironment.gameObject.SetActive(!render.RectEnvironment.gameObject.activeSelf);
        Debug.Log("在3D 中 渲染环境选择");
    }
    private void onRenderRotation()
    {
        Debug.Log("在3D中  设置360度旋转");
    }
    private void onFourToThere()
    {
        UITool.SetActionTrue(render.fourToThereBG.gameObject);
        render.fourToThereText.color = new Color32(255,255,255,255);
        UITool.SetActionFalse(render.sixteenToNineBG.gameObject);
        render.sixteenToNineText.color = new Color32(162,136,88,255);
    }
    private void onSixteenToNine()
    {
        UITool.SetActionFalse(render.fourToThereBG.gameObject);
        render.fourToThereText.color = new Color32(162, 136, 88, 255); 
        UITool.SetActionTrue(render.sixteenToNineBG.gameObject);
        render.sixteenToNineText.color = new Color32(255, 255, 255, 255);
    }
    private void onPreview(bool value)
    {
        Debug.Log("预览");
    }
    private void onCommon(bool value)
    {
        Debug.Log("普清");
    }
    private void onHight(bool value)
    {
        Debug.Log("高清");
    }
    private void onToggleOne(bool value)
    {
        Debug.Log("toggleOne");
    }
    private void onToggleTwo(bool value)
    {
        Debug.Log("toggleTwo");
    }
    private void onToggleThere(bool value)
    {
        Debug.Log("toggleThere");
    }
    private void onToggleFour(bool value)
    {
        Debug.Log("toggleFour");
    }
    #endregion
    private void onColse()
    {
        UITool.SetActionFalse(render.progress.gameObject);
        Debug.Log("关闭进度条"); 
    }
    private void onEffectImage()
    {
        UITool.SetActionFalse(render.progress.gameObject);
        Debug.Log("查看效果图");
    }
    private void onProgressBar(float value )
    {
        render.value.text = string.Format("{0}%",value);
        Debug.Log("显示进度条值");
    }
   private float tempLoadTime =10;
    private float progressValue = 0;
    IEnumerator loadProgressBar()
    {
        yield return new WaitForEndOfFrame();
        progressValue += 1f;
        render.progressBar.value = progressValue;
        if ((int)(progressValue)%10==0)
        {
            yield return new WaitForSeconds(0.5f);
            tempLoadTime--;
            render.Loadtime.text = string.Format("剩余时间约{0}", tempLoadTime);
        }
    }
    public override void exit()
    {
        if (inputMachine.currentInputIs2D)
        {
            inputMachine.setState(FreeState2D.NAME);
        }
        else
        {
            inputMachine.setState(FreeState3D.NAME);
        }
        for (int i = 0; i < mainpage.ChildTransfrom.Count; i++)
        {
            mainpage.ChildTransfrom[i].gameObject.SetActive(true);
        }
        Debug.Log("Render Exit");
        render.skin.SetActive(false);
        base.exit();
    }
}
