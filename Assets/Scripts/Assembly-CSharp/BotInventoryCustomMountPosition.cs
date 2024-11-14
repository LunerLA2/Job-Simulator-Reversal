using UnityEngine;

public class BotInventoryCustomMountPosition : MonoBehaviour
{
	[SerializeField]
	private Vector3 localPosWhenHeldAsLargeItem = Vector3.zero;

	[SerializeField]
	private Vector3 localRotWhenHeldAsLargeItem = Vector3.zero;

	[SerializeField]
	private Vector3 localRotWhenHeldAsSmallItem = Vector3.zero;

	[SerializeField]
	private bool registerAsLargeWhenGrabbedNormally;

	public Vector3 LocalPosWhenHeldAsLargeItem
	{
		get
		{
			return localPosWhenHeldAsLargeItem;
		}
	}

	public Vector3 LocalRotWhenHeldAsLargeItem
	{
		get
		{
			return localRotWhenHeldAsLargeItem;
		}
	}

	public Vector3 LocalRotWhenHeldAsSmallItem
	{
		get
		{
			return localRotWhenHeldAsSmallItem;
		}
	}

	public bool RegisterAsLargeWhenGrabbedNormally
	{
		get
		{
			return registerAsLargeWhenGrabbedNormally;
		}
	}
}
