using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystem
{
	string _name;
	string _location;
	float _age;
	bool running;
	bool started;
	float systemLogScaleBase = float.MaxValue;

	Sun[] Suns;
	Planet[] OrbitingPlanets;

	GameObject systemObj;

	public SolarSystem(SolarSystemData Data)
	{
		Suns = new Sun[Data.Suns.Length];
		OrbitingPlanets = new Planet[Data.Planets.Length];

		systemObj = new GameObject();
		systemObj.name = Data.Name;
		systemObj.transform.position = Vector3.zero;
		systemObj.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, true);

		// Create Suns, Planets and their Moons
		for (int i = 0; i < Suns.Length; i++) {
			SunData sd = Data.Suns[i];
			Sun sun = new Sun(sd.Name, sd.Type, sd.Color, sd.Radius, sd.Mass, sd.Age, systemObj);
			sun.SetPosition(sd.Position);
			Suns[i] = sun;
		}
		for (int i = 0; i < OrbitingPlanets.Length; i++) {
			PlanetData pd = Data.Planets[i];
			Orbit orbit = Orbit.Create(pd.Apoapsis, pd.Periapsis, Suns[pd.OrbitingIndex]);
			systemLogScaleBase = systemLogScaleBase > Mathf.Log(orbit.SemiMajor) ? Mathf.Log(orbit.SemiMajor) : systemLogScaleBase;
			Debug.Log(pd.Name + " Orbit info, T: " + orbit.Period + " Apoapsis: " + orbit.ApoApsis + " Periapsis: " + orbit.PeriApsis);
			Planet planet = new Planet(pd.Name, pd.Type, pd.Color, pd.Radius, pd.Mass, orbit, Suns[pd.OrbitingIndex].GameObject);
			if (pd.Moons != null) {
				int nr = pd.Moons.Length;
				planet.Moons = new Moon[nr];
				for (int j = 0; j < nr; j++) {
					MoonData md = pd.Moons[j];
					Orbit moonOrbit = Orbit.Create(md.Apoapsis, md.Periapsis, planet);
					planet.Moons[j] = new Moon(md.Name, md.Type, md.Color, md.Radius, md.Mass, moonOrbit, md.Approachable, planet.GameObject);
				}
			}
			OrbitingPlanets[i] = planet;
		}
		Scaling.LogBase = Mathf.Floor(systemLogScaleBase);
		TimeAcceleration = Scaling.SpeedUpFactor;
		SetOrbitingPositions(0);
	}

	public string Name { get { return _name; } }
	public string Location { get { return _location; } }
	public float Age { get { return _age; } }
	public float TimeAcceleration { get; set; } 

	public void Start() { started = true; running = true; }

	public void Pause() { if (started) {running = !running; } }

	public void Restart() { SetOrbitingPositions(0); }

	public void Update(float time) { SetOrbitingPositions(time); }

	private void SetOrbitingPositions(float time)
	{
//		time *= TimeAcceleration;
		float logTime = time;						// Scale the speed up logaritmic
		foreach (Planet p in OrbitingPlanets) 
		{
			if (p.Orbit != null) 
			{
				if (Scaling.UseLogScale)
					logTime = time * Mathf.Log(p.Orbit.SemiMajor);
				p.SetPosition(p.Orbit.GetOrbitInformationAtTime(logTime).Position);
			}
			if (p.Moons != null) {
				foreach (Moon m in p.Moons) {
					if (m.Orbit != null) 			// As of now Moons always have an orbit.
						m.SetPosition(m.Orbit.GetOrbitInformationAtTime(time).Position);
				}
			}
		}
	}
}
	
public class CelestialBody
{
	string _name;
	string _type;
	Vector2 _position;
	float _mass;
	float _radius;

	// TODO calculate?
	float _gravity;
	float _karman; // https://physics.stackexchange.com/questions/31576/what-equations-constants-were-used-to-calculate-the-kármán-line-for-earth

