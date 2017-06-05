using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
public class UITestJson : MonoBehaviour {

    // Use this for initialization
    

    void Start () {
        //  string str=  MyJsonTool.ToJson(SchemeManifest.Instance);
        //  using (StreamWriter stream = new StreamWriter(Application.streamingAssetsPath+"/ObjectToJson.txt",true,System.Text.Encoding.UTF8) )
        //  {
        //      stream.WriteLine(str);
        //  }
        //Debug.Log(str);
        string Path = Application.streamingAssetsPath + "/ObjectToJson.txt";
        string str = "";
        using (StreamReader stream = new StreamReader(File.Open(Path,FileMode.Open),System.Text.Encoding.UTF8))
        {
            str = stream.ReadToEnd();
        }

        object obj = MyJsonTool.FromJson(str);
        //SchemeManifest.Instance.Deserialize(obj as Dictionary<string ,object>);

        //Debug.Log("schemeName : = " + SchemeManifest.Instance.name);
        //Debug.Log("guid : = " + SchemeManifest.Instance.guid );
    }
	
	// Update is called once per frame
	void Update () {
	
	}

   
}
