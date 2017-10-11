using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Ellipse : Shape
{
	[Range(0, 360)]
	public int segments = 360;
//	public float xRadius = 50;
//	public float yRadius = 50;
	//http://www.mathopenref.com/coordparamellipse.html

//	public void SetRadii (Vector2 v){
////		xRadius = v.x;
////		yRadius = v.y;
//		rectTransform.sizeDelta = new Vector2(Mathf.Abs(v.x)*2, Mathf.Abs(v.y)*2);
//	}

	protected override void OnPopulateMesh(Mesh vh)
	{
		float A = rectTransform.rect.width/2;
		float B = rectTransform.rect.height/2;

		float Ai = A - this.thickness;
		float Bi = B - this.thickness;


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
		Vector3[] boundaryVerts = new Vector3[fa+1]; //overlap

		for (int i = 0; i <= fa; i++)
		{
			float rad = Mathf.Deg2Rad * (-i * degrees);
			float c = Mathf.Cos(rad);
			float s = Mathf.Sin(rad);

			pos0 = prevX;
			pos1 = new Vector2(A * c, B * s);

			if (fill)
			{
				boundaryVerts [i] = pos1;
				pos2 = Vector2.zero;
				pos3 = Vector2.zero;
			}
			else
			{
				pos2 = new Vector2(Ai * c, Bi * s);
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