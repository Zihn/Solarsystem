using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Shape: MaskableGraphic
{
	public bool fill = true;
	public float thickness = 5;
	protected Vector3[] shapeVerts;
	protected int plus;
	protected Vector3 centre;
	protected float width;
	protected float height;	

	void Awake() 
	{
		height = rectTransform.rect.height;
		width = rectTransform.rect.width;
	}

	public void SetBoundingBox (Vector2 v)
	{
		rectTransform.sizeDelta = new Vector2(Mathf.Abs(v.x)*2, Mathf.Abs(v.y)*2);
	}

	protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
	{
		UIVertex[] vbo = new UIVertex[4];
		for (int i = 0; i < 4; i++)
		{
			var vert = UIVertex.simpleVert;
			vert.color = color;
			vert.position = vertices[i];
			vert.uv0 = uvs[i];
			vbo[i] = vert;
			shapeVerts [i+plus] = vertices [i];
		}
		plus += 4;
		return vbo;
	}

	public Vector3[] GetVertices()
	{
		return shapeVerts;
	}

	public Vector2 GetCentre()
	{
		centre = rectTransform.localPosition;
		return centre;
	}

	/// <summary>
	/// Determines if the given point is inside the polygon
	/// Only for convex filled shapes 
	/// </summary>
	/// <param name="polygon">the vertices of polygon</param>
	/// <param name="testPoint">the given point</param>
	/// <returns>true if the point is inside the polygon; otherwise, false</returns>
	/// SRC: http://stackoverflow.com/questions/4243042/c-sharp-point-in-polygon
	public bool IsInShape(Vector3 point)
	{
		centre = rectTransform.localPosition;
		// Much faster method to check for circles
		if (this.GetType() == typeof(Circle) && shapeVerts.Length > 30) {
			float d = Vector2.Distance (centre, point);
			if (d < rectTransform.rect.width/2) {
				return true;
			}
			return false;
		} else {
			bool result = false;
			int j = shapeVerts.Length - 1;

			int l = j + 1;
			for (int i = 0; i < l; i++) {
				Vector3 vI = shapeVerts [i] + centre;
				Vector3 vJ = shapeVerts [j] + centre;

				if (vI.y < point.y && vJ.y >= point.y || vJ.y < point.y && vI.y >= point.y) {
					if (vI.x + (point.y - vI.y) / (vJ.y - vI.y) * (vJ.x - vI.x) < point.x) {
						result = !result;
					}
				}
				j = i;
			}
			return result;
		}
	}

	public Vector3[] OverlapVertices(Shape otherShape)
	{
		Vector3[] otherVertices = otherShape.GetVertices();
		Vector3 otherCentre = otherShape.centre; 
		List<Vector3> overlapVertices = new List<Vector3>();
		// Each shape has two lines intersecting the other shape's boundary
		// The first two are from the 'other' shape
		// Each line has one point in and one out the other shape's area
		Vector3[,] linePoints = new Vector3[4, 2];

		// Index of i and j vertices where i or j is the outer vertex,
		// Depending on position.. The outer vertex will be replaced by
		// the overlapping vertex. And marks the start/end of the overlap
		// boundary from either shape 1 or 2. 

		int[] overlapLineIdx = new int[4];
		int idx = 0;
		int iIdx = 0;

		int j = otherVertices.Length - 1;
		int l = j + 1;

		Vector3 vj = otherVertices [j]+otherCentre;
		bool prevInside = IsInShape(vj);

		for (int i = 0; i < l; i++)
		{
			Vector3 vi = otherVertices [i]+otherCentre;

			bool inside = IsInShape(vi);
			if (inside) {
				if (prevInside != inside) {
					linePoints [iIdx, 0] = vi;
					linePoints [iIdx, 1] = vj;
					overlapVertices.Add (vi);
					overlapLineIdx [iIdx] = idx;
					iIdx++;
					idx++;
				}
				overlapVertices.Add (vi);
				idx++;
			} else {
				if (prevInside != inside) {
					linePoints [iIdx, 0] = vj;
					linePoints [iIdx, 1] = vi;
					overlapVertices.Add (vj);
					overlapLineIdx [iIdx] = idx;
					iIdx++;
					idx++;
				}
			}
			j = i;
			prevInside = inside;
			vj = vi;
		}

		// Do it again
		j = shapeVerts.Length - 1;
		l = j + 1;

		vj = shapeVerts [j]+centre;
		prevInside = otherShape.IsInShape(vj);

		for (int i = 0; i < l; i++)
		{
			Vector3 vi = shapeVerts [i]+centre;

			bool inside = otherShape.IsInShape(vi);
			if (inside) {
				if (prevInside != inside) {
					linePoints [iIdx, 0] = vi;
					linePoints [iIdx, 1] = vj;
					overlapVertices.Add (vi);
					overlapLineIdx [iIdx] = idx;
					iIdx++;
					idx++;
				}
				overlapVertices.Add (vi);
				idx++;
			} else {
				if (prevInside != inside) {
					linePoints [iIdx, 0] = vi;
					linePoints [iIdx, 1] = vj;
					overlapVertices.Add (vj);
					overlapLineIdx [iIdx] = idx;
					iIdx++;
					idx++;
				}
			}
			j = i;
			prevInside = inside;
			vj = vi;
		}

		// Check the linepoints
		if (iIdx == 4) {
			for (int h = 0; h < 2; h++) {
				Vector2 p11 = linePoints [h, 0];
				Vector2 p12 = linePoints [h, 1];
				for (int g = 0; g < 2; g++) {
					Vector2 p21 = linePoints [g + 2, 0];
					Vector2 p22 = linePoints [g + 2, 1];
					Vector2 intersectPoint;
					bool intersect = LineSegmentsIntersect (p11, p12, p21, p22, out intersectPoint);
//					intersectPoint = new Vector3 (intersectPoint.x, intersectPoint.y, 0f);
					if (intersect) {
						overlapVertices [overlapLineIdx [h]] = intersectPoint; 
						overlapVertices [overlapLineIdx [g + 2]] = intersectPoint;
					}
				}
			}
		}
		return overlapVertices.ToArray();
	}

	/// <summary>
	/// Test whether two line segments intersect. If so, calculate the intersection point.
	/// see discussion: "http://stackoverflow.com/a/14143738/292237"/ 
	/// code from: "http://www.codeproject.com/Tips/862988/Find-the-Intersection-Point-of-Two-Line-Segments">
	/// </summary>
	/// <param name="p">Vector to the start point of p.</param>
	/// <param name="p2">Vector to the end point of p.</param>
	/// <param name="q">Vector to the start point of q.</param>
	/// <param name="q2">Vector to the end point of q.</param>
	/// <param name="intersection">The point of intersection, if any.</param>
	/// <param name="considerOverlapAsIntersect">Do we consider overlapping lines as intersecting?
	/// </param>
	/// <returns>True if an intersection point was found.</returns>
	bool LineSegmentsIntersect(Vector2 p, Vector2 p2, Vector2 q, Vector2 q2, 
		out Vector2 intersection)
	{
		intersection = new Vector2();
//		SC.DrawDebugLine (p, p2, 1);
//		SC.DrawDebugLine (q, q2, 1);

		Vector2 r = p2 - p;
		Vector2 s = q2 - q;
		float rxs = r.x*s.y - r.y*s.x;  //Cross product: v × w = vx wy − vy wx
		Vector2 qp = q - p;
		Vector2 pq = p - q;
		float qpxr = qp.x*r.y - qp.y*r.x;
		float qpxs = qp.x*s.y - qp.y*s.x;

		// If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
		if (rxs == 0 && qpxr == 0)
		{
			return false;
		}

		// If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
		if (rxs == 0 && qpxr != 0)
			return false;

		// t = (q - p) x s / (r x s)
		float t = qpxs/rxs;

		// u = (q - p) x r / (r x s)

		float u = qpxr/rxs;

		// If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
		// the two line segments meet at the point p + t r = q + u s.
		if (rxs != 0 && (0 <= t && t <= 1) && (0 <= u && u <= 1))
		{
			// We can calculate the intersection point using either t or u.
			intersection = p + t*r;

			// An intersection was found.
			return true;
		}
		// Otherwise, the two line segments are not parallel but do not intersect.
		return false;
	}
}
