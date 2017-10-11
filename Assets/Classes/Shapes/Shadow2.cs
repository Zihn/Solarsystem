using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Shadow2 : Shape
{
	public Vector2 shadowCenter = Vector2.zero;
	[Range(1.1f, 5.0f)]
	public float shadowLineRadiusRatio = 1.1f;
	public bool flip;

	protected override void OnPopulateMesh(Mesh vh)
	{
		// TO create a shadow we need two circles, one for the edge and 
		// one for the shadow line. The circle creating the shadow line will
		// be shifted up left. To get its center we use the circle equation:
		// Equation of a circle - > (x - x0)^2 + (y - y0)^2 = r^2

		float outer = rectTransform.pivot.x * rectTransform.rect.width;
		float outerShadow = outer * shadowLineRadiusRatio;

		Color color = new Color();
		ColorUtility.TryParseHtmlString("#ffffffff", out color);
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

		Vector2 startPos = Vector2.zero; 
		Vector2 endPos = Vector2.zero;
		FindCircleCircleIntersections(Vector2.zero, outer, shadowCenter, outerShadow, out startPos, out endPos);

		int startDegree = GetDegreeOnCircle(startPos, Vector2.zero);
		int endDegree = GetDegreeOnCircle(endPos, Vector2.zero);
		int startShadowDegree = GetDegreeOnCircle(startPos, shadowCenter);
		int endShadowDegree = GetDegreeOnCircle(endPos, shadowCenter);

		int length = Mathf.Abs(endDegree - startDegree);

		float shadowRatio = Mathf.Abs(endShadowDegree - startShadowDegree)/(float)-length;


		Debug.Log(shadowCenter + " " + startDegree + "  " + endDegree + "  " + startShadowDegree + " " + endShadowDegree);

		Vector2 prev1 = GetVector(startDegree, outer);
		Vector2 prev3 = GetVector(startDegree + 1, outer);

		plus = 0;
		shapeVerts = new Vector3[4*2*(length + 1)];
		Vector3[] boundaryVerts = new Vector3[length+1];

		int iter = 0;
		while(iter < length + 1)
		{
			Vector2 newShadow = GetVector(startShadowDegree + (iter*shadowRatio), outerShadow) + shadowCenter;
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

	private void GetStartDegrees(Vector2 s, Vector2 e, Vector2 c, out int sd, out int ed)
	{
		sd = GetDegreeOnCircle(s, c);
		ed = GetDegreeOnCircle(e, c);
		if (sd > ed) {
			int t = sd;
			sd = ed;
			ed = t;
		}
	}

//	private float GetDegreeDifference(float start, float end)
//	{
//		if (start > end)
//			start += 360;
//		return Mathf.Abs(start - end);
//	}
//
	private int GetDegreeOnCircle(Vector2 pointOnCircle, Vector2 CircleCenter)
	{
		return (int)(360 - Mathf.Rad2Deg * Mathf.Atan2(pointOnCircle.y - CircleCenter.y, pointOnCircle.x - CircleCenter.x)) % 360;
	}

//	private int ClampDegree(int degrees){
//		return degrees % 360;
//	}
//
	private Vector2 GetVector(float iter, float radius)
	{
		float rad = Mathf.Deg2Rad * (iter);
		float c = Mathf.Cos(rad);
		float s = Mathf.Sin(rad);
		return new Vector2(radius * c, radius * s);
	}

	// Find the points where the two circles intersect.
	// SRC: http://csharphelper.com/blog/2014/09/determine-where-two-circles-intersect-in-c/
	private bool FindCircleCircleIntersections(
		Vector2 c1, float r1,
		Vector2 c2, float r2,
		out Vector2 intersection1, out Vector2 intersection2)
	{
		// Find the distance between the centers.
		float dx = c1.x - c2.x;
		float dy = c1.y - c2.y;
		float dist = Mathf.Sqrt(dx * dx + dy * dy);

//		dist > r1 + r2; 			// No solutions, the circles are too far apart.
//		dist < Mathf.Abs(r1 - r2);	// No solutions, one circle contains the other.
//		(dist == 0) && (r1 == r2); 	// No solutions, the circles coincide.

		Debug.Log(dist + "  r1 " + r1 + "  r2 " + r2);
		if (dist > r1 + r2 || dist < Mathf.Abs(r1 - r2) || (dist == 0) && (r1 == r2))
		{
			intersection1 = Vector2.zero;
			intersection2 = Vector2.zero;
			Debug.Log(false);
			return false;
		}
		else
		{
			// Find a and h.
			float a = (r1 * r1 -
				r2 * r2 + dist * dist) / (2 * dist);
			float h = Mathf.Sqrt(r1 * r1 - a * a);

			// Find P2.
			float cx2 = c1.x + a * (c2.x - c1.x) / dist;
			float cy2 = c1.y + a * (c2.x - c1.y) / dist;

			// Get the points P3.
			intersection1 = new Vector2(
				(float)(cx2 + h * (c2.y - c1.y) / dist),
				(float)(cy2 - h * (c2.x - c1.x) / dist));
			intersection2 = new Vector2(
				(float)(cx2 - h * (c2.y - c1.y) / dist),
				(float)(cy2 + h * (c2.x - c1.x) / dist));
			Debug.Log(true);

			return true;
		}
	}
}