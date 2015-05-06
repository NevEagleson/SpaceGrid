using UnityEngine;
using System;

public class ColorLibrary : ScriptableObject
{
	public ColorSelection[] Colors = new ColorSelection[3];
}

[Serializable]
public class ColorSelection
{
	public Color NormalColor;
	public Color[] AttackLevels = new Color[4];
	public Color[] DefneseLevels = new Color[4];
}
