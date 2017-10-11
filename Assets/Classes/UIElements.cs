using System.Globalization;

public static class UIElements 
{
	static int KilometersMax = 6;
	static int KilogramsMax = 4;
	static int DaysMax = 325 * 2;	// If period more than 2 yrs than display as years
	static int HoursMax = 24 * 3; 	// if more than 3 days display in days


	public static string FormatKM(float kilometers)
	{
		if(kilometers.ToString().Length > UIElements.KilometersMax)
			return kilometers.ToString("0.###e00", CultureInfo.InvariantCulture) + " km";
		else
			return string.Format("{0:#,##0}", kilometers) + " km";
	}

	public static string FormatKG(float kilograms)
	{
		if(kilograms.ToString().Length > UIElements.KilogramsMax)
			return kilograms.ToString("0.###e00", CultureInfo.InvariantCulture) + " kg";
		else
			return string.Format("{0:#,##0}", kilograms) + " kg";
	}

	public static string FormatPeriod(float period)
	{
		// The period will be in seconds.
		string end;
		float hours = period / 3600;
		float days = period / (3600 * 24);
		float years = period / (3600 * 24 * 365);
		float display;
		if (hours <= UIElements.HoursMax) {
			display = hours;
			end = " hours";
		} else if (days <= UIElements.DaysMax) {
			display = days;
			end = " days";
		} else {
			display = years;
			end = " years";
		}

		return CheckEndingZeros(string.Format("{0:0.00}", display)) + end;
	}

	public static string FormatSpeed(float speed)
	{
		string end;
		if (speed.ToString().Length < 4)
			end = " m/s";
		else
			end = " km/s";

		return CheckEndingZeros(string.Format("{0:0.00}", speed)) + end;
	}

	private static string CheckEndingZeros(string s)
	{
		if ( s.EndsWith("00") )
			return s.Substring(0, s.Length - 4);  // Remove the 00's and the comma
		else
			return s;
	}
}
