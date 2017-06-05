using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MyTweenRotation))]
public class MyTweenRotationEditor : MyTweenEditor
{

    public override void OnInspectorGUI()
    {
        GUILayout.Space(6f);
        EditorGUIUtility.labelWidth = 120f;

        MyTweenRotation tw = target as MyTweenRotation;

        GUI.changed = false;

        Vector3 from = EditorGUILayout.Vector3Field("From", tw.from);
        Vector3 to = EditorGUILayout.Vector3Field("To", tw.to);
        
        bool worldSpace = EditorGUILayout.Toggle("WorldSpace", tw.worldSpace);

        if (GUI.changed)
        {
            MyEditorTool.RegisterUndo("Tween Change", tw);
            tw.from = from;
            tw.to = to;
            tw.worldSpace = worldSpace;
            MyEditorTool.SetDirty(tw);
        }

        DrawCommonProperties();

    }
}

