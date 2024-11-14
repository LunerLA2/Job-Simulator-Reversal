using System;
using OwlchemyVR;
using UnityEngine;

public class MechanicDesktopComputerProgram : ComputerProgram
{
	[SerializeField]
	private GameObject desktopScreen;

	[SerializeField]
	private GrabbableItem mouseGrabbableItem;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.Desktop;
		}
	}

	private void OnEnable()
	{
		hostComputer.ShowCursor();
		desktopScreen.SetActive(true);
		blockScreensaver = false;
		blockAlerts = false;
		GrabbableItem grabbableItem = mouseGrabbableItem;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnGrabbed, new Action<GrabbableItem>(MouseGrabbed));
		GrabbableItem grabbableItem2 = mouseGrabbableItem;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem2.OnReleased, new Action<GrabbableItem>(MouseReleased));
	}

	private void OnDisable()
	{
		GrabbableItem grabbableItem = mouseGrabbableItem;
		grabbableItem.OnGrabbed = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnGrabbed, new Action<GrabbableItem>(MouseGrabbed));
		GrabbableItem grabbableItem2 = mouseGrabbableItem;
		grabbableItem2.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem2.OnReleased, new Action<GrabbableItem>(MouseReleased));
	}

	private void MouseGrabbed(GrabbableItem item)
	{
	}

	private void MouseReleased(GrabbableItem item)
	{
	}

	protected override bool OnMouseMove(Vector2 cursorPos)
	{
		return base.OnMouseMove(cursorPos);
	}

	protected override void OnClickableClicked(ComputerClickable clickable)
	{
		if (clickable != null)
		{
			if (clickable.name == "Parts")
			{
				hostComputer.StartProgram(ComputerProgramID.OrderParts);
			}
			else if (clickable.name == "Paint")
			{
				hostComputer.StartProgram(ComputerProgramID.PaintCar);
			}
			else if (clickable.name == "Receipt")
			{
				hostComputer.StartProgram(ComputerProgramID.PrintReceipt);
			}
		}
	}

	protected override bool OnKeyPress(string code)
	{
		return true;
	}
}
