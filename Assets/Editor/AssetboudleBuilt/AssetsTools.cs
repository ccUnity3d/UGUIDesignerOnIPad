using foundation;
using System.IO;
using System.Xml;
using UnityEditor;

namespace foundationEditor
{
    //[InitializeOnLoad]
    public class AssetsTools
    {
        public static bool enabledAutoAMF = true;
        static AssetsTools()
        {
            //load();
        }
        //public static void load()
        //{
        //    string path = Application.dataPath + "/Editor/config.xml";

        //    if (File.Exists(path))
        //    {
        //        XmlDocument doc = new XmlDocument();
        //        doc.Load(path);

        //        XmlNode node = doc.SelectSingleNode("config");
        //        ConfigurationUtil.initConfigXML(node.ChildNodes);

        //        string v=ConfigurationUtil.getPrefix("enabledAutoAMF");
        //        if (string.IsNullOrEmpty(v)==false)
        //        {
        //            enabledAutoAMF = ConfigurationUtil.getPrefix("enabledAutoAMF") == "1";
        //        }
        //    }
        //    else
        //    {
        //        EditorUtility.DisplayDialog("提示", "配置不存在", "ok");
        //    }
        //}

        [MenuItem("Tools/PackageAssetboundle")]
        public static void ExportSelectionToAssetboundle()
        {
            ScriptableWizard.DisplayWizard("选择发布资源的平台类型", typeof(BuildPrefabWindow), "确定");
        }

        [MenuItem("Assets/DeleteAsset", false, 0)]
        public static void DeleteAsset()
        {
            UnityEngine.Object go = Selection.activeObject;
            //string path = AssetDatabase.GetAssetPath(go);
            UnityEngine.Object .DestroyImmediate(go, true);
            AssetDatabase.SaveAssets();
        }

        
    }
}