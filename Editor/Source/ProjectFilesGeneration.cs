namespace StyleCop.Analyzer
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Xml.Linq;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Customize the project file generation with Roslyn Analyzers and custom c# version.
	/// </summary>
	public class ProjectFilesGeneration : AssetPostprocessor
	{
		private const string DefaultDirectory = "Packages/Unity-StyleCop/RequiredPackages";
		private const string StyleCopAnalyzerDllName = "StyleCop.Analyzers.dll";
		private const string StyleCopAnalyzerCodeFixesDllName = "StyleCop.Analyzers.CodeFixes.dll";
		private const string StyleCopRulesetFileName = "UnityStyleCop.ruleset";
		private const string StyleCopJsonFileName = "stylecop.json";

		private static bool ValidateAnalyzerFolderIsSetup()
		{
			string currentDirectory = Directory.GetCurrentDirectory();
			string analyzerDirectory = Path.Combine(currentDirectory, DefaultDirectory);
			if (!Directory.Exists(analyzerDirectory))
			{
				return false;
			}

			IEnumerable<string> files = Directory.GetFiles(analyzerDirectory).Select(Path.GetFileName).ToArray();
			return files.Contains(StyleCopJsonFileName) &&
			       files.Contains(StyleCopAnalyzerDllName) &&
			       files.Contains(StyleCopAnalyzerCodeFixesDllName) &&
			       files.Contains(StyleCopRulesetFileName);
		}

		private static string OnGeneratedCSProject(string path, string contents)
		{
#if STYLECOP_DEBUG
			Debug.Log("*.csproj change detected. Ensuring it adds StyleCop to project.");
#endif
			XDocument xml = XDocument.Parse(contents);

			AddStyleCopRoslynAnalyzerToProjectFile(xml);

			using (var str = new Utf8StringWriter())
			{
				xml.Save(str);
				return str.ToString();
			}
		}

		private static void AddStyleCopRoslynAnalyzerToProjectFile(XDocument doc)
		{
			XElement projectContentElement = doc.Root;
			if (projectContentElement == null)
			{
				return;
			}

			XNamespace xmlns = projectContentElement.Name.NamespaceName; // do not use var
			AddStyleCopRoslynAnalyzer(projectContentElement, xmlns);
		}

		private static void AddStyleCopRoslynAnalyzer(
			XElement rootElement,
			XNamespace xmlNameSpace)
		{
			string currentDirectory = Directory.GetCurrentDirectory();
			if (!ValidateAnalyzerFolderIsSetup())
			{
#if STYLECOP_DEBUG
				Debug.Log("One of the StyleCop required files is missing!");
#endif
				return;
			}

			var requiredFilesDirectoryInfo = new DirectoryInfo(Path.Combine(currentDirectory, DefaultDirectory));
			FileInfo[] files = requiredFilesDirectoryInfo.GetFiles();
			var itemGroup = new XElement(xmlNameSpace + "ItemGroup");
			foreach (FileInfo file in files)
			{
				string extension = file.Extension;
				switch (extension)
				{
					case ".dll":
					{
						var reference = new XElement(xmlNameSpace + "Analyzer");
						reference.Add(new XAttribute("Include", file));
						itemGroup.Add(reference);
						break;
					}

					case ".json":
					{
						var reference = new XElement(xmlNameSpace + "AdditionalFiles");
						reference.Add(new XAttribute("Include", file));
						itemGroup.Add(reference);
						break;
					}

					case ".ruleset":
						SetOrUpdateProperty(
							rootElement,
							xmlNameSpace,
							"CodeAnalysisRuleSet",
							existing => file.FullName);
						break;
				}
			}

			rootElement.Add(itemGroup);
		}

		private static void SetOrUpdateProperty(
			XContainer root,
			XNamespace xmlns,
			string name,
			Func<string, string> updater)
		{
			XElement element = root.Elements(xmlns + "PropertyGroup").Elements(xmlns + name).FirstOrDefault();
			if (element != null)
			{
				string result = updater(element.Value);
				if (result != element.Value)
				{
					element.SetValue(result);
				}
			}
			else
			{
				AddProperty(root, xmlns, name, updater(string.Empty));
			}
		}

		private static void AddProperty(
			XContainer root,
			XNamespace xmlns,
			string name,
			string content)
		{
			XElement propertyGroup = root.Elements(xmlns + "PropertyGroup")
				.FirstOrDefault(e => !e.Attributes(xmlns + "Condition").Any());
			if (propertyGroup == null)
			{
				propertyGroup = new XElement(xmlns + "PropertyGroup");
				root.AddFirst(propertyGroup);
			}

			propertyGroup.Add(new XElement(xmlns + name, content));
		}

		private class Utf8StringWriter : StringWriter
		{
			public override Encoding Encoding
			{
				get { return Encoding.UTF8; }
			}
		}
	}
}
