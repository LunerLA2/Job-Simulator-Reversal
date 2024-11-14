using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractGoSplineSolver
{
	protected List<Vector3> _nodes;

	protected float _pathLength;

	protected int totalSubdivisionsPerNodeForLookupTable = 5;

	protected Dictionary<float, float> _segmentTimeForDistance;

	public List<Vector3> nodes
	{
		get
		{
			return _nodes;
		}
	}

	public virtual void buildPath()
	{
		int num = _nodes.Count * totalSubdivisionsPerNodeForLookupTable;
		_pathLength = 0f;
		float num2 = 1f / (float)num;
		_segmentTimeForDistance = new Dictionary<float, float>(num);
		Vector3 b = getPoint(0f);
		for (int i = 1; i < num + 1; i++)
		{
			float num3 = num2 * (float)i;
			Vector3 point = getPoint(num3);
			_pathLength += Vector3.Distance(point, b);
			b = point;
			_segmentTimeForDistance.Add(num3, _pathLength);
		}
	}

	public abstract void closePath();

	public abstract Vector3 getPoint(float t);

	public virtual Vector3 getPointOnPath(float t)
	{
		float num = _pathLength * t;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		foreach (KeyValuePair<float, float> item in _segmentTimeForDistance)
		{
			if (item.Value >= num)
			{
				num4 = item.Key;
				num5 = item.Value;
				if (num2 > 0f)
				{
					num3 = _segmentTimeForDistance[num2];
				}
				break;
			}
			num2 = item.Key;
		}
		float num6 = num4 - num2;
		float num7 = num5 - num3;
		float num8 = num - num3;
		t = num2 + num8 / num7 * num6;
		return getPoint(t);
	}

	public void reverseNodes()
	{
		_nodes.Reverse();
	}

	public virtual void drawGizmos()
	{
	}
}
