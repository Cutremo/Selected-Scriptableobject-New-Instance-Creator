using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Cutremo
{
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

	public class SelectedScriptableobjectNewInstanceCreator : MonoBehaviour
	{
		private static readonly string[] assemblyNames = new string[] { "Assembly-CSharp", "Assembly-CSharp-Editor" };

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

			if (!File.Exists(filePath))
			{
				return false;
			}
			UnityEngine.Debug.Log("== Player Assemblies ==");
			UnityEditor.Compilation.Assembly[] playerAssemblies =
				UnityEditor.Compilation.CompilationPipeline.GetAssemblies(UnityEditor.Compilation.AssembliesType.Player);

			foreach (var assembly in playerAssemblies)
			{
				UnityEngine.Debug.Log(assembly.name);
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
					Assembly[] assemblies = Array.FindAll(AppDomain.CurrentDomain.GetAssemblies(), assembly => assemblyNames.Contains(assembly.GetName().Name));

					Type returnType = null;

					for (int b = 0; b < assemblies.Length; b++)
					{
						returnType = assemblies[b].GetType(words[a + 1]);

						if (returnType != null)
						{
							break;
						}
					}
					return returnType;
				}
			}
			return null;
		}
	}
}
