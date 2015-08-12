using UnityEngine;
using System;

public class ColorLibrary : ScriptableObject
{
	public ColorSelection[] Colors = new ColorSelection[3];
	public Color[] DefenseLevels = new Color[4];

	public Color DefenseColor { get { return DefenseLevels[0]; } }
}

[Serializable]
public class ColorSelection
{
	public Color IdleColor;
	public Color[] AttackLevels = new Color[4];
	public Color AttackColor { get { return AttackLevels[0]; } }
}
