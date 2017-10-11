using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Rectangle: Shape
{
	private int sides = 4;

	protected override void OnPopulateMesh(Mesh vh)
	{
		vh.Clear ();
		VertexHelper vbo = new VertexHelper(vh);
		// This only works with anchor on center mid
		float halfWidth = rectTransform.rect.width / 2;
		float halfHeight = rectTransform.rect.height / 2;

		float minSize = halfWidth < halfHeight ? halfWidth : halfHeight;
		if (thickness >  minSize){
			thickness = minSize;
		}

		Vector2 prevX = new Vector2 (-halfWidth, -halfHeight);
		Vector2 prevY = new Vector2 (-halfWidth, -halfHeight);
		Vector2 uv0 = new Vector2(0, 0);
		Vector2 uv1 = new Vector2(0, 1);
		Vector2 uv2 = new Vector2(1, 1);
		Vector2 uv3 = new Vector2(1, 0);
		Vector2 pos0;
		Vector2 pos1;
		Vector2 pos2;
		Vector2 pos3;
		float degrees = 360f / sides;
		int vertices = sides + 1;
		plus = 0;

		if (fill) {
			shapeVerts = new Vector3[4];
			pos0 = new Vector2 (-halfWidth, -halfHeight);
			pos1 = new Vector2 (-halfWidth, halfHeight);
			pos2 = new Vector2 (halfWidth, halfHeight);
			pos3 = new Vector2 (halfWidth, -halfHeight);
			vbo.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
		} else {
			shapeVerts = new Vector3[4*vertices];
			for (int i = 0; i < vertices; i++)
			{
//				float outer = Mathf.Sqrt((halfWidth * halfWidth) + (halfHeight * halfHeight));
//				float inner = outer - (Mathf.Sqrt(2) * thickness);

				float rad = Mathf.Deg2Rad * (-i * degrees - 45);

				float c = Mathf.Round(Mathf.Cos(rad));
				float s = Mathf.Round(Mathf.Sin(rad));

				pos0 = prevX;
				pos1 = new Vector2(halfWidth * c, halfHeight * s);
				pos2 = new Vector2((halfWidth-thickness) * c, (halfHeight-thickness) * s);
				pos3 = prevY;
				prevX = pos1;
				prevY = pos2;

				vbo.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
			}
		}
		vbo.FillMesh(vh);
	}
}