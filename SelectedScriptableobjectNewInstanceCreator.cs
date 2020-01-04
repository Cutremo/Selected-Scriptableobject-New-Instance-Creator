using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class SelectedScriptableobjectNewInstanceCreator : MonoBehaviour
{
	[MenuItem("Assets/Create/Selected Scriptableobject New Instance", priority = 1)]
	private static void CreateAsset()
	{
		string filePath = AssetDatabase.GetAssetPath(Selection.activeObject);

		string fileText = File.ReadAllText(filePath);

		Type fileType = GetScriptType(fileText);

		ScriptableObject instance = ScriptableObject.CreateInstance(fileType);

		string directoryPath = Path.GetDirectoryName(filePath);
		string assetPath = Path.Combine(directoryPath, fileType.Name + ".asset");

		for (int a = 1; !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(assetPath)); a++)
		{
			assetPath = Path.Combine(directoryPath, fileType.Name + " (" + a + ")" + ".asset");
		}
		filePath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

		AssetDatabase.CreateAsset(instance, assetPath);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	[MenuItem("Assets/Create/Selected Scriptableobject New Instance", validate = true)]
	private static bool IsSelectedObjectAnScriptableObject()
	{
		string filePath = AssetDatabase.GetAssetPath(Selection.activeObject);

		Debug.Log(filePath);
		if (!File.Exists(filePath))
		{
			return false;
		}

		string fileText = File.ReadAllText(filePath);

		Type fileType = GetScriptType(fileText);

		return fileType != null && fileType.IsSubclassOf(typeof(ScriptableObject)) && !fileType.IsAbstract;
	}

	private static Type GetScriptType(string fileText)
	{
		var delimiters = new string[] { " " };
		var words = fileText.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

		for (int a = 0; a < words.Length; a++)
		{
			if (words[a] == "class")
			{
				Assembly asm = typeof(Conversation).Assembly;
				return asm.GetType(words[a + 1]);
			}
		}
		return null;
	}
}