using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using Assembly = System.Reflection.Assembly;
///<summary>
///Repository link: https://github.com/Culoextremo/Selected-Scriptableobject-New-Instance-Creator

///# Selected-Scriptableobject-New-Instance-Creator
///Allows creating a new instance of a selected ScriptableObject in Unity without having to create a new asset menu.

///</summary>

///<remarks>
///# How to use
///1-Place "SelectedScriptableobjectNewInstanceCreator.cs" file in your project's Editor folder.

///2-Select a file that contains a non-abstract class which is a subclass of ScriptableObject.

///3-On the asset menu, click Create -> Selected Scriptable Object New Instance.
///</remarks>

namespace Cutremo
{
	public static class SelectedScriptableobjectNewInstanceCreator 
	{
		[MenuItem("Assets/Create/Selected Scriptableobject New Instance", priority = 1)]
		private static void CreateAsset()
		{
			string filePath = AssetDatabase.GetAssetPath(Selection.activeObject);

			string fileText = File.ReadAllText(filePath);

			Type fileType = GetScriptType(fileText, filePath);

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

			if (!File.Exists(filePath))
			{
				return false;
			}

			string fileText = File.ReadAllText(filePath);

			Type fileType = GetScriptType(fileText, filePath);

			return fileType != null && fileType.IsSubclassOf(typeof(ScriptableObject)) && !fileType.IsAbstract;
		}
		private static Type GetScriptType(string fileText, string path)
		{
			var delimiters = new string[] { " " };
			var words = fileText.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

			for (int a = 0; a < words.Length; a++)
			{
				if (words[a] == "class")
				{
					string fullAssemblyName = CompilationPipeline.GetAssemblyNameFromScriptPath(path);
					string assemblyName = Path.GetFileNameWithoutExtension(fullAssemblyName);

					Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(x => x.GetName().Name == assemblyName);

					Type returnType = null;

					returnType = assembly.GetType(words[a + 1]);

					if (returnType != null)
					{
						return returnType;
					}
				}
			}
			return null;
		}
	}
}
