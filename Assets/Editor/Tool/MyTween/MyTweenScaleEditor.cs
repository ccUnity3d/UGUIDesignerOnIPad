using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MyTweenScale))]
public class MyTweenScaleEditor : MyTweenEditor{

    public override void OnInspectorGUI()
    {
        GUILayout.Space(6f);
        EditorGUIUtility.labelWidth = 120f;

        MyTweenScale tw = target as MyTweenScale;

        GUI.changed = false;

        Vector3 from = EditorGUILayout.Vector3Field("From", tw.from);
        Vector3 to = EditorGUILayout.Vector3Field("To", tw.to);

        if (GUI.changed)
        {
            MyEditorTool.RegisterUndo("Tween Change", tw);
            tw.from = from;
            tw.to = to;
            MyEditorTool.SetDirty(tw);
        }

        DrawCommonProperties();

    }
}
