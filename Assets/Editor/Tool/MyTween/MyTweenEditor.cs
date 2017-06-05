using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MyTween))]
public class MyTweenEditor : Editor
{
    public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		EditorGUIUtility.labelWidth = 110f;
        //base.OnInspectorGUI();
		DrawCommonProperties();
	}

    protected void DrawCommonProperties()
    {
        MyTween tw = target as MyTween;

        if (MyEditorTool.DrawHeader("Tweener"))
        {
            MyEditorTool.BeginContents();
            EditorGUIUtility.labelWidth = 110f;

            GUI.changed = false;

            AnimationCurve curve = EditorGUILayout.CurveField("Animation Curve", tw.curve, GUILayout.Width(170f), GUILayout.Height(62f));
            Style style = (Style)EditorGUILayout.EnumPopup("Play Style", tw.style);
            MoveType moveType = (MoveType)EditorGUILayout.EnumPopup("Play MoveType", tw.moveType);

            GUILayout.BeginHorizontal();
            float dur = EditorGUILayout.FloatField("Duration", tw.duration, GUILayout.Width(170f));
            GUILayout.Label("seconds");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float del = EditorGUILayout.FloatField("Start Delay", tw.delay, GUILayout.Width(170f));
            GUILayout.Label("seconds");
            GUILayout.EndHorizontal();

            bool ts = EditorGUILayout.Toggle("Ignore TimeScale", tw.ignoreTimeScale);

            if (GUI.changed)
            {
                MyEditorTool.RegisterUndo("Tween Change", tw);
                tw.curve = curve;
                tw.moveType = moveType;
                tw.style = style;
                tw.ignoreTimeScale = ts;
                tw.duration = dur;
                tw.delay = del;
                MyEditorTool.SetDirty(tw);
            }
            MyEditorTool.EndContents();
        }

    }

}

