using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Polygon : Shape
{
	public Vector3[] vees;

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
		}
		return vbo;
	}

	protected override void OnPopulateMesh(Mesh vh)
	{
		vh.Clear ();
		VertexHelper vbo = new VertexHelper(vh);
		shapeVerts = new Vector3[vees.Length*20];

		if (vees != null) {
			Vector3[] boundaryVerts = new Vector3[vees.Length];

			if (vees.Length > 0) {
				Vector2 startV = vees [0];
				Vector2 prevX = startV;

				Vector2 pos0;
				Vector2 pos1;
				Vector2 pos2;
				Vector2 pos3;
				Vector2 uv0 = new Vector2 (0, 1);
				Vector2 uv1 = new Vector2 (1, 1);
				Vector2 uv2 = new Vector2 (1, 0);
				Vector2 uv3 = new Vector2 (0, 0);
				for (int i = 0; i < vees.Length; i++) {
					pos0 = prevX;
					pos1 = vees [i];
					pos2 = startV;
					pos3 = startV;
					//				pos2 = Vector2.zero;
					//				pos3 = Vector2.zero;
					prevX = pos1;
					vbo.AddUIVertexQuad (SetVbo (new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
					boundaryVerts [i] = vees [i];
				}
			}
			if (vbo.currentVertCount > 3)
			{
				vbo.FillMesh(vh);
				shapeVerts = boundaryVerts;
			}
		}

		vbo.FillMesh(vh);
	}

	public void DrawPolygon(Vector3[] verts)
	{
		vees = verts;
	}
}