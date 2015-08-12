using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColorLibrary))]
public class ColorLibEditor : Editor
{
	private ColorLibrary Library { get { return target as ColorLibrary; } }

	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.LabelField("Defense");
		++EditorGUI.indentLevel;
		Library.DefenseLevels[0] = EditorGUILayout.ColorField("Defense", Library.DefenseLevels[0]);

		EditorGUILayout.BeginHorizontal();
		int beginIndent = EditorGUI.indentLevel;
		Library.DefenseLevels[1] = EditorGUILayout.ColorField("Defense Levels", Library.DefenseLevels[1]);
		EditorGUI.indentLevel = 0;
		Library.DefenseLevels[2] = EditorGUILayout.ColorField(Library.DefenseLevels[2]);
		Library.DefenseLevels[3] = EditorGUILayout.ColorField(Library.DefenseLevels[3]);
		EditorGUI.indentLevel = beginIndent;
		EditorGUILayout.EndHorizontal();

		--EditorGUI.indentLevel;

		for (int i = 0; i < 3; ++i)
		{
			EditorGUILayout.LabelField("Color " + i);

			++EditorGUI.indentLevel;
			beginIndent = EditorGUI.indentLevel;

			Library.Colors[i].IdleColor = EditorGUILayout.ColorField("Idle", Library.Colors[i].IdleColor);
			Library.Colors[i].AttackLevels[0] = EditorGUILayout.ColorField("Attack", Library.Colors[i].AttackLevels[0]);

			EditorGUILayout.BeginHorizontal();
			Library.Colors[i].AttackLevels[1] = EditorGUILayout.ColorField("Attack Levels", Library.Colors[i].AttackLevels[1]);
			EditorGUI.indentLevel = 0;
			Library.Colors[i].AttackLevels[2] = EditorGUILayout.ColorField(Library.Colors[i].AttackLevels[2]);
			Library.Colors[i].AttackLevels[3] = EditorGUILayout.ColorField(Library.Colors[i].AttackLevels[3]);
			EditorGUI.indentLevel = beginIndent;
			EditorGUILayout.EndHorizontal();

			--EditorGUI.indentLevel;
		}

		if(EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetDirty(Library);
		}
	}
}