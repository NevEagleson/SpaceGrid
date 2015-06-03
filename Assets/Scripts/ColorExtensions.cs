using UnityEngine;
using System.Collections;

public static class ColorExtensions
{
	public static void SetRGB(this Color col, Color value)
	{
		col.r = value.r;
		col.g = value.g;
		col.b = value.b;
	}

	public static void SetAlpha(this Color col, Color value)
	{
		col.a = value.a;
	}

	public static void SetAlpha(this Color col, float value)
	{
		col.a = value;
	}
}
