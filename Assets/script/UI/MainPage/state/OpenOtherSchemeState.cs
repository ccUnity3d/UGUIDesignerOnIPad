using UnityEngine;
using System.Collections;
using System;

public class OpenOtherSchemeState : MainPageState
{
    public const string Name = "OpenSchemeState";
    public override void enter()
    {
        base.enter();
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            MsgToIOS msg = new MsgToIOS();
            msg.code = "101009";
            UnityIOSMsg.sendToIOS(msg);
            setState(MainPageFreeState.Name);
        }
        else {
            tempDo();
        }
        setState(MainPageFreeState.Name);
        inputMachine.setState(FreeState2D.NAME);
    }

    private void tempDo()
    {
        MsgFromIOS msg = new MsgFromIOS();
        /*
        cid = "e1ce9df2-195b-4c93-8275-05c60a3c5094";
        contentType = 1;
        createdTime = 1468992842;
        data = "http://midea-staging-contents.oss-cn-shanghai.aliyuncs.com/designs/e1ce9df2-195b-4c93-8275-05c60a3c5094/af55aac3-fabf-4a80-a7b6-1b574a5b79c0.midf";
        description = "";
        id = 58;
        meta = "http://midea-staging-contents.oss-cn-shanghai.aliyuncs.com/designs/74a9e4e0-9fa1-43bb-96f8-c028f5058f54/afde0370-01e2-4542-a27a-5742516597f8.jpg";
        name = Empty;
        option = " ";
        provider = millionideas;
        status = 1;
        updatedTime = 1468992842;
        userId = "a61f96dd-35cf-418b-8a45-c9b3086e11f8";
         */

        msg.code = "201001";
        msg.info = new MsgFromIOS.InfoFromIOS();
        msg.info.result = "1";
        msg.info.enterType = "3";

        int template = 4;
        switch (template)
        {
            case 0:
                msg.info.projectId = "e1ce9df2-195b-4c93-8275-05c60a3c5094";
                msg.info.json = "http://midea-staging-contents.oss-cn-shanghai.aliyuncs.com/designs/e1ce9df2-195b-4c93-8275-05c60a3c5094/af55aac3-fabf-4a80-a7b6-1b574a5b79c0.midf";
                break;
            case 1:
                msg.info.projectId = "67d6b9ee-9b0e-4acb-a38c-0b1cc0b3b054";
                msg.info.json = "http://midea-staging-contents.oss-cn-shanghai.aliyuncs.com/designs/67d6b9ee-9b0e-4acb-a38c-0b1cc0b3b054/53bc64e7-0b00-41ad-9c1f-4f6e856700a2.midf";
                break;
            case 2:
                msg.info.projectId = "a127a624-68eb-4e4a-8a8a-9d96a73d38cd";
                msg.info.json = "http://midea-staging-contents.oss-cn-shanghai.aliyuncs.com/designs/a127a624-68eb-4e4a-8a8a-9d96a73d38cd/8ca44052-3c60-4803-a6b5-f4a12c06fb7d.midf";
                break;
            case 3:
                msg.info.projectId = "c32b1323-e035-4e6f-9785-f5353593a95c";
                msg.info.json = "http://midea-staging-contents.oss-cn-shanghai.aliyuncs.com/designs/c32b1323-e035-4e6f-9785-f5353593a95c/45e2c2f9-5e62-4972-93ed-60b90c6f74c8.midf";
                break;
            case 4:
                msg.info.projectId = "22819d4e-064b-4918-b8fd-533d7e74d327";
                msg.info.json = "http://midea-staging-contents.oss-cn-shanghai.aliyuncs.com/designs/22819d4e-064b-4918-b8fd-533d7e74d327/69b90547-7380-49c1-9881-76c4c844b240.midf";
                break;
            case 5:
                msg.info.projectId = "acfae042-275d-4e3a-93fa-9e228b5eac36";
                msg.info.json = "http://midea-staging-contents.oss-cn-shanghai.aliyuncs.com/designs/acfae042-275d-4e3a-93fa-9e228b5eac36/dafe98b0-57bd-49cf-a8a8-ef72fc81b775.midf";
                break;
            case 6:
                msg.info.projectId = "301bc872-1b62-4146-b7da-2422f0ca3df8";
                msg.info.json = "http://midea-staging-contents.oss-cn-shanghai.aliyuncs.com/designs/301bc872-1b62-4146-b7da-2422f0ca3df8/3664cfac-8a63-4f35-9004-945e58d79271.midf";
                break;
            case 7:
                msg.info.projectId = "0aa848f4-0001-4917-84db-e887f4ad135f";
                msg.info.json = "http://midea-staging-contents.oss-cn-shanghai.aliyuncs.com/designs/0aa848f4-0001-4917-84db-e887f4ad135f/f5a979fc-5051-49af-9d8f-12729c8cd612.midf";
                break;
            default:
                break;
        }

        UnityIOSMsg.Instance.receiveFromIOSMsg(msg);
        //undoHelper.CreatNewEnter();
        //string path = "file://" + Application.dataPath + "/OriginalProjectJson/temp/mydesign0.json";
        //MyMono.MyStartCoroutine(LoadJson, this, path);
    }

    //private IEnumerator LoadJson(object[] args)
    //{
    //    string path = args[0].ToString();
    //    WWW www = new WWW(path);
    //    yield return www;
    //    if (string.IsNullOrEmpty(www.error))
    //    { 
    //        string json = System.Text.Encoding.UTF8.GetString(www.bytes);
    //        int index = json.IndexOf("{");
    //        if (index != 0)
    //        {
    //            json = json.Substring(index);
    //        }
    //        WriteToLocal(json);
    //        object obj = MyJsonTool.FromJson(json);
    //        OriginalProjectData data = new OriginalProjectData();
    //        data.DeSerialize(obj as System.Collections.Generic.Dictionary<string, object>);
    //        //TotalData total = new TotalData();
    //        //total.data = data.data;
    //        //total.schemeManifest.id = data.id;
    //        //total.schemeManifest.meta = data.meta;
    //        //total.schemeManifest.name = data.name;
    //        //total.schemeManifest.description = data.description;
    //        //total.schemeManifest.tempId = data.tempId;
    //        //total.schemeManifest.isNew = data.isNew;
    //        undoHelper.SetCurrentData(data);
    //        RefreshView();
    //    }
    //    else {
    //        Debug.LogError(www.error);
    //    }
    //}


    private void WriteToLocal(string json)
    {
        string outpath = Application.persistentDataPath + "/OriginalProjectJson/";

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            outpath = Application.dataPath + "/OriginalProjectJson/";
        }
        outpath += "temp/test3.json";

        System.IO.FileInfo info = new System.IO.FileInfo(outpath);
        if (info.Exists == true)
        {
            info.Delete();
        }
        if (info.Directory.Exists == false)
        {
            info.Directory.Create();
        }
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(outpath, true, System.Text.Encoding.UTF8))
        {
            writer.WriteLine(json);
        }
    }

    public override void exit()
    {
        base.exit();
    }

}
