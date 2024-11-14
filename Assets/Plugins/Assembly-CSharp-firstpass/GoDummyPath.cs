using System.Collections.Generic;
using UnityEngine;

public class GoDummyPath : MonoBehaviour
{
	public string pathName = string.Empty;

	public Color pathColor = Color.magenta;

	public List<Vector3> nodes = new List<Vector3>
	{
		Vector3.zero,
		Vector3.zero
	};

	public bool useStandardHandles;

	public bool forceStraightLinePath;

	public int pathResolution = 50;

	public void OnDrawGizmos()
	{
		if (!forceStraightLinePath)
		{
			GoSpline goSpline = new GoSpline(nodes);
			Gizmos.color = pathColor;
			goSpline.drawGizmos(pathResolution);
		}
	}
}
