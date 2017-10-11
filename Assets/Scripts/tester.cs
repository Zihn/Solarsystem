using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tester : MonoBehaviour 
{
	SolarSystem solarSystem;

	// Scaling
	[Header("Scaling")]
	public float DistanceScale = 7e-6f;
	public float MoonDistanceScale = 1e-5f;
	public bool UseLogScale = false;
	public float LogBaseAxisLength = 300f;
	public float PlanetSizeScale = 1e-3f;
	public float MoonToPlanetRatio = 0.5f;
	public float SunSizeScale = 1e-3f;
	[Header("System settings")]
	public float startTime;
	public float SpeedUpFactor = 1000f; 

//	Planet earth;
//	Moon moon;
//	Planet mercury;
//	Planet venus;
//	Planet mars;
//	Planet jupiter;
//	Planet saturnus;
//	Planet uranus;
//	Planet neptune;

//	Orbit earthOrbit;
//	Orbit moonOrbit;
//	Orbit mercuryOrbit;
//	Orbit venusOrbit;
//	Orbit marsOrbit;
//	Orbit saturnusOrbit;
//	Orbit jupiterOrbit;
//	Orbit uranusOrbit;
//	Orbit neptuneOrbit;

//	public Canvas canvas;
//	public GameObject planetTemplate;
//	public GameObject sunTemplate;
//	GameObject earthObj;
//	GameObject moonObj;
//	GameObject mercuryObj;
//	GameObject venusObj;
//	GameObject uranusObj;
//	GameObject neptuneObj;
//	GameObject marsObj;
//	GameObject saturnusObj;
//	GameObject jupiterObj;
//	GameObject sunObj;

	void Start()
	{
		Scaling.DistanceScale = DistanceScale;
		Scaling.MoonDistanceScale = MoonDistanceScale;

		Scaling.UseLogScale = UseLogScale;
		Scaling.LogBaseAxisLength = LogBaseAxisLength;
		Scaling.PlanetSizeScale = PlanetSizeScale;
		Scaling.MoonToPlanetRatio = MoonToPlanetRatio;
		Scaling.SunSizeScale = SunSizeScale;
		Scaling.SpeedUpFactor = SpeedUpFactor;

		SolarSystemData data = DataProcessor.LoadSolarSystem("SolarSystem.json");
		solarSystem = new SolarSystem(data);
		solarSystem.Start();
	}

	void Update()
	{
		solarSystem.Update(Time.timeSinceLevelLoad + startTime);
	}
//		sun = new Sun("Sun", Sun.Category.YellowDwarf, 1.98855e30f, 6.95700e5f, 4.6e9f); 
//		Planet[] planets = new Planet[8];

//		earthOrbit = Orbit.Create(1.521e8f, 1.47095e8f, sun);
//		uranusOrbit = Orbit.Create(3.008413e9f, 2.742128e9f, sun);
//		neptuneOrbit = Orbit.Create(4.537303e9f, 4.459512e9f, sun);
//		mercuryOrbit = Orbit.Create(6.98169e7f, 4.60012e7f, sun);
//		venusOrbit = Orbit.Create(1.08939e8f, 1.07477e8f, sun);
//		marsOrbit = Orbit.Create(2.49230e8f, 2.06654e8f, sun);
//		saturnusOrbit = Orbit.Create(1.508844e9f, 1.349971e9f, sun);
//		jupiterOrbit = Orbit.Create(8.16044e8f, 7.40552e8f, sun);
//		planets[0] = new Planet("Earth", Planet.Category.Terrestial, 6.371e3f, 5.97237e24f, earthOrbit);
//		moonOrbit = Orbit.Create(4.054e5f, 3.626e5f, planets[0]);

//		planets[0]. = new Moon("Moon", Planet.Category.Terrestial, 1.7371e3f, 7.342e22f, moonOrbit);
//		uranus = new Planet("Uranus", Planet.Category.IceGiant, 25.362e3f, 8.6810e25f, uranusOrbit);
//		neptune = new Planet("Neptune", Planet.Category.IceGiant, 24.622e3f, 1.0243e26f, neptuneOrbit);
//		venus = new Planet("Venus", Planet.Category.Terrestial, 6.0518e3f, 4.8675e24f, venusOrbit);
//		mercury = new Planet("Mercury", Planet.Category.Terrestial, 2.4397e3f, 3.3011e23f, mercuryOrbit);
//		mars = new Planet("Mars", Planet.Category.Terrestial, 3.3895e3f, 6.4171e23f, marsOrbit);
//		saturnus = new Planet("Saturnus", Planet.Category.GasGiant, 58.232e3f, 5.6836e26f, saturnusOrbit);
//		jupiter = new Planet("Jupiter", Planet.Category.GasGiant, 69.911e3f, 1.8986e27f, jupiterOrbit);

//		sun.SetPlanets(planets);

//		drawSun(sun.Name, sun.Position, sun.Radius);
//		earthObj = drawPlanet(earth.Name, earth.Position, earth.Radius, "#2D4671");
//		moonObj = drawPlanet(moon.Name, moon.Position, moon.Radius, "#505660");
//		venusObj = drawPlanet(venus.Name, venus.Position, venus.Radius, "#DCCC8E");
//		mercuryObj = drawPlanet(mercury.Name, mercury.Position, mercury.Radius, "#505660");
//		uranusObj = drawPlanet(uranus.Name, uranus.Position, uranus.Radius, "#B4F1ED");
//		neptuneObj = drawPlanet(neptune.Name, neptune.Position, neptune.Radius, "#531CF8");
//		marsObj = drawPlanet(mars.Name, mars.Position, mars.Radius, "#fc8865");
//		saturnusObj = drawPlanet(saturnus.Name, saturnus.Position, saturnus.Radius, "#FFE7AA");
//		jupiterObj = drawPlanet(jupiter.Name, jupiter.Position, jupiter.Radius, "#FFD7A4");
//		satelite = new Planet("Sateliet", Planet.Category.Terrestial, 20, 5e8f, orbit);
//		Orbit.OrbitDetails od1 = orbit.GetOrbitInformationAtTime(0);
//		Orbit.OrbitDetails od2 = orbit.GetOrbitInformationAtTime(orbit.Period/2);
//		Orbit.OrbitDetails od3 = orbit.GetOrbitInformationAtTime(2000f);
//		Debug.Log(od1.Altitude);
//		Debug.Log(od1.Velocity);
//		Debug.Log(od1.Position);
//		Debug.Log("---------------");
//		Debug.Log(od2.Altitude);
//		Debug.Log(od2.Velocity);
//		Debug.Log(od2.Position);
//		Debug.Log("---------------");
//		Debug.Log(od3.Altitude);
//		Debug.Log(od3.Velocity);
//		Debug.Log(od3.Position);
//
//		Debug.Log("---------------");
//		Debug.Log(orbit.Period);

//	}

//	void InstanceMainBody()
//	{
//
//	}
//
//	void InstanceOrbitingBody(PlanetData data, CelestialBody mainBody)
//	{
//		Orbit orbit = Orbit.Create(data.Apoapsis, data.Periapsis, mainBody);
//		Planet planet = new Planet(data.Name, data.Type, data.Color, data.Radius, data.Mass, orbit);
//	}

//	void Update()
//	{
//		float time = Time.timeSinceLevelLoad;
//		earth.SetPosition(earthOrbit.GetOrbitInformationAtTime(time).Position);
//		moon.SetPosition(earthOrbit.GetOrbitInformationAtTime(time).Position);
//		venus.SetPosition(earthOrbit.GetOrbitInformationAtTime(time).Position);
//		mercury.SetPosition(earthOrbit.GetOrbitInformationAtTime(time).Position);
//		uranus.SetPosition(earthOrbit.GetOrbitInformationAtTime(time).Position);
//		neptune.SetPosition(earthOrbit.GetOrbitInformationAtTime(time).Position);
//		earth.SetPosition(earthOrbit.GetOrbitInformationAtTime(time).Position);
//
//		earthObj.transform.position = earth.Position;
//		moonObj.transform.position = moonOrbit.GetOrbitInformationAtTime(time).Position;
//		venusObj.transform.position = venusOrbit.GetOrbitInformationAtTime(time).Position;
//		mercuryObj.transform.position = mercuryOrbit.GetOrbitInformationAtTime(time).Position;
//		uranusObj.transform.position = uranusOrbit.GetOrbitInformationAtTime(time).Position;
//		neptuneObj.transform.position = neptuneOrbit.GetOrbitInformationAtTime(time).Position;
//		marsObj.transform.position = marsOrbit.GetOrbitInformationAtTime(time).Position;
//		saturnusObj.transform.position = saturnusOrbit.GetOrbitInformationAtTime(time).Position;
//		jupiterObj.transform.position = jupiterOrbit.GetOrbitInformationAtTime(time).Position;
//	}

//	void setPosition(CelestialBody body)
//	{
//
//	}
//
//	GameObject drawPlanet(string name, Vector2 pos, float radius, string colorString)
//	{
//		GameObject tmp = Instantiate(planetTemplate, new Vector3(pos.x, pos.y, 1), Quaternion.identity) as GameObject;
//		tmp.name = name;
//		Color color = new Color();
//		ColorUtility.TryParseHtmlString(colorString, out color);
//		tmp.GetComponent<Shape>().color = color;
//		float d = radius * 2;
//		tmp.transform.SetParent(canvas.transform, true);
//		tmp.GetComponent<RectTransform> ().sizeDelta = new Vector2(d, d);
//		return tmp;
//	}
//
//	GameObject drawSun(string name, Vector2 pos, float radius)
//	{
//		GameObject tmp = Instantiate(sunTemplate, new Vector3(pos.x, pos.y, 1), Quaternion.identity) as GameObject;
//		tmp.name = name;
//		float d = radius * 2;
//		tmp.transform.SetParent(canvas.transform, true);
//		tmp.GetComponent<RectTransform> ().sizeDelta = new Vector2(d, d);
//		return tmp;
//	}
}
