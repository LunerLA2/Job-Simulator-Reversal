using System;
using UnityEngine;

public class UniqueObject : MonoBehaviour
{
	public Action<Bot> OnWasPulledIntoInventoryOfBot;

	public Action OnWasEjectedFromInventoryOfBot;

	public Action<Bot> OnWasMovedToByBot;

	public Action<Bot> OnWasLookedAtByBot;

	[SerializeField]
	private bool manageRegistrationAutomatically = true;

	[SerializeField]
	private bool forceNameOfObject;

	[SerializeField]
	private string forcedNameToApply = string.Empty;

	[SerializeField]
	private bool remainRegisteredWhileInactive;

	[SerializeField]
	private AttachablePoint optionalAssociatedAttachpoint;

	[SerializeField]
	private ClearAreaChecker optionalAssociatedClearAreaChecker;

	private bool selfIsRegistered;

	private bool selfWasPassedToChild;

	public AttachablePoint AssociatedAttachpoint
	{
		get
		{
			return optionalAssociatedAttachpoint;
		}
		set
		{
			optionalAssociatedAttachpoint = value;
		}
	}

	public ClearAreaChecker AssociatedClearAreaChecker
	{
		get
		{
			return optionalAssociatedClearAreaChecker;
		}
	}

	public bool RemainRegisteredWhileInactive
	{
		get
		{
			return remainRegisteredWhileInactive;
		}
		set
		{
			remainRegisteredWhileInactive = value;
		}
	}

	private void Awake()
	{
		if (selfIsRegistered || selfWasPassedToChild)
		{
			return;
		}
		BasePrefabSpawner component = GetComponent<BasePrefabSpawner>();
		if (component != null)
		{
			if (forceNameOfObject)
			{
				component.LastSpawnedPrefabGO.name = forcedNameToApply;
			}
			UniqueObject uniqueObject = component.LastSpawnedPrefabGO.AddComponent<UniqueObject>();
			uniqueObject.TransferSettings(forceNameOfObject, forcedNameToApply);
			selfWasPassedToChild = true;
			return;
		}
		if (forceNameOfObject)
		{
			base.gameObject.name = forcedNameToApply;
		}
		if (manageRegistrationAutomatically)
		{
			BotUniqueElementManager.Instance.RegisterObject(this);
			selfIsRegistered = true;
		}
	}

	private void OnEnable()
	{
		if (!selfIsRegistered && !selfWasPassedToChild && manageRegistrationAutomatically)
		{
			BotUniqueElementManager.Instance.RegisterObject(this);
			selfIsRegistered = true;
		}
	}

	private void OnDisable()
	{
		if (!remainRegisteredWhileInactive && manageRegistrationAutomatically)
		{
			Unregister();
		}
	}

	private void OnDestroy()
	{
		if (manageRegistrationAutomatically)
		{
			Unregister();
		}
	}

	private void Unregister()
	{
		if (!Application.isPlaying || selfWasPassedToChild)
		{
			selfIsRegistered = false;
		}
		else if (selfIsRegistered)
		{
			if (BotUniqueElementManager._instanceNoCreate != null)
			{
				BotUniqueElementManager._instanceNoCreate.UnregisterObject(this);
			}
			selfIsRegistered = false;
		}
	}

	public void ManualChangeName(string changeTo)
	{
		Unregister();
		forceNameOfObject = true;
		forcedNameToApply = changeTo;
		base.gameObject.name = forcedNameToApply;
		BotUniqueElementManager.Instance.RegisterObject(this);
		selfIsRegistered = true;
	}

	public void TransferSettings(bool forceName, string nameText)
	{
		forceNameOfObject = forceName;
		forcedNameToApply = nameText;
	}

	public void WasPulledIntoInventoryOfBot(Bot bot)
	{
		if (OnWasPulledIntoInventoryOfBot != null)
		{
			OnWasPulledIntoInventoryOfBot(bot);
		}
	}

	public void WasEjectedFromInventoryOfBot()
	{
		if (OnWasEjectedFromInventoryOfBot != null)
		{
			OnWasEjectedFromInventoryOfBot();
		}
	}

	public void WasMovedToByBot(Bot bot)
	{
		if (OnWasMovedToByBot != null)
		{
			OnWasMovedToByBot(bot);
		}
	}

	public void WasLookedAtByBot(Bot bot)
	{
		if (OnWasLookedAtByBot != null)
		{
			OnWasLookedAtByBot(bot);
		}
	}

	public void Reregister()
	{
		if (selfIsRegistered)
		{
			BotUniqueElementManager.Instance.RegisterObject(this);
		}
	}
}
