using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Line: MaskableGraphic
{
	[SerializeField]
	public float thickness = 5;

	public Vector2 P1;
	public Vector2 P2;

	public void DrawLine(Vector2 p1, Vector2 p2) 
	{
		P1 = p1;
		P2 = p2;
	}

	protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
	{
		UIVertex[] vbo = new UIVertex[4];
		for (int i = 0; i < vertices.Length; i++)
		{
			var vert = UIVertex.simpleVert;
			vert.color = color;
			vert.position = vertices[i];
			vert.uv0 = uvs[i];
			vbo[i] = vert;
		}
		return vbo;
	}

	protected override void OnPopulateMesh(Mesh vh)
	{
		vh.Clear ();
		VertexHelper vbo = new VertexHelper(vh);
		float dist = Vector2.Distance (P1, P2);
		if (thickness > dist) {
			thickness = dist;
		}
		float alpha = Mathf.Atan2 (P2.y - P1.y, P2.x - P1.x);
		float shiftRad = Mathf.Deg2Rad * 90f;
		float radP = alpha + shiftRad;
		float radN = alpha - shiftRad;
		float cP = thickness/2 * Mathf.Cos(radP);
		float sP = thickness/2 * Mathf.Sin(radP);
		float cN = thickness/2 * Mathf.Cos(radN);
		float sN = thickness/2 * Mathf.Sin(radN);

		Vector2 uv0 = new Vector2(0, 0);
		Vector2 uv1 = new Vector2(0, 1);
		Vector2 uv2 = new Vector2(1, 1);
		Vector2 uv3 = new Vector2(1, 0);
		Vector2 pos0 = new Vector2(P1.x + cP, P1.y + sP);
		Vector2 pos1 = new Vector2(P2.x + cP, P2.y + sP);
		Vector2 pos2 = new Vector2(P2.x + cN, P2.y + sN);
		Vector2 pos3 = new Vector2(P1.x + cN, P1.y + sN);

		vbo.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));

		vbo.FillMesh(vh);
	}
}
