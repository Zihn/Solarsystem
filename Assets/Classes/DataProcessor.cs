using UnityEngine;
using System;

public static class DataProcessor
{
	public static SolarSystemData LoadSolarSystem(string filename)
	{
		string json = ReadFromResource(filename);
		SolarSystemData solarSystem = JsonUtility.FromJson<SolarSystemData>(json);
		return solarSystem;
	}


	private static string ReadFromResource(string filename)
	{
		string filePath = "Data/" + filename.Replace(".json", "");
		TextAsset targetFile = UnityEngine.Resources.Load<TextAsset>(filePath);
		return targetFile.text;
	}

}