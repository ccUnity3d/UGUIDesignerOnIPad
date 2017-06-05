using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class JsonReadTest : MonoBehaviour {

    public void Start()
    {
        Object obj = Resources.Load("design");
        string msg = obj.ToString();
        Debug.Log(msg);
        //JsonProjectData data = new JsonProjectData();
        //data.meta = new JsonProjectData.JsonMeta();
        //data.data.Add(new JsonProjectData.JsonCameraData());
        //data.data.Add(new JsonProjectData.JsonContentData());
        //data.data.Add(new JsonProjectData.JsonCoWallData());
        //data.data.Add(new JsonProjectData.JsonDoorData());
        //data.data.Add(new JsonProjectData.JsonFloorplan());
        //data.data.Add(new JsonProjectData.JsonMaterialData());
        //data.data.Add(new JsonProjectData.JsonModelData());
        //data.data.Add(new JsonProjectData.JsonMoldingData());
        //data.data.Add(new JsonProjectData.JsonPointData());
        //data.data.Add(new JsonProjectData.JsonRoom());
        //data.data.Add(new JsonProjectData.JsonUnderlayData());
        //data.data.Add(new JsonProjectData.JsonWallData());
        //data.products.Add(new JsonProjectData.JsonProductData());
        //string msg = MyJsonTool.ToJson(data);

        JsonProjectData data = MyJsonTool.ReadProjectJson(msg);
        //string str = MyJsonTool.ToJson(data);
        //Debug.Log(data);
        //Debug.Log(str);
        string outpath = Application.dataPath + "/msg.txt";
        using (StreamWriter writer = new StreamWriter(outpath, true, System.Text.Encoding.UTF8))
        {
            writer.WriteLine(msg);
        }

        string msg2 = MyJsonTool.ToJson(data);
        string outpath2 = Application.dataPath + "/msg2.txt";
        using (StreamWriter writer = new StreamWriter(outpath2, true, System.Text.Encoding.UTF8))
        {
            writer.WriteLine(msg2);
        }
    }
}
