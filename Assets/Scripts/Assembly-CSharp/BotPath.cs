using System.Collections.Generic;
using UnityEngine;

public class BotPath : MonoBehaviour
{
	public float timeToCompletePath = 10f;

	public GoEaseType easingType;

	public GoEaseType easeTypeIfUsedBackwards;

	public bool showGizmos = true;

	public Color gizmoColor = Color.yellow;

	[SerializeField]
	private BotPathWaypoint[] waypoints;

	private Vector3[] waypointPositions;

	public BotPathWaypoint[] Waypoints
	{
		get
		{
			return waypoints;
		}
	}

	private void Awake()
	{
		RebuildWaypointList();
		BotUniqueElementManager.Instance.RegisterPath(this);
	}

	private void RebuildWaypointList()
	{
		List<BotPathWaypoint> list = new List<BotPathWaypoint>(GetComponentsInChildren<BotPathWaypoint>());
		waypoints = list.ToArray();
		waypointPositions = new Vector3[waypoints.Length];
		for (int i = 0; i < waypoints.Length; i++)
		{
			waypointPositions[i] = waypoints[i].transform.position;
		}
	}

	public GoSpline GetPathAsGoSpline(bool useStraightLines, Vector3 includeExtraStartPosition, bool backwards)
	{
		List<Vector3> list = new List<Vector3>();
		list.Add(includeExtraStartPosition);
		list.Add(includeExtraStartPosition);
		for (int i = 0; i < waypointPositions.Length; i++)
		{
			list.Add(waypointPositions[i]);
		}
		list.Add(waypointPositions[waypointPositions.Length - 1]);
		if (backwards)
		{
			list.RemoveAt(0);
			list.RemoveAt(0);
			list.Insert(0, waypointPositions[0]);
			list.Reverse();
			list.Insert(0, includeExtraStartPosition);
			list.Insert(0, includeExtraStartPosition);
		}
		return new GoSpline(list, useStraightLines);
	}

	public GoSpline GetPathAsGoSpline(bool useStraightLines, bool backwards)
	{
		List<Vector3> list = new List<Vector3>();
		list.Add(waypointPositions[0]);
		for (int i = 0; i < waypointPositions.Length; i++)
		{
			list.Add(waypointPositions[i]);
		}
		list.Add(waypointPositions[waypointPositions.Length - 1]);
		if (backwards)
		{
			list.Reverse();
		}
		return new GoSpline(list, useStraightLines);
	}

	private void OnDrawGizmos()
	{
		RebuildWaypointList();
		for (int i = 0; i < waypoints.Length; i++)
		{
			if (waypoints[i].GetComponent<BotPathWaypoint>() == null)
			{
				waypoints[i].gameObject.AddComponent<BotPathWaypoint>().Initialize(this);
			}
			if (waypoints[i].name != "Waypoint " + i)
			{
				waypoints[i].name = "Waypoint " + i;
			}
		}
	}
}
