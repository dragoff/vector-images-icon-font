using System.IO;
using UnityEngine;
using UnityEditor;

namespace VectorIconImages
{
	public class IconDataManipulator : Editor
	{
		[MenuItem("Tools/Vector Icons/Open icomoon.io",false,0)]
		public static void Link1() => Application.OpenURL("https://icomoon.io/app/#/select");

		[MenuItem("Tools/Vector Icons/Load font with codepoints",false,16)]
		public static void OpenCodepointsFile()
		{
			string file = EditorUtility.OpenFilePanel("Select codepoint File", Application.dataPath, "");
			if(!string.IsNullOrEmpty(file))
				CreateIconDataset(file);
		}
		public static void CreateIconDataset(string codepointsFile)
		{
			
			//PARSE
			string[] codepoints = File.ReadAllLines(codepointsFile);
			string scriptContent = "";

			string[] enumContents = new string[codepoints.Length];
			string[] unicodeContents = new string[codepoints.Length];

			int index = 0;
			foreach (string s in codepoints)
			{
				string[] e = s.Split(' ');
				enumContents[index] = e[0];
				unicodeContents[index] = e[1];
				index++;
			}
			//CODE CREATING
			scriptContent += "namespace VectorIconImages\n{";

			scriptContent += "\n\tpublic readonly struct Data";
			scriptContent += "\n\t{";
			scriptContent += "\n\t\tpublic static System.Collections.Generic.Dictionary<string, string> IconData = new System.Collections.Generic.Dictionary<string, string>{";
			for (int i = 0; i < codepoints.Length; i++)
			{
				scriptContent += "\n\t\t\t{\""+enumContents[i]+"\", \"" + unicodeContents[i] + "\"},";
			}
			scriptContent += "\n\t\t};";

			scriptContent += "\n\t}";

			scriptContent += "}";

			System.IO.File.WriteAllText(Application.dataPath+ "/VectorIconImages/Scripts/IconDataset.cs", scriptContent);
			
			var file =  EditorUtility.OpenFilePanel("Select your font", Application.dataPath, "ttf");
			if(!string.IsNullOrEmpty(file))
				File.Copy(file,Application.dataPath+ $"/VectorIconImages/Font/{Definitions.FontName}.ttf",true);
			AssetDatabase.Refresh();
		}
	}
}
