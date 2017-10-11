using System.Collections.Generic;
using UnityEngine;

public static class Scaling
{
	public static float DistanceScale = 7e-6f;
	public static float MoonDistanceScale = 1e-5f;
	public static bool UseLogScale = false;
	public static float LogBase = 1f; 
	public static float LogBaseAxisLength = 10f;
	public static float PlanetSizeScale = 1e-3f;
	public static float MoonToPlanetRatio = 0.5f;
	public static float SunSizeScale = 1e-3f;
	public static float SpeedUpFactor = 1.752e6f; 

}

[System.Serializable]
public class SolarSystemData
{
	public string Name; 
	public float Age; 
	public string Location;
	public SunData[] Suns;
	public PlanetData[] Planets;
}
[System.Serializable]
public class CelestialBodyData
{
	public string Name;
	public string Type; 
	public Vector2 Position; 
	public float Radius; 
	public float Mass; 
	public string Color;
}
[System.Serializable]
public class SunData : CelestialBodyData 
{
	public float Age;
//	public PlanetData[] Planets ;
}
[System.Serializable]
public class PlanetData : CelestialBodyData
{
	public float Apoapsis;
	public float Periapsis;
	public int OrbitingIndex;
	public MoonData[] Moons;
}
[System.Serializable]
public class MoonData : CelestialBodyData
{	
	public float Apoapsis;
	public float Periapsis;
	public bool Approachable;
}

public class Element
{
	ElementName name;
	float percentage;

	class ElementName
	{
		// TODO insert -> bunch of elements here
		// see photospheric tab composition: https://en.wikipedia.org/wiki/Sun
	}
}

public class Resources
{
	Material[] materials;
	float totalMass;
}


public class Material
{
	Material.Names name;
	Material.Types type;
	float mass;

	public Material.Names Name { get { return name; } } 
	public Material.Types Type { get { return type; } }
	public float Mass { get { return mass; } }

	public class Names
	{
		//TODO insert -> a bunch of material names
	}

	public class Types
	{
		//TODO insert -> a bunch of material types
	}
}

public class Difficulty
{
	public static int VeryEasy { get { return 1; } } 
	public static int Easy { get { return 2; } } 
	public static int Medium { get { return 3; } }
	public static int Hard { get { return 4; } }
	public static int VeryHard { get { return 5; } }
}