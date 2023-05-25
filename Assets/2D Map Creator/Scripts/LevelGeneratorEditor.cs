using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Xml;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor 
{

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		LevelGenerator myTarget = (LevelGenerator)target;

		if (GUILayout.Button("Save")) {
			if (myTarget.mappingfile == "")
				myTarget.mappingfile = "mappings.xml";
			SaveXml(myTarget.mappingfile, myTarget.colorMappings);
		}
		if (GUILayout.Button("Load")) {
			if (myTarget.mappingfile == "")
				myTarget.mappingfile = "mappings.xml";
			myTarget.colorMappings = LoadXml(myTarget.mappingfile);
		}
	}

	public void SaveXml (string filename, ColorSpawn[] css) {
		string appdata = Application.dataPath;
		string filePath = appdata + "/" + filename;

		XmlDocument xml = new XmlDocument();
		xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", ""));

		XmlNode root = xml.CreateElement("ColorMappings");
		xml.AppendChild(root);

		foreach (ColorSpawn cs in css) {
			XmlNode element = xml.CreateElement("element");

			XmlAttribute color = xml.CreateAttribute("color");
			color.Value = "#" + ColorUtility.ToHtmlStringRGB(cs.color) + "FF";
			element.Attributes.Append(color);

			Color newColor = Color.white;
			if (ColorUtility.TryParseHtmlString(color.Value, out newColor)) {
				cs.color = newColor;
			}

			XmlAttribute prefab = xml.CreateAttribute("prefab");
			prefab.Value = AssetDatabase.GetAssetPath(cs.prefab);
			element.Attributes.Append(prefab);

			root.AppendChild(element);
		}

		if (!System.IO.File.Exists(filePath))
			System.IO.Directory.GetParent(filePath).Create();
		xml.Save(filePath);

	}

	public ColorSpawn[] LoadXml (string filename) {
		string appdata = Application.dataPath;
		string filePath = appdata + "/" + filename;
		XmlDocument xml = new XmlDocument();
		ColorSpawn[] result = {};
		try {
			xml.Load(filePath);
			XmlNode elements = xml.SelectSingleNode("ColorMappings");
			int idx = 0;
			result = new ColorSpawn[elements.SelectNodes("element").Count];
			foreach (XmlNode element in elements.SelectNodes("element")) {
				Color newColor = Color.white;
				result[idx] = new ColorSpawn();
				if (ColorUtility.TryParseHtmlString(element.Attributes.GetNamedItem("color").Value, out newColor)) {
					result[idx].color = newColor;
				} else {
					Debug.LogWarning("Error while color converting");
				}
				GameObject ob = AssetDatabase.LoadAssetAtPath<GameObject>(element.Attributes.GetNamedItem("prefab").Value);
				if (ob) {
					result[idx].prefab = ob;
				} else {
					Debug.LogWarning("Error while loading prefab");
				}
				idx ++;
			}
		} catch (System.Exception e) {
			Debug.LogWarning("Error while loading mapping xml : " + e.Message);
		}
		return result;
	}
}