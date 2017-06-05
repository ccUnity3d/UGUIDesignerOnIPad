using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChangeFont))]
public class ChangeFontEditor : Editor{
    ChangeFont mtarget;
    public override void OnInspectorGUI()
    {
        mtarget = target as ChangeFont;
        mtarget.font = EditorGUILayout.ObjectField("字体：", mtarget.font, typeof(Font), false, null) as Font;

        if (GUILayout.Button("切换字体集"))
        {
            //TODO:
            //UILabel[] uilabels = mtarget.transform.GetComponentsInChildren<UILabel>(true);
            //for (int i = 0; i < uilabels.Length; i++)
            //{
            //    uilabels[i].trueTypeFont = mtarget.font;
            //}
            Debug.Log("切换完成");
        }
    }
}
