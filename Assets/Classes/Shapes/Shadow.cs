using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Shadow : Shape
{
	[Range(0, 360)]
	public int startDegree = 90;
	[Range(0, 360)]
	public int endDegree = 270;
	[Range(1.1f, 5.0f)]
	public float shadowLineRadiusRatio = 1.1f;
	public bool flip;

	protected override void OnPopulateMesh(Mesh vh)
	{
		// TO create a shadow we need two circles, one for the edge and 
		// one for the shadow line. The circle creating the shadow line will
		// be shifted up left. To get its center we use the circle equation:
		// Equation of a circle - > (x - x0)^2 + (y - y0)^2 = r^2

		float outer = -rectTransform.pivot.x * rectTransform.rect.width;
		float outerShadow = outer * shadowLineRadiusRatio;

		if (startDegree > endDegree) {
			int tempDeg = startDegree;
			startDegree = endDegree;
			endDegree = startDegree;
		}

		int length = (int)Mathf.Abs(startDegree - endDegree);

		Color color = new Color();
		ColorUtility.TryParseHtmlString("#45454580", out color); // #45454580
		this.color = color;

		vh.Clear ();
		VertexHelper vbo = new VertexHelper(vh);
	
		Vector2 uv0 = new Vector2(0, 0);
		Vector2 uv1 = new Vector2(0, 1);
		Vector2 uv2 = new Vector2(1, 1);
		Vector2 uv3 = new Vector2(1, 0);
		Vector2 pos0;
		Vector2 pos1;
		Vector2 pos2;
		Vector2 pos3;

		Vector2 startPos = GetVector(startDegree, outer); 
		Vector2 endPos = GetVector(endDegree, outer); 
		Vector2 shadowCenter = CenterCircle(startPos.x, startPos.y, endPos.x, endPos.y, outerShadow);

		int startShadowDegree = GetDegreeOnCircle(startPos, shadowCenter) + 1;
		int endShadowDegree = GetDegreeOnCircle(endPos, shadowCenter);
		if (startShadowDegree > endShadowDegree) {
			int tempDeg = startShadowDegree;
			startShadowDegree = endShadowDegree;
			endShadowDegree = tempDeg;
		}
		float shadowDegree = GetDegreeDifference(startShadowDegree, endShadowDegree)/(float)length;
		shadowDegree = fill ? shadowDegree : -shadowDegree;

//		Debug.Log(shadowCenter + " " + startShadowDegree + "  " + endShadowDegree + "  " + shadowDegree);

		Vector2 prev1 = GetVector(startDegree, outer);
		Vector2 prev3 = GetVector(startDegree + 1, outer);

		plus = 0;
		shapeVerts = new Vector3[4*2*(length + 1)];
		Vector3[] boundaryVerts = new Vector3[length+1];

		int iter = 1;
		while(iter < length + 1)
		{
			Vector2 newShadow = GetVector(startShadowDegree +(iter*shadowDegree), outerShadow) + shadowCenter;
			Vector2 newEdge = GetVector(startDegree + iter, outer);

			for (int q = 0; q < 2; q++) {
				pos0 = newShadow;
				pos1 = prev3;
				pos2 = prev1;
				pos3 = pos0;
				prev1 = newEdge;
				vbo.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
			}
			prev1 = newShadow;
			prev3 = newEdge;
			iter++;
		}
			
		shapeVerts = boundaryVerts;
		vbo.FillMesh(vh);
		if (flip)
			gameObject.transform.eulerAngles = new Vector3(0, 0, 180);
		else
			gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
	}

	private float GetDegreeDifference(float start, float end)
	{
		if (!fill)
			start += 360;
		return Mathf.Abs(start - end);
	}

	private int GetDegreeOnCircle(Vector2 pointOnCircle, Vector2 CircleCenter)
	{
		return (int)(180  - Mathf.Rad2Deg * Mathf.Atan2(pointOnCircle.y - CircleCenter.y, pointOnCircle.x - CircleCenter.x)) % 360;
	}

	private int ClampDegree(int degrees){
		return degrees % 360;
	}

	private Vector2 GetVector(float iter, float radius)
	{
		float rad = Mathf.Deg2Rad * (-iter);
		float c = Mathf.Cos(rad);
		float s = Mathf.Sin(rad);
		return new Vector2(radius * c, radius * s);
	}
	// SRC: https://stackoverflow.com/questions/36211171/finding-arc-circle-center-given-2-points-and-radius
	private Vector2 CenterCircle(float x1, float y1, float x2, float y2, float radius)
	{
		float r2 = radius * radius;
		float q = Mathf.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
		float x3 = (x1 + x2) / 2;
		float y3 = (y1 + y2) / 2;

		float baseX = Mathf.Sqrt(r2 - ((q / 2) * (q / 2))) * ((y1 - y2) / q);
		float baseY = Mathf.Sqrt(r2 - ((q / 2) * (q / 2))) * ((x2 - x1) / q);

		float centerx1 = x3 + baseX; //center x of circle 1
		float centery1 = y3 + baseY; //center y of circle 1
		float centerx2 = x3 - baseX; //center x of circle 2
		float centery2 = y3 - baseY; //center y of circle 2

		int deg = GetDegreeOnCircle(new Vector2(centerx1, centery1), Vector2.zero);

		bool decision = fill ? startDegree > deg : startDegree < deg;

		if (decision)
			return new Vector2(centerx1, centery1);
		else
			return new Vector2(centerx2, centery2);
	}
}