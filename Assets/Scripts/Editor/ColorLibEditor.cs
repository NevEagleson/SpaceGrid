using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColorLibrary))]
public class ColorLibEditor : Editor
{
	private ColorLibrary Library { get { return target as ColorLibrary; } }

	public override void OnInspectorGUI()
	{
		for (int i = 0; i < 3; ++i)
		{
			EditorGUILayout.LabelField("Color " + i);
			++EditorGUI.indentLevel;

			int beginIndent = EditorGUI.indentLevel;
			Library.Colors[i].NormalColor = EditorGUILayout.ColorField("Normal", Library.Colors[i].NormalColor);

			EditorGUILayout.BeginHorizontal();
			Library.Colors[i].AttackLevels[1] = EditorGUILayout.ColorField("Attack", Library.Colors[i].AttackLevels[1]);
			EditorGUI.indentLevel = 0;
			Library.Colors[i].AttackLevels[2] = EditorGUILayout.ColorField(Library.Colors[i].AttackLevels[2]);
			Library.Colors[i].AttackLevels[3] = EditorGUILayout.ColorField(Library.Colors[i].AttackLevels[3]);
			EditorGUI.indentLevel = beginIndent;
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			Library.Colors[i].DefneseLevels[1] = EditorGUILayout.ColorField("Defense", Library.Colors[i].DefneseLevels[1]);
			EditorGUI.indentLevel = 0;
			Library.Colors[i].DefneseLevels[2] = EditorGUILayout.ColorField(Library.Colors[i].DefneseLevels[2]);
			Library.Colors[i].DefneseLevels[3] = EditorGUILayout.ColorField(Library.Colors[i].DefneseLevels[3]);
			EditorGUI.indentLevel = beginIndent;
			EditorGUILayout.EndHorizontal();

			--EditorGUI.indentLevel;
		}
	}
}