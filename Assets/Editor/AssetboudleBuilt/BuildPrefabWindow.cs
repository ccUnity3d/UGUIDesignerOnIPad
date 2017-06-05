using foundationExport;
using UnityEditor;
using UnityEngine;
using System.IO;
using foundation;
using System.Collections.Generic;

namespace foundationEditor
{
    public class BuildPrefabWindow : ScriptableWizard
    {
        private string[] fromList;
        public string exportPrefabFromPath;
        public string exportPrefabToPath;
        /// <summary>
        /// 
        /// </summary>
        public string exprotExtentions;
        public BuildTarget buildTarget = BuildTarget.WebPlayer;
     
        public BuildPrefabWindow()
        {
            this.titleContent = new GUIContent("发布资源设置");
            string froms = ConfigurationUtil.getPrefix("exportPrefabFromPath");

            fromList = froms.Split(',');
            if (fromList.Length == 1)
            {
                exportPrefabFromPath = froms;
            }
            exportPrefabToPath = ConfigurationUtil.getPrefix("exportPrefabToPath");

            exprotExtentions = ConfigurationUtil.getPrefix("exprotExtentions").ToLower();

            if (SystemInfo.operatingSystem.IndexOf("Mac") != -1)
            {
                buildTarget = BuildTarget.iOS;
            }
            else
            {
                buildTarget = EditorUserBuildSettings.activeBuildTarget;
            }
        }

        private int selectedIndex = 0;

        protected override bool DrawWizardGUI()
        {
            if (fromList.Length > 1)
            {
                string[] froms = new string[fromList.Length];
                for (int i = 0; i < fromList.Length; i++)
                {
                    froms[i] = fromList[i].Replace('/', '.');
                }
                selectedIndex = EditorGUILayout.Popup("选择一项", selectedIndex, froms);

                exportPrefabFromPath = fromList[selectedIndex];
            }
            return base.DrawWizardGUI();
        }

        protected virtual void OnWizardCreate()
        {
            if (Directory.Exists(exportPrefabFromPath) == false)
            {
                EditorUtility.DisplayDialog("导出", "请配置Prefab from目录", "确定");
                return;
            }
            if (Directory.Exists(exportPrefabToPath) == false)
            {
                EditorUtility.DisplayDialog("导出", "请配置Prefab to目录", "确定");
                return;
            }

            if (switchToPlatform(buildTarget))
            {
                startProgress();
            }
        }

        protected void startProgress()
        {
            PrefabExport prefabExport;
            prefabExport = new PrefabExport(exportPrefabFromPath, exportPrefabToPath);

            List<BuildTarget> buildTargets = new List<BuildTarget>();
            buildTargets.Add(buildTarget);

            List<string> exNameArr = new List<string>();
            exNameArr.Add("*.prefab");

            if (string.IsNullOrEmpty(exprotExtentions) == false)
            { 
                string[] temp=exprotExtentions.Split(',');
                string key;
                foreach (string s in temp)
                {
                    key = "*." + s;
                    if (exNameArr.Contains(key) == false)
                    {
                        exNameArr.Add(key);
                    }
                }
            }

            prefabExport.exportAllPrefab(buildTargets, exNameArr);
        }

        protected bool switchToPlatform(BuildTarget targetPlatform)
        {
            if (EditorUserBuildSettings.activeBuildTarget != targetPlatform)
            {
                Debug.Log("自动切换至 " + targetPlatform + " 平台.");

                EditorUserBuildSettings.activeBuildTargetChanged += activeBuildTargetChanged;
                EditorUserBuildSettings.SwitchActiveBuildTarget(targetPlatform);
                return false;
            }
            return true;
        }

        private void activeBuildTargetChanged()
        {
            EditorUserBuildSettings.activeBuildTargetChanged -= activeBuildTargetChanged;
            startProgress();
        }
    }
}
