using UnityEngine;

public static class UIColors
{
	public class UIColor
	{
		string _string;
		Color _color;
		public UIColor(string colorstring)
		{
			_string = colorstring;
			_color = UnityColor(colorstring);
		}

		private Color UnityColor(string c)
		{	
			Color color = new Color();
			ColorUtility.TryParseHtmlString(c, out color);
			return color;
		}

		public string Hex { get { return _string; } }
		public Color Color { get { return _color; } }
	}

	public static UIColor DarkBlueBG { get { return new UIColor("#1D314DFF"); } }
	public static UIColor DarkBlueBGTransparent { get { return new UIColor("#00265A80"); } }

}