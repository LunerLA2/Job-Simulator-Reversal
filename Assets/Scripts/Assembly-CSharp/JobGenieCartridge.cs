using System;
using OwlchemyVR;
using UnityEngine;

public class JobGenieCartridge : GameCartridge
{
	[Flags]
	public enum GenieModeTypes
	{
		None = 0,
		NoGravityMode = 1,
		DollhouseMode = 2,
		UnderwaterMode = 4,
		NightVisionMode = 8,
		RubberMode = 0x10,
		TurboMode = 0x20,
		OfficeModMode = 0x40,
		EndlessMode = 0x80
	}

	[SerializeField]
	private AttachablePoint attachablePoint;

	[SerializeField]
	private AttachableObject myAttachableObject;

	private GenieModeTypes thisCartsType;

	[SerializeField]
	private WorldItem worldItem;

	[SerializeField]
	private MeshRenderer meshRenderer;

	public AttachablePoint AttachablePoint
	{
		get
		{
			return attachablePoint;
		}
	}

	public AttachablePoint GetAttachablePoint
	{
		get
		{
			return attachablePoint;
		}
	}

	public void SetupMaterial(Material mat)
	{
		meshRenderer.material = mat;
	}

	private void OnEnable()
	{
		attachablePoint.OnObjectWasAttached += SomethingAttachedToGenie;
		attachablePoint.OnObjectWasDetached += SomethingDetachedFromGenie;
	}

	private void OnDisable()
	{
		attachablePoint.OnObjectWasAttached -= SomethingAttachedToGenie;
		attachablePoint.OnObjectWasDetached -= SomethingAttachedToGenie;
	}

	public override JobCartridgeWithGenieFlags GetJobCartridgeWithGenieFlags(GenieModeTypes types = GenieModeTypes.None)
	{
		if (attachablePoint.NumAttachedObjects > 0)
		{
			GameCartridge component = attachablePoint.AttachedObjects[0].GetComponent<GameCartridge>();
			if (component != null)
			{
				return component.GetJobCartridgeWithGenieFlags(types | thisCartsType);
			}
			Debug.LogError("Something was connected to " + base.gameObject.name + " but it didn't have a GameCartridge on it");
			return null;
		}
		return new JobCartridgeWithGenieFlags(null, thisCartsType);
	}

	public override void SetTerminalReference(TerminalManager terminal)
	{
		base.SetTerminalReference(terminal);
		if (attachablePoint.NumAttachedObjects > 0)
		{
			GameCartridge component = attachablePoint.GetAttachedObject(0).gameObject.GetComponent<GameCartridge>();
			if (component != null)
			{
				component.SetTerminalReference(terminal);
			}
		}
	}

	private void SomethingAttachedToGenie(AttachablePoint point, AttachableObject obj)
	{
		if (terminalReference != null)
		{
			terminalReference.ReCheckCartridge(point, myAttachableObject);
		}
		JobCartridge component = obj.GetComponent<JobCartridge>();
		if (component != null)
		{
			component.DisableBlowing();
			Debug.Log("blowing disabled");
		}
	}

	private void SomethingDetachedFromGenie(AttachablePoint point, AttachableObject obj)
	{
		if (terminalReference != null)
		{
			terminalReference.ReCheckCartridge(point, myAttachableObject);
		}
		JobCartridge component = obj.GetComponent<JobCartridge>();
		if (component != null)
		{
			component.EnableBlowing();
			Debug.Log("blowing enabled");
		}
	}

	public void SetGenieModeType(GenieModeTypes type, WorldItemData data)
	{
		thisCartsType = type;
		worldItem.ManualSetData(data);
		base.gameObject.name = data.ItemName;
	}
}
