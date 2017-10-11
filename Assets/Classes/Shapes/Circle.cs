using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Circle : Shape
{
	[Range(0, 360)]
	public int segments = 360;

	protected override void OnPopulateMesh(Mesh vh)
	{
		float outer = -rectTransform.pivot.x * rectTransform.rect.width;
		float inner = -rectTransform.pivot.x * rectTransform.rect.width + this.thickness;

		vh.Clear ();
		VertexHelper vbo = new VertexHelper(vh);

		Vector2 prevX = Vector2.zero;
		Vector2 prevY = Vector2.zero;
		Vector2 uv0 = new Vector2(0, 0);
		Vector2 uv1 = new Vector2(0, 1);
		Vector2 uv2 = new Vector2(1, 1);
		Vector2 uv3 = new Vector2(1, 0);
		Vector2 pos0;
		Vector2 pos1;
		Vector2 pos2;
		Vector2 pos3;
	
		float f = (360 / 360f);
		float degrees = 360f / segments;
		int fa = (int)(segments * f);
		plus = 0;
		shapeVerts = new Vector3[4*(fa+1)];
		Vector3[] boundaryVerts = new Vector3[fa+1];

		for (int i = 0; i <= fa; i++)
		{
			float rad = Mathf.Deg2Rad * (-i * degrees);
			float c = Mathf.Cos(rad);
			float s = Mathf.Sin(rad);

			pos0 = prevX;
			pos1 = new Vector2(outer * c, outer * s);

			if (fill)
			{
				boundaryVerts [i] = pos1;
				pos2 = Vector2.zero;
				pos3 = Vector2.zero;
			}
			else
			{
				pos2 = new Vector2(inner * c, inner * s);
				pos3 = prevY;
			}

			prevX = pos1;
			prevY = pos2;

			vbo.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
		}
		shapeVerts = boundaryVerts;
		vbo.FillMesh(vh);
	}
}