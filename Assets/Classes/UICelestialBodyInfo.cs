using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICelestialBodyInfo : MonoBehaviour {

	int height;
	int width;

	int lineHeight = 60;
	int fontsizeTitle = 100;
	int fontsizeSub = 40;
	int fontsize = 30;

	public Text bodyName;
	public Text bodyType;
	public Text satellites;
	public Text nrOfSatellites;
	public Text mass;
	public Text radius;
	[Header("Orbit Info")]
	public GameObject orbit;
	public Text apoapsis;
	public Text periapsis;
	public Text eccentricity;
	public Text period;
	public Text speed;

	public void Start()
	{
		HideUI();
	}

	public void SetTarget(CelestialBody body, Camera cam)
	{
		string bodyTypeMain = body.GetType().ToString();
		string type = body.Type;
		if (bodyTypeMain != "Sun")
			type = bodyTypeMain + ", " + type;

		bodyName.text = body.Name;
		bodyType.text = type;
		mass.text = UIElements.FormatKG(body.Mass);
		radius.text = UIElements.FormatKM(body.Radius);

		// Orbit
		if (body.GetType() == typeof(Sun)) {
			satellites.text = "Planets";
			nrOfSatellites.text = (body as Sun).Planets != null ? (body as Sun).Planets.Length.ToString() : "0";
			orbit.SetActive(false);
		}else if (body.GetType() == typeof(Planet)) {
			Planet planet = (body as Planet);
			satellites.text = "Moons";
			nrOfSatellites.text = planet.Moons != null ? planet.Moons.Length.ToString() : "0";
			SetOrbitDetails(planet.Orbit);
			orbit.SetActive(true);
		}else if (body.GetType() == typeof(Moon)){
			Moon moon = (body as Moon);
			satellites.text = "Satellites";
			nrOfSatellites.text = moon.Moons != null ? moon.Satellites.Length.ToString() : "0";
			SetOrbitDetails(moon.Orbit);
			orbit.SetActive(true);
		}

		gameObject.SetActive(true);
	}

	private void SetOrbitDetails(Orbit orbit)
	{
		apoapsis.text = UIElements.FormatKM(orbit.ApoApsis);
		periapsis.text = UIElements.FormatKM(orbit.PeriApsis);
		eccentricity.text = orbit.Eccentricity.ToString();
		period.text = UIElements.FormatPeriod(orbit.Period);
		speed.text = UIElements.FormatSpeed(orbit.MeanMotion);
	}

	public void HideUI()
	{
		gameObject.SetActive(false);
	}
}