	private GameObject _gameObject;
	private GameObject _parent;
	private GameObject _shadow;
	private float _parentRadius;

	public CelestialBody(string Name, string Category, string Color, float Radius, float Mass, GameObject Parent = null)
	{
		_name = Name;
		_type = Category;
		_mass = Mass;
		_radius = Radius;
		_position = Position;
		_parent = Parent;

		RectTransform rect = _parent.GetComponent<RectTransform>();
		_parentRadius = rect == null ? 0 : rect.sizeDelta.x / 2;

		float scaledRadius = ScaleSize(Radius);
		_gameObject = InstantiateGameObject(Name, Color, scaledRadius*2, Position);	
	}

	public string Name { get { return _name; } }
	public string Type { get { return _type; } }
	public float Mass { get { return _mass; } }
	public float Radius { get { return _radius; } }
	public Vector2 Position { get { return _position; } }
	public float KarmanLine { get { return _karman; } }
	public float GravitationalForce { get { return _gravity; } }
	public GameObject GameObject { get { return _gameObject; } }

	// TODO This can be sped up by removing the if checks and do that once..
	private Vector2 ScalePosition(Vector2 pos)
	{
		if (this.GetType() == typeof(Moon))
			pos = pos.normalized * (_parentRadius + (pos.magnitude * Scaling.MoonDistanceScale));
		else if (Scaling.UseLogScale && pos.magnitude != 0) {
			float log = Mathf.Log(pos.magnitude) - Scaling.LogBase;
			pos = pos.normalized * (_parentRadius + log * Scaling.LogBaseAxisLength);
		}else
			pos = pos.normalized * (_parentRadius + (pos.magnitude * Scaling.DistanceScale)) ;
		return pos;
	}

	private float ScaleSize(float r)
	{
		if (this.GetType() == typeof(Sun))
			r *= Scaling.SunSizeScale;
		else if (this.GetType() == typeof(Planet))
			r *= Scaling.PlanetSizeScale;
		else if (this.GetType() == typeof(Moon))
			r *= Scaling.PlanetSizeScale * Scaling.MoonToPlanetRatio;
		return r;
	}

	private GameObject InstantiateGameObject(string n, string c, float d, Vector2 pos)
	{
		if(_parent == null)
			_parent = GameObject.FindGameObjectWithTag("Canvas");
		GameObject bodyObj = new GameObject();
		bodyObj.name = n;
		BodyClick click = bodyObj.AddComponent<BodyClick>();
		click.Body = this;
		Circle s = bodyObj.AddComponent<Circle>();
		RectTransform r = bodyObj.GetComponent<RectTransform>();
		Color color = new Color();
		ColorUtility.TryParseHtmlString(c, out color);
		s.color = color;
		bodyObj.transform.SetParent(_parent.transform, true);
		r.sizeDelta = new Vector2(d, d);
		//Shadow
		if (this.GetType() != typeof(Sun)) {
			GameObject shadowObj = new GameObject();
			shadowObj.name = "Shadow";
			shadowObj.AddComponent<Shadow>();
			shadowObj.GetComponent<RectTransform>().sizeDelta = new Vector2(d, d);
			shadowObj.transform.SetParent(bodyObj.transform, false);
			_shadow = shadowObj;
		}
		return bodyObj;
	}

	public void SetPosition(Vector2 position)
	{
		_position = position;
		Vector2 scaledPos = ScalePosition(position);
		_gameObject.transform.localPosition = scaledPos;
		if (_shadow != null) {
			float alpha;
			// TODO needs to find the sun coords
			if (this.GetType() == typeof(Moon)) 
				alpha = Mathf.Atan2(_parent.transform.position.y - 0, _parent.transform.position.x - 0);
			else 
				alpha = Mathf.Atan2(scaledPos.y - _parent.transform.position.y, scaledPos.x - _parent.transform.position.x);
			_shadow.transform.localRotation = Quaternion.Euler(0f, 0f, alpha * Mathf.Rad2Deg);
		}
	}
}

