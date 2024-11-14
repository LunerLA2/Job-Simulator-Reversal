using System;
using UnityEngine;

[Serializable]
public class MailCartLoadInfo
{
	[SerializeField]
	private TaskData task;

	[SerializeField]
	private PageData[] clearAfterPages;

	[SerializeField]
	private TaskData clearOnTaskStart;

	[SerializeField]
	private GameObject load;

	[SerializeField]
	private float extraDelayBeforeSendingIn;

	[SerializeField]
	private float extraDelayBeforeSendingOut;

	[SerializeField]
	private bool unparentLoadWhenItArrives = true;

	[SerializeField]
	private bool alwaysTakeLoadAwayAsSoonAsFinished;

	[SerializeField]
	private bool useCartVisuals = true;

	[SerializeField]
	private bool exitToLeft;

	[SerializeField]
	private GrabbableHinge hinge;

	public Transform defaultParent;

	public TaskData Task
	{
		get
		{
			return task;
		}
	}

	public PageData[] ClearAfterPages
	{
		get
		{
			return clearAfterPages;
		}
	}

	public TaskData ClearOnTaskStart
	{
		get
		{
			return clearOnTaskStart;
		}
	}

	public GameObject Load
	{
		get
		{
			return load;
		}
	}

	public bool UnparentLoadWhenItArrives
	{
		get
		{
			return unparentLoadWhenItArrives;
		}
	}

	public bool AlwaysTakeLoadAwayAsSoonAsFinished
	{
		get
		{
			return alwaysTakeLoadAwayAsSoonAsFinished;
		}
	}

	public bool UseCartVisuals
	{
		get
		{
			return useCartVisuals;
		}
	}

	public float ExtraDelayBeforeSendingIn
	{
		get
		{
			return extraDelayBeforeSendingIn;
		}
	}

	public float ExtraDelayBeforeSendingOut
	{
		get
		{
			return extraDelayBeforeSendingOut;
		}
	}

	public bool ExitToLeft
	{
		get
		{
			return exitToLeft;
		}
	}

	public void HingeLoweLockWhenLeaving()
	{
		if (hinge != null)
		{
			hinge.LockLower();
		}
	}
}
