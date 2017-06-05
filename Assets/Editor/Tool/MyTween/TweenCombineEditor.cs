using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenCombine))]
public class TweenCombineEditor : Editor {

	public override void OnInspectorGUI ()
	{
        MyEditorTool.SetLabelWidth(120f);
        //base.OnInspectorGUI ();
        TweenCombine mtarget = target as TweenCombine;

		mtarget.usePosition = EditorGUILayout.ToggleLeft ("使用位移", mtarget.usePosition);

		if (mtarget.usePosition)
		{
            EditorGUI.indentLevel++;
			mtarget.worldSpace = EditorGUILayout.Toggle("是否为世界坐标", mtarget.worldSpace);
			mtarget.fromPosition = EditorGUILayout.Vector3Field("起始位置", mtarget.fromPosition);
			mtarget.toPosition = EditorGUILayout.Vector3Field("结束位置", mtarget.toPosition);
            mtarget.animationCurve_Position = EditorGUILayout.CurveField("Animation Curve", mtarget.animationCurve_Position, GUILayout.Width(170f), GUILayout.Height(62f));
			mtarget.usePositionType = (Style)EditorGUILayout.EnumPopup("循环类型", mtarget.usePositionType);
            mtarget.positionDeletime = EditorGUILayout.FloatField("延迟播放时间", mtarget.positionDeletime);
			mtarget.timeOfPositionOnce = EditorGUILayout.FloatField("单次时间", mtarget.timeOfPositionOnce);
            EditorGUI.indentLevel--;
		}

        mtarget.useScale = EditorGUILayout.ToggleLeft("使用大小", mtarget.useScale);
		if (mtarget.useScale)
		{
            EditorGUI.indentLevel++;
			mtarget.fromScale = EditorGUILayout.Vector3Field("起始大小", mtarget.fromScale);
			mtarget.toScale = EditorGUILayout.Vector3Field("结束大小", mtarget.toScale);
            mtarget.animationCurve_Scale = EditorGUILayout.CurveField("Animation Curve", mtarget.animationCurve_Scale, GUILayout.Width(170f), GUILayout.Height(62f));
            mtarget.useScaleType = (Style)EditorGUILayout.EnumPopup("循环类型", mtarget.useScaleType);
            mtarget.scaleDeletime = EditorGUILayout.FloatField("延迟播放时间", mtarget.scaleDeletime);
			mtarget.timeOfScaleOnce = EditorGUILayout.FloatField("单次时间", mtarget.timeOfScaleOnce);
            EditorGUI.indentLevel--;
		}

        mtarget.useRotat = EditorGUILayout.ToggleLeft("使用旋转", mtarget.useRotat);
		if (mtarget.useRotat)
		{
            EditorGUI.indentLevel++;
			mtarget.fromAngle = EditorGUILayout.Vector3Field("起始旋转角", mtarget.fromAngle);
			mtarget.toAngle = EditorGUILayout.Vector3Field("结束旋转角", mtarget.toAngle);
            mtarget.animationCurve_Rotation = EditorGUILayout.CurveField("Animation Curve", mtarget.animationCurve_Rotation, GUILayout.Width(170f), GUILayout.Height(62f));
            mtarget.useRotatType = (Style)EditorGUILayout.EnumPopup("循环类型", mtarget.useRotatType);
            mtarget.rotatDeletime = EditorGUILayout.FloatField("延迟播放时间", mtarget.rotatDeletime);
			mtarget.timeOfRotatOnce = EditorGUILayout.FloatField("单次时间", mtarget.timeOfRotatOnce);
            EditorGUI.indentLevel--;
		}

        mtarget.useColor = EditorGUILayout.ToggleLeft("使用颜色", mtarget.useColor);
		if (mtarget.useColor)
		{
            EditorGUI.indentLevel++;
            mtarget.type = (RenderType)EditorGUILayout.EnumPopup("类型", mtarget.type);
		    if (mtarget.type == RenderType.ShaderColor)
		    {
                mtarget.shaderColorName = EditorGUILayout.TextField("shaderProptyName", mtarget.shaderColorName);
            }
		    mtarget.fromColor = EditorGUILayout.ColorField("起始颜色", mtarget.fromColor);
			mtarget.toColor  = EditorGUILayout.ColorField("结束颜色", mtarget.toColor);
            mtarget.animationCurve_Color = EditorGUILayout.CurveField("Animation Curve", mtarget.animationCurve_Color, GUILayout.Width(170f), GUILayout.Height(62f));
            mtarget.useColorType = (Style)EditorGUILayout.EnumPopup("循环类型", mtarget.useColorType);
            mtarget.colorDeletime = EditorGUILayout.FloatField("延迟播放时间", mtarget.colorDeletime);
			mtarget.timeOfColorOnce = EditorGUILayout.FloatField("单次时间", mtarget.timeOfColorOnce);
            EditorGUI.indentLevel--;
		}

        EditorUtility.SetDirty(mtarget);
	}

}
