using UnityEngine;
using UnityEditor;
using System.IO;

public static class AssetMenu 
{
	private static string CurrentFolderPath
	{
		get
		{
			string path = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (string.IsNullOrEmpty(path))
				path = "Assets";
			else if (!string.IsNullOrEmpty(Path.GetExtension(path)))
				path = path.Replace(Path.GetFileName(path), "");
			return path;
		}
	}

	[MenuItem("Assets/Create/ColorLibrary")]
	public static void CreateColorLibrary()
	{
		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ColorLibrary>(), AssetDatabase.GenerateUniqueAssetPath(CurrentFolderPath + "/New ColorLib.asset"));
	}

	[MenuItem("Assets/Create/ShipDefinition")]
	public static void CreateShipDefinition()
	{
		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ShipDefinition>(), AssetDatabase.GenerateUniqueAssetPath(CurrentFolderPath + "/New Ship.asset"));
	}
}
