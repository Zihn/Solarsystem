using UnityEngine;
public class Orbit
{
	static float GC = 6.674e-11f; 	// Gravitational Constant [m3 s−2 kg−1] to allow km input /1000.
	static float GCS = GC * 1e-3f * Scaling.DistanceScale; // Scaled down GC 
	float apoapsis;
	float periapsis;
	float semimajor;
	float eccentricity;
	float period;
	Vector2 baryCenter;
	float meanMotion;
	float mu;
	CelestialBody orbitingBody;

	// TODO Allow for rotated orbit
	private Orbit(float a, float p, float e, float op, float mm, float m, CelestialBody body)
	{
		apoapsis = a;
		periapsis = p;
		semimajor = (a + p) * 0.5f;
		eccentricity = e;
		baryCenter = new Vector2(a + semimajor, 0);//phi * semimajor*Mathf.Sqrt(1 - eccentricity * eccentricity));
		period = op;
		meanMotion = mm;
		mu = m;
		orbitingBody = body;
	}
		
	public float ApoApsis { get { return apoapsis; } }
	public float PeriApsis { get { return periapsis; } }
	public float SemiMajor { get { return semimajor; } }
	public float Eccentricity { get { return eccentricity; } }
	public float Period { get { return period; } }
	public float MeanMotion { get { return meanMotion; } }
	public Vector2 ApoApsisPosition { get { return new Vector2(-apoapsis, 0); } }  // Apoapsis is always left
	public Vector2 PeriApsisPosition { get { return new Vector2(periapsis, 0);} }
	public CelestialBody OrbitingBody { get { return orbitingBody; } }


	public class OrbitDetails
	{
		// Altitude [km], Velocity [km/s], position [km]
		public readonly float Altitude; public readonly float Velocity; public readonly Vector2 Position;
		public OrbitDetails(float a, float v, Vector2 p)
		{
			Altitude = a; Velocity = v; Position = p;
		}
	}

	public OrbitDetails GetOrbitInformationAtTime(float time)
	{
		//http://www.jgiesen.de/kepler/kepler.html
		// Init basic settings  (ADJUST ITER AND D for best results vs performance)
		int i = 0; int maxIter = 30; float pi = Mathf.PI; float K = pi/180f; float d = Mathf.Pow(10, -5);

		float t = time / (period * Scaling.SpeedUpFactor);					
		float M = 2f * pi * (t - Mathf.Floor(t));	// t - floor(t) will clamp it between 0-1
		
		float E = eccentricity < 0.8 ? M : pi;
		float F = E - eccentricity * Mathf.Sin(M) - M;

		// Find the Eccentric Anomaly
		while ((Mathf.Abs(F) > d) && (i < maxIter)) 
		{
			E = E - F / (1f-eccentricity*Mathf.Cos(E));
			F = E - eccentricity * Mathf.Sin(E) - M;
			i++;
		}
//		Debug.Log("iterations: " + i);
		float C = Mathf.Cos(E); float S = Mathf.Sin(E);
		float fak = Mathf.Sqrt(1f - eccentricity * eccentricity);
//		float phi = Mathf.Atan2(fak * S, C - eccentricity)/K;

		float X = semimajor * (C - eccentricity);
		float Y = semimajor * fak * S;

		Vector2 position = new Vector2(X, Y);
		// Altitude and Velocity
		float altitude = (semimajor * (1f-eccentricity*Mathf.Cos(E))) - orbitingBody.Radius;
		// https://en.wikipedia.org/wiki/Orbital_speed
		float velocity = Mathf.Sqrt(mu * ((2f / (altitude + orbitingBody.Radius)) - (1f / semimajor))) * 1e-3f;
		return new OrbitDetails(altitude, velocity, position);
	}

//	public Vector2 GetPosition(float time)
//	{
//		//http://www.jgiesen.de/kepler/kepler.html
//		float t = time / period;		// A percentage of the total period time
//		t = t <= 1 ? t : 1; 			// Clamp between 0-1
//		float M = speed * t;
//
//	}
//
//	public Vector2 GetAltitudeAndVelocity()
//	{
//
//	}

//	public static Orbit Create(float Apoapsis, float Periapsis, Sun OrbitingBody)
//	{
//		/// <summary> Enter variables in kilometers </summary>
//		return CreateOrbit(Apoapsis, Periapsis, OrbitingBody.Mass, OrbitingBody.Radius, OrbitingBody.Position);
//	}

	public static Orbit Create(float Apoapsis, float Periapsis, CelestialBody OrbitingBody)
	{
		/// <summary> Enter variables in kilometers </summary>
		return CreateOrbit(Apoapsis, Periapsis, OrbitingBody);
	}

	private static Orbit CreateOrbit(float a, float p, CelestialBody body)
	{
		// https://www.orbiter-forum.com/showthread.php?t=26682
		if (a < p) 
		{
			float t = p; p = a; a = t; // switch apoapsis/periapsis
		}

		float Ra = a;
		float Rp = p;
		float scaledMu = GCS * body.Mass;							// Used for position
		float mu = GC * body.Mass;
		float e = (Ra - Rp) / (Ra + Rp);							// Eccentricity
		float SMa = (Ra + Rp) * 0.5f;								// Semi-Major Axis

		float SMaUnscaled = SMa * 1e3f;								// Using meters					
		float SMa3 = SMaUnscaled * SMaUnscaled * SMaUnscaled;
		float Mm = Mathf.Sqrt(mu / SMaUnscaled) * 1e-3f; 			// Mean motion in km/s
		float T = 2*Mathf.PI*Mathf.Sqrt(SMa3 / mu);					// Orbital Period in [km/s]src: https://en.wikipedia.org/wiki/Orbital_period
				

//		Debug.Log("e " + e + " mu " + mu + " SMa " + SMa + " T " + T);
		// To calculate the circumpherence we use this approximation:
		// src: https://en.wikipedia.org/wiki/Ellipse#Circumference  where h = e^2
		//		double C = Math.PI*(Aphelion + Perihelion) * (1+((3*e*e)/(10+Math.Sqrt(4-(3*e*e)))));
		//		float s = ((float)C/OrbitalPeriod) / (24*60*60); // From km per day to km per second

		return new Orbit(Ra, Rp, e, T, Mm, scaledMu, body);
	}

	//	public Orbit FromAphelion(float Aphelion, float Eccentricity, float OrbitalPeriod)
	//	{
	//		
	//	}
	//
	//	public Orbit FromEllipse(float SemiMajor, float SemiMinor, Vector2 Center)
	//	{
	//
	//	}
	//
	//	public Orbit GenerateRandom(Vector2 Center = Vector2.zero)
	//	{
	//
	//	}
}