using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MyTweenColor))]
public class MyTweenColorEditor : MyTweenEditor {

    public override void OnInspectorGUI()
    {
        GUILayout.Space(6f);
        EditorGUIUtility.labelWidth = 120f;

        MyTweenColor tw = target as MyTweenColor;

        GUI.changed = false;

        Color from = EditorGUILayout.ColorField("From", tw.from);
        Color to = EditorGUILayout.ColorField("To", tw.to);
        RenderType renderType = (RenderType)EditorGUILayout.EnumPopup("RenderType", tw.renderType);
        if (renderType == RenderType.ShaderColor)
        {
            tw.shaderColorName = EditorGUILayout.TextField("RenderType", tw.shaderColorName);
        }
        if (GUI.changed)
        {
            MyEditorTool.RegisterUndo("Tween Change", tw);
            tw.from = from;
            tw.to = to;
            tw.renderType = renderType;
            MyEditorTool.SetDirty(tw);
        }

        DrawCommonProperties();

    }
}
