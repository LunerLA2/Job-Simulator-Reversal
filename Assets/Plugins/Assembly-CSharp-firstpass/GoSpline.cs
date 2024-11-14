using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GoSpline
{
	private bool _isReversed;

	private AbstractGoSplineSolver _solver;

	public int currentSegment { get; private set; }

	public bool isClosed { get; private set; }

	public GoSplineType splineType { get; private set; }

	public List<Vector3> nodes
	{
		get
		{
			return _solver.nodes;
		}
	}

	public GoSpline(List<Vector3> nodes, bool useStraightLines = false)
	{
		if (useStraightLines || nodes.Count == 2)
		{
			splineType = GoSplineType.StraightLine;
			_solver = new GoSplineStraightLineSolver(nodes);
		}
		else if (nodes.Count == 3)
		{
			splineType = GoSplineType.QuadraticBezier;
			_solver = new GoSplineQuadraticBezierSolver(nodes);
		}
		else if (nodes.Count == 4)
		{
			splineType = GoSplineType.CubicBezier;
			_solver = new GoSplineCubicBezierSolver(nodes);
		}
		else
		{
			splineType = GoSplineType.CatmullRom;
			_solver = new GoSplineCatmullRomSolver(nodes);
		}
	}

	public GoSpline(Vector3[] nodes, bool useStraightLines = false)
		: this(new List<Vector3>(nodes), useStraightLines)
	{
	}

	public GoSpline(string pathAssetName, bool useStraightLines = false)
		: this(nodeListFromAsset(pathAssetName), useStraightLines)
	{
	}

	private static List<Vector3> nodeListFromAsset(string pathAssetName)
	{
		string empty = string.Empty;
		if (!pathAssetName.EndsWith(".asset"))
		{
			pathAssetName += ".asset";
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			empty = Path.Combine("jar:file://" + Application.dataPath + "!/assets/", pathAssetName);
			WWW wWW = new WWW(empty);
			while (!wWW.isDone)
			{
			}
			return bytesToVector3List(wWW.bytes);
		}
		empty = ((Application.platform != RuntimePlatform.IPhonePlayer) ? Path.Combine(Path.Combine(Application.dataPath, "StreamingAssets"), pathAssetName) : Path.Combine(Path.Combine(Application.dataPath, "Raw"), pathAssetName));
		byte[] bytes = File.ReadAllBytes(empty);
		return bytesToVector3List(bytes);
	}

	public static List<Vector3> bytesToVector3List(byte[] bytes)
	{
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < bytes.Length; i += 12)
		{
			Vector3 item = new Vector3(BitConverter.ToSingle(bytes, i), BitConverter.ToSingle(bytes, i + 4), BitConverter.ToSingle(bytes, i + 8));
			list.Add(item);
		}
		return list;
	}

	public Vector3 getLastNode()
	{
		return _solver.nodes[_solver.nodes.Count];
	}

	public void buildPath()
	{
		_solver.buildPath();
	}

	private Vector3 getPoint(float t)
	{
		return _solver.getPoint(t);
	}

	public Vector3 getPointOnPath(float t)
	{
		if (t < 0f || t > 1f)
		{
			t = ((!isClosed) ? Mathf.Clamp01(t) : ((!(t < 0f)) ? (t - 1f) : (t + 1f)));
		}
		return _solver.getPointOnPath(t);
	}

	public void closePath()
	{
		if (!isClosed)
		{
			isClosed = true;
			_solver.closePath();
		}
	}

	public void reverseNodes()
	{
		if (!_isReversed)
		{
			_solver.reverseNodes();
			_isReversed = true;
		}
	}

	public void unreverseNodes()
	{
		if (_isReversed)
		{
			_solver.reverseNodes();
			_isReversed = false;
		}
	}

	public void drawGizmos(float resolution)
	{
		_solver.drawGizmos();
		Vector3 to = _solver.getPoint(0f);
		resolution *= (float)_solver.nodes.Count;
		for (int i = 1; (float)i <= resolution; i++)
		{
			float t = (float)i / resolution;
			Vector3 point = _solver.getPoint(t);
			Gizmos.DrawLine(point, to);
			to = point;
		}
	}

	public static void drawGizmos(Vector3[] path, float resolution = 50f)
	{
		GoSpline goSpline = new GoSpline(path);
		goSpline.drawGizmos(resolution);
	}
}
