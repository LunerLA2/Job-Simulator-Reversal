using UnityEngine;
using Valve.VR;

public class SteamVR_TrackedController : MonoBehaviour
{
	public uint controllerIndex;

	public VRControllerState_t controllerState;

	public bool triggerPressed;

	public bool steamPressed;

	public bool menuPressed;

	public bool padPressed;

	public bool padTouched;

	public bool gripped;

	public event ClickedEventHandler MenuButtonClicked;

	public event ClickedEventHandler MenuButtonUnclicked;

	public event ClickedEventHandler TriggerClicked;

	public event ClickedEventHandler TriggerUnclicked;

	public event ClickedEventHandler SteamClicked;

	public event ClickedEventHandler PadClicked;

	public event ClickedEventHandler PadUnclicked;

	public event ClickedEventHandler PadTouched;

	public event ClickedEventHandler PadUntouched;

	public event ClickedEventHandler Gripped;

	public event ClickedEventHandler Ungripped;

	private void Start()
	{
		if (GetComponent<SteamVR_TrackedObject>() == null)
		{
			base.gameObject.AddComponent<SteamVR_TrackedObject>();
		}
		GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)controllerIndex;
		if (GetComponent<SteamVR_RenderModel>() != null)
		{
			GetComponent<SteamVR_RenderModel>().index = (SteamVR_TrackedObject.EIndex)controllerIndex;
		}
	}

	public virtual void OnTriggerClicked(ClickedEventArgs e)
	{
		if (this.TriggerClicked != null)
		{
			this.TriggerClicked(this, e);
		}
	}

	public virtual void OnTriggerUnclicked(ClickedEventArgs e)
	{
		if (this.TriggerUnclicked != null)
		{
			this.TriggerUnclicked(this, e);
		}
	}

	public virtual void OnMenuClicked(ClickedEventArgs e)
	{
		if (this.MenuButtonClicked != null)
		{
			this.MenuButtonClicked(this, e);
		}
	}

	public virtual void OnMenuUnclicked(ClickedEventArgs e)
	{
		if (this.MenuButtonUnclicked != null)
		{
			this.MenuButtonUnclicked(this, e);
		}
	}

	public virtual void OnSteamClicked(ClickedEventArgs e)
	{
		if (this.SteamClicked != null)
		{
			this.SteamClicked(this, e);
		}
	}

	public virtual void OnPadClicked(ClickedEventArgs e)
	{
		if (this.PadClicked != null)
		{
			this.PadClicked(this, e);
		}
	}

	public virtual void OnPadUnclicked(ClickedEventArgs e)
	{
		if (this.PadUnclicked != null)
		{
			this.PadUnclicked(this, e);
		}
	}

	public virtual void OnPadTouched(ClickedEventArgs e)
	{
		if (this.PadTouched != null)
		{
			this.PadTouched(this, e);
		}
	}

	public virtual void OnPadUntouched(ClickedEventArgs e)
	{
		if (this.PadUntouched != null)
		{
			this.PadUntouched(this, e);
		}
	}

	public virtual void OnGripped(ClickedEventArgs e)
	{
		if (this.Gripped != null)
		{
			this.Gripped(this, e);
		}
	}

	public virtual void OnUngripped(ClickedEventArgs e)
	{
		if (this.Ungripped != null)
		{
			this.Ungripped(this, e);
		}
	}

	private void Update()
	{
		CVRSystem system = OpenVR.System;
		if (system != null && system.GetControllerState(controllerIndex, ref controllerState))
		{
			ulong num = controllerState.ulButtonPressed & 0x200000000uL;
			if (num != 0 && !triggerPressed)
			{
				triggerPressed = true;
				ClickedEventArgs e = default(ClickedEventArgs);
				e.controllerIndex = controllerIndex;
				e.flags = (uint)controllerState.ulButtonPressed;
				e.padX = controllerState.rAxis0.x;
				e.padY = controllerState.rAxis0.y;
				OnTriggerClicked(e);
			}
			else if (num == 0L && triggerPressed)
			{
				triggerPressed = false;
				ClickedEventArgs e2 = default(ClickedEventArgs);
				e2.controllerIndex = controllerIndex;
				e2.flags = (uint)controllerState.ulButtonPressed;
				e2.padX = controllerState.rAxis0.x;
				e2.padY = controllerState.rAxis0.y;
				OnTriggerUnclicked(e2);
			}
			ulong num2 = controllerState.ulButtonPressed & 4;
			if (num2 != 0 && !gripped)
			{
				gripped = true;
				ClickedEventArgs e3 = default(ClickedEventArgs);
				e3.controllerIndex = controllerIndex;
				e3.flags = (uint)controllerState.ulButtonPressed;
				e3.padX = controllerState.rAxis0.x;
				e3.padY = controllerState.rAxis0.y;
				OnGripped(e3);
			}
			else if (num2 == 0L && gripped)
			{
				gripped = false;
				ClickedEventArgs e4 = default(ClickedEventArgs);
				e4.controllerIndex = controllerIndex;
				e4.flags = (uint)controllerState.ulButtonPressed;
				e4.padX = controllerState.rAxis0.x;
				e4.padY = controllerState.rAxis0.y;
				OnUngripped(e4);
			}
			ulong num3 = controllerState.ulButtonPressed & 0x100000000uL;
			if (num3 != 0 && !padPressed)
			{
				padPressed = true;
				ClickedEventArgs e5 = default(ClickedEventArgs);
				e5.controllerIndex = controllerIndex;
				e5.flags = (uint)controllerState.ulButtonPressed;
				e5.padX = controllerState.rAxis0.x;
				e5.padY = controllerState.rAxis0.y;
				OnPadClicked(e5);
			}
			else if (num3 == 0L && padPressed)
			{
				padPressed = false;
				ClickedEventArgs e6 = default(ClickedEventArgs);
				e6.controllerIndex = controllerIndex;
				e6.flags = (uint)controllerState.ulButtonPressed;
				e6.padX = controllerState.rAxis0.x;
				e6.padY = controllerState.rAxis0.y;
				OnPadUnclicked(e6);
			}
			ulong num4 = controllerState.ulButtonPressed & 2;
			if (num4 != 0 && !menuPressed)
			{
				menuPressed = true;
				ClickedEventArgs e7 = default(ClickedEventArgs);
				e7.controllerIndex = controllerIndex;
				e7.flags = (uint)controllerState.ulButtonPressed;
				e7.padX = controllerState.rAxis0.x;
				e7.padY = controllerState.rAxis0.y;
				OnMenuClicked(e7);
			}
			else if (num4 == 0L && menuPressed)
			{
				menuPressed = false;
				ClickedEventArgs e8 = default(ClickedEventArgs);
				e8.controllerIndex = controllerIndex;
				e8.flags = (uint)controllerState.ulButtonPressed;
				e8.padX = controllerState.rAxis0.x;
				e8.padY = controllerState.rAxis0.y;
				OnMenuUnclicked(e8);
			}
			num3 = controllerState.ulButtonTouched & 0x100000000uL;
			if (num3 != 0 && !padTouched)
			{
				padTouched = true;
				ClickedEventArgs e9 = default(ClickedEventArgs);
				e9.controllerIndex = controllerIndex;
				e9.flags = (uint)controllerState.ulButtonPressed;
				e9.padX = controllerState.rAxis0.x;
				e9.padY = controllerState.rAxis0.y;
				OnPadTouched(e9);
			}
			else if (num3 == 0L && padTouched)
			{
				padTouched = false;
				ClickedEventArgs e10 = default(ClickedEventArgs);
				e10.controllerIndex = controllerIndex;
				e10.flags = (uint)controllerState.ulButtonPressed;
				e10.padX = controllerState.rAxis0.x;
				e10.padY = controllerState.rAxis0.y;
				OnPadUntouched(e10);
			}
		}
	}
}
