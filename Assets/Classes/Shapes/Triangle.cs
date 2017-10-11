using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Triangle : Shape
{
	protected override void OnPopulateMesh(Mesh vh)
	{
		vh.Clear ();
		VertexHelper vbo = new VertexHelper(vh);
		// This only works with anchor on center mid
		float hW = rectTransform.rect.width / 2;
		float h = rectTransform.rect.height; 
		float hH = h/ 2;
//
		// l = h - s; (short and long lenght)
		// s = sqrt(h-s^2 - hW^2) -> s^2 = (h-s)^2 - hw^2 
		// s^2 = h^2 - 2hs + s^2 - hw^2 -> s = (h^2 - hw^2 )/ 2h
//		float tT = ((h*h) - (hW*hW))/(2*h);
//		Debug.Log (tT);
//		if (thickness > tT) {
//			thickness = tT;
//			Debug.Log ("thick");
//		}

		// Not quite correct quickfix :
		if (thickness > 0.3f * h) {
			thickness = 0.3f * h;
//			Debug.Log (thickness);
		}


		Vector2 uv0 = new Vector2(0, 0);
		Vector2 uv1 = new Vector2(0, 1);
		Vector2 uv2 = new Vector2(1, 1);
		Vector2 uv3 = new Vector2(1, 0);
		Vector2 pos0;Vector2 pos1;Vector2 pos2;Vector2 pos3;

		// Equilateral Triangle 
		// The triangle side lengths are based on the recttransform width
		// The radius of triangle is 2/3 of the triangle height. 
		//float tHr = Mathf.Sqrt ((2 * hH * 2 * hH) - (hH * hH))/3; 
		//float ofs = (2 * hH) - tHr*3;  //offset to put the triangle in the middle of the rectT

		plus = 0;
		if (fill) {
			shapeVerts = new Vector3[4];
			// Square Triangle
			pos0 = new Vector2 (-hW, -hH);
			pos1 = new Vector2 (0, hH);
			pos2 = new Vector2 (hW, -hH);
			// Equilateral Triangle
//			pos0 = new Vector2 (-hW, -tHr-ofs);
//			pos1 = new Vector2 (hW, -tHr-ofs);
//			pos2 = new Vector2 (0, tHr*2-ofs);
			pos3 = pos0;
			vbo.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
		} else {
			// Square triangel for equilateral see commented code below
			Vector2 prevO = new Vector2 (-hW, -hH);
			Vector2 prevI = new Vector2 (-hW, -hH);

			int vertices = 3 + 1;  // sides plus 1, first points is done twice
			shapeVerts = new Vector3[4*vertices];
//			float tNormal = (thickness / hW); //Double thickness is needed
			float angle = Mathf.Atan (1f / 0.5f); // angle of the equal corners
			// The y component of the thickness in the equal corners derived from half the angle
			float yT = thickness/Mathf.Tan(0.5f*angle); 
			float sqrtTN = Mathf.Sqrt(2*(yT*yT));

			float[] outerX = {-hW, 0, hW, -hW};
			float[] outerY = {-hH, hH, -hH, -hH};

			float[] innerX = {-hW+yT, 0, hW-yT, -hW+yT};
			float[] innerY = {-hH+thickness, hH-sqrtTN, -hH+thickness, -hH+thickness};

			for (int i = 0; i < outerX.Length; i++)
			{
				pos0 = prevO;
				pos1 = new Vector2(outerX[i], outerY[i]);
				pos2 = new Vector2(innerX[i], innerY[i]);
				pos3 = prevI;
				prevO = pos1;
				prevI = pos2;
				vbo.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
			}
		}
		vbo.FillMesh(vh);
	}
}

// THIS IS FOR EQUILATERAL TRIANGLE
//Vector2 prevO = new Vector2 (-hW, -tHr-ofs);
//Vector2 prevI = new Vector2 (-hW, -tHr-ofs);
//float degrees = 120f;
//int vertices = 3 + 1;  // sides plus 1, first points is done twice
//float d2r = Mathf.Deg2Rad;
//shapeVerts = new Vector3[4*vertices];
//
//for (int i = 0; i < vertices; i++)
//{
//	float outer = tHr*2;
//	float inner = outer - thickness;
//	float rad = d2r * (i * degrees - 30);
//	float c = Mathf.Cos(rad);
//	float s = Mathf.Sin(rad);
//
//	pos0 = prevO;
//	pos1 = new Vector2(outer * c, outer * s - ofs);
//	pos2 = new Vector2(inner * c, inner * s - ofs);
//	pos3 = prevI;
//	prevO = pos1;
//	prevI = pos2;
//
//	vbo.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
//}