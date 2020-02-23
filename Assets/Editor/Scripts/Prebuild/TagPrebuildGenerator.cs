using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor.Callbacks;

public static class TagPrebuildGenerator
{
	[DidReloadScripts]
	public static void Recompile()
	{
		Dictionary<string, Type> tagTypes = new Dictionary<string, Type>();
		
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		foreach (Type type in assembly.GetTypes())
		{
			TagAttribute attribute = type.GetCustomAttribute<TagAttribute>();
			
			if (attribute == null || attribute.Tags == null)
				continue;
			
			foreach (string tag in attribute.Tags)
				tagTypes[tag] = type;
		}
		
		StringBuilder tagPrebuildBuilder = new StringBuilder();
		
		tagPrebuildBuilder.AppendLine("using System;");
		tagPrebuildBuilder.AppendLine("using System.Collections.Generic;");
		
		tagPrebuildBuilder.AppendLine();
		
		tagPrebuildBuilder.AppendLine("public static class TagPrebuild");
		tagPrebuildBuilder.AppendLine("{");
		tagPrebuildBuilder.AppendLine("\tstatic readonly Dictionary<string, Type> m_TagTypes = new Dictionary<string, Type>()");
		tagPrebuildBuilder.AppendLine("\t{");
		
		foreach (var entry in tagTypes)
		{
			tagPrebuildBuilder.AppendFormat("\t\t{{ \"{0}\", typeof({1}) }},", entry.Key, entry.Value.Name).AppendLine();
		}
		
		tagPrebuildBuilder.AppendLine("\t};");
		tagPrebuildBuilder.AppendLine();
		
		tagPrebuildBuilder.AppendLine("\tpublic static Type GetTagType(string _Tag)");
		tagPrebuildBuilder.AppendLine("\t{");
		tagPrebuildBuilder.AppendLine("\t\treturn m_TagTypes.ContainsKey(_Tag) ? m_TagTypes[_Tag] : null;");
		tagPrebuildBuilder.AppendLine("\t}");
		
		tagPrebuildBuilder.AppendLine("}");
		
		const string directory = "Assets/Scripts/Prebuild/";
		const string filename  = "TagPrebuild.cs";
		
		if (!Directory.Exists(directory))
			Directory.CreateDirectory(directory);
		
		File.WriteAllText(directory + filename, tagPrebuildBuilder.ToString());
	}
}
