using System.Collections.Generic;
using UnityEngine;

public class GoSplineQuadraticBezierSolver : AbstractGoSplineSolver
{
	public GoSplineQuadraticBezierSolver(List<Vector3> nodes)
	{
		_nodes = nodes;
	}

	protected float quadBezierLength(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint)
	{
		Vector3[] array = new Vector3[2]
		{
			controlPoint - startPoint,
			startPoint - 2f * controlPoint + endPoint
		};
		if (array[1] != Vector3.zero)
		{
			float num = 4f * Vector3.Dot(array[1], array[1]);
			float num2 = 8f * Vector3.Dot(array[0], array[1]);
			float num3 = 4f * Vector3.Dot(array[0], array[0]);
			float num4 = 4f * num3 * num - num2 * num2;
			float num5 = 2f * num + num2;
			float num6 = num + num2 + num3;
			float num7 = 0.25f / num;
			float num8 = num4 / (8f * Mathf.Pow(num, 1.5f));
			return num7 * (num5 * Mathf.Sqrt(num6) - num2 * Mathf.Sqrt(num3)) + num8 * (Mathf.Log(2f * Mathf.Sqrt(num * num6) + num5) - Mathf.Log(2f * Mathf.Sqrt(num * num3) + num2));
		}
		return 2f * array[0].magnitude;
	}

	public override void closePath()
	{
	}

	public override Vector3 getPoint(float t)
	{
		float num = 1f - t;
		return num * num * _nodes[0] + 2f * num * t * _nodes[1] + t * t * _nodes[2];
	}

	public override void drawGizmos()
	{
		Color color = Gizmos.color;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(_nodes[0], _nodes[1]);
		Gizmos.DrawLine(_nodes[1], _nodes[2]);
		Gizmos.color = color;
	}
}
