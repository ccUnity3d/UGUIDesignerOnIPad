using System.Collections;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Net;
using System;

public class testLoader:MonoBehaviour {

    //public List<GameObject> list = new List<GameObject>();
    //private RaycastHit[] hits = new RaycastHit[20];

    IEnumerator Start()
    {
        //string p1 = "File://D:/GitProjects/mi-mobile-3d-tool/UGUIDesignerOnIPad/Assets/Prefabs/2D/prefab/iconbase2d.prefab";
        //string p2 = "http://midea-prod-assets.s3.cn-north-1.amazonaws.com.cn/i/b63a5fcb-607c-4aab-87ec-bbe89e9ce676/model-norm.obj";
        //string p3 = "http://midea-prod-assets.s3.cn-north-1.amazonaws.com.cn/i/b63a5fcb-607c-4aab-87ec-bbe89e9ce676/model-fc.gz";
        //string p4 = "File://D:/GitProjects/mi-mobile-3d-tool/UGUIDesignerOnIPad/Assets/StreamingAssets/Web/2D/prefab/iconbase2d.assetbundle";
        //string p5 = "http://midea-prod-assets.s3.cn-north-1.amazonaws.com.cn/i/7e3e90d9-da17-459d-b7ed-14e681519d6a/Top.png";

        //Stream outStream = File.Create(Application.streamingAssetsPath + "/Top.png");
        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(p5);
        //WebResponse response = request.GetResponse();
        //Stream inStream = response.GetResponseStream();

        //byte[] bytes = new byte[inStream.Length];
        //inStream.Read(bytes, 0, bytes.Length);
        //// 设置当前流的位置为流的开始
        //inStream.Seek(0, SeekOrigin.Begin);
        //Texture2D texture = new Texture2D(4,4);
        //texture.LoadImage(bytes);
        //transform.GetComponent<Renderer>().material.mainTexture = texture;

        //int bufferSize = 1024;
        //int readCount;
        //byte[] buffer = new byte[bufferSize];
        //readCount = inStream.Read(buffer, 0, bufferSize);
        //while (readCount>0)
        //{
        //    outStream.Write(buffer, 0, readCount);
        //    readCount = inStream.Read(buffer, 0, bufferSize);
        //}



        //outStream.Close();
        //inStream.Close();
        //response.Close();

        //WWW www = new WWW(p1);
        //yield return www;
        //if (string.IsNullOrEmpty(www.error))
        //{
        //    byte[] bytes = www.bytes;
        //    //AssetBundle bundle = GameObject.LoadFromMemory(bytes);
        //    //Object obj = bundle.mainAsset;
        //    //Object obj = www.assetBundle.mainAsset;
        //    //byte[] bts = Serialize(obj);
        //    //object obj2 = Deserialize(bts);

        //    //MemoryStream memStream = new MemoryStream();
        //    //BinaryFormatter binForm = new BinaryFormatter();

        //    //memStream.Write(bytes, 0, bytes.Length);
        //    //memStream.Seek(0, SeekOrigin.Begin);

        //    //object obj = (object)binForm.Deserialize(memStream);

        //    //return obj;

        //    //object _newOjb = null;

        //    //System.IO.MemoryStream _memory = new System.IO.MemoryStream(bytes);
        //    //_memory.Position = 0;
        //    //BinaryFormatter formatter = new BinaryFormatter();
        //    //_newOjb = formatter.Deserialize(_memory);
        //    //_memory.Close();

        //    //doc.Value;
        //    //try
        //    //{
        //    //    ByteArrayStream bis = new Stream(bytes);
        //    //    ObjectInputStream ois = new ObjectInputStream(bis);
        //    //    obj = ois.readObject();
        //    //    ois.close();
        //    //    bis.close();
        //    //}
        //    //catch (IOException ex)
        //    //{
        //    //    ex.printStackTrace();
        //    //}
        //    //catch (ClassNotFoundException ex)
        //    //{
        //    //    ex.printStackTrace();
        //    //}
        //    //object obj = Deserialize(bytes);
        //    //Object obj2 = obj as Object;

        //    //Debug.LogError(obj2);
        //}
        //else
        //{
        //    Debug.LogError(www.error);
        //}

        string p1 = "File://E:/GitProject/UGUIDesignerOnIPad/Assets/OriginalProjectJson/textureEncoding/texture.txt";
        WWW www = new WWW(p1);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            string encoding = www.text;
            encoding = encoding.Substring(encoding.IndexOf("/"));
            byte[] bytes = Convert.FromBase64String(encoding);
            Texture2D texture = new Texture2D(4, 4);
            texture.LoadImage(bytes);
            transform.GetComponent<Renderer>().material.mainTexture = texture;
        }
        else
        {
            Debug.LogError(www.error);
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //int count = Physics.RaycastNonAlloc(ray, hits);
            //list.Clear();
            //for (int i = 0; i < count; i++)
            //{
            //    list.Add(hits[i].collider.gameObject);
            //}
        }
    }

    ///<summary> 
    /// 序列化 
    /// </summary> 
    /// <param name="data">要序列化的对象</param> 
    /// <returns>返回存放序列化后的数据缓冲区</returns> 
    public static byte[] Serialize(object data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream rems = new MemoryStream();
        formatter.Serialize(rems, data);
        return rems.GetBuffer();
    }

    /// <summary> 
    /// 反序列化 
    /// </summary> 
    /// <param name="data">数据缓冲区</param> 
    /// <returns>对象</returns> 
    public static object Deserialize(byte[] data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream rems = new MemoryStream(data);
        //data = null;
        return formatter.Deserialize(rems);
    }

}