//  Some known suns: https://www.universetoday.com/24299/types-of-stars/
public class Sun : CelestialBody
{
	float _age;
	float _temperature; // In kelvin, celcius or fahrenheit?
	Planet[] _planets;
	Difficulty difficulty;

	public Sun(string Name, string Category, string Color, float Radius, float Mass, float Age, GameObject Parent)
		: base(Name, Category, Color, Radius, Mass, Parent)
	{
		_age = Age;
	}

	public void SetPlanets(Planet[] planets)
	{
		_planets = planets;
	}

	public Planet[] Planets { get { return _planets; } }

	//	public class Photosphere
	//	{
	//		// https://en.wikipedia.org/wiki/Photosphere
	//		float temperature;
	//		System.Collections.Generic.Dictionary<Element, float> elements;
	//	}

	public class Category
	{
		// TODO inser more 'fictional' sun types
		//https://en.wikipedia.org/wiki/Stellar_classification
		public static string Dwarf { get { return "Dwarf"; } } 
		public static string BrownDwarf { get { return "Brown Dwarf"; } } 
		public static string RedDwarf { get { return "Red Dwarf"; } } 
		public static string SubDwarf { get { return "Sub Dwarf"; } } 
		public static string WhiteDwarf { get { return "White Dwarf"; } } 
		public static string YellowDwarf { get { return "Yellow Dwarf"; } } 
		public static string Giant { get { return "Giant"; } } 
		public static string BrightGiant { get { return "Bright Giant"; } } 
		public static string SubGiant { get { return "Sub Giant"; } } 
		public static string SuperGiant { get { return "Super Giant"; } } 
		public static string HyperGiant { get { return "Hyper Giant"; } }
	}
}


public class Planet : CelestialBody
{
	Orbit _orbit;
	Planet.Atmosphere _atmosphere;
	Resources _resources;

	public Planet (string Name, string Category, string Color, float Radius, float Mass, Orbit Orbit = null, GameObject Parent = null)
		: base(Name, Category, Color, Radius, Mass, Parent)
	{
		_orbit = Orbit;
		if (Orbit != null)
			SetPosition(Orbit.GetOrbitInformationAtTime(0).Position);
	}

	public Orbit Orbit { get { return _orbit; } }
	public Moon[] Moons { get; set; }


	// TODO add more getters (atmosphere etc..)

	public class Atmosphere
	{
		float temperature;
		float pressure;
		System.Collections.Generic.Dictionary<Element, float> elements;  // Element and its percentage
	}

	public class Category
	{
		// TODO inser more 'fictional' planets
		// https://en.wikipedia.org/wiki/List_of_planet_types
		// https://orig01.deviantart.net/66d3/f/2014/007/5/5/stars_in_shadow__special_planet_types_by_ariochiv-d6rjqj7.jpg
		public static string Carbon { get { return "Carbon"; } } 
		public static string Desert { get { return "Desert"; } }
		public static string Ice { get { return "Ice"; } }
		public static string IceGiant { get { return "Ice Giant"; } }
		public static string Iron { get { return "Iron"; } }
		public static string GasGiant { get { return "Gas Giant"; } }
		public static string Ocean { get { return "Ocean"; } }
		public static string Terrestial { get { return "Terrestial"; } }
	}

}

public class Moon : Planet
{
	bool _approachable;

	public Moon (string Name, string Category, string Color, float Radius, float Mass, Orbit Orbit, bool ApproachAsPlanet = false, GameObject Parent = null) 
		: base (Name, Category, Color, Radius, Mass, Orbit, Parent)
	{
		_approachable = ApproachAsPlanet;
	}
	public bool IsApproachable { get { return _approachable; } }
	public Moon[] Satellites { get; set; }

}