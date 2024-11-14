using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComputerProgram : MonoBehaviour
{
	[SerializeField]
	protected bool blockScreensaver;

	[SerializeField]
	protected bool blockAlerts;

	[SerializeField]
	protected bool restoreIfInterrupted;

	protected List<ComputerClickable> clickables = new List<ComputerClickable>();

	protected ComputerController hostComputer;

	protected ComputerProgramPriority priority;

	public abstract ComputerProgramID ProgramID { get; }

	public bool BlockScreensaver
	{
		get
		{
			return blockScreensaver;
		}
	}

	public bool BlockAlerts
	{
		get
		{
			return blockAlerts;
		}
	}

	public bool RestoreIfInterrupted
	{
		get
		{
			return restoreIfInterrupted;
		}
	}

	public ComputerController HostComputer
	{
		get
		{
			return hostComputer;
		}
	}

	public ComputerProgramPriority Priority
	{
		get
		{
			return priority;
		}
	}

	public ComputerClickable[] Clickables
	{
		get
		{
			return clickables.ToArray();
		}
	}

	public event Action<ComputerProgram> OnFinish;

	public bool InjectKeyPress(string code)
	{
		return OnKeyPress(code);
	}

	protected virtual bool OnKeyPress(string code)
	{
		return true;
	}

	public void SetHostComputer(ComputerController hostComputer)
	{
		this.hostComputer = hostComputer;
	}

	public void SetPriority(ComputerProgramPriority priority)
	{
		this.priority = priority;
	}

	public void AddClickable(ComputerClickable clickable)
	{
		if (!clickables.Contains(clickable))
		{
			clickables.Add(clickable);
			clickable.CursorEntered += OnClickableCursorEntered;
			clickable.CursorExited += OnClickableCursorExited;
			clickable.Clicked += OnClickableClicked;
			clickable.ClickedUp += OnClickableStopClick;
			clickable.Highlighted += OnClickableHighlighted;
			clickable.Unhighlighted += OnClickableUnhighlighted;
		}
	}

	public void RemoveClickable(ComputerClickable clickable)
	{
		if (clickables.Contains(clickable))
		{
			clickables.Remove(clickable);
			clickable.Clicked -= OnClickableClicked;
		}
	}

	public void InjectCursorPosition(Vector2 cursorPos)
	{
		for (int i = 0; i < clickables.Count; i++)
		{
			ComputerClickable computerClickable = clickables[i];
			if (computerClickable != null && computerClickable.isActiveAndEnabled && computerClickable.IsInteractive && !computerClickable.IsBusy)
			{
				computerClickable.InjectCursorPosition(cursorPos);
			}
		}
	}

	public bool InjectMouseMove(Vector2 cursorPos)
	{
		return OnMouseMove(cursorPos);
	}

	protected virtual bool OnMouseMove(Vector2 cursorPos)
	{
		return false;
	}

	public bool InjectMouseClick(Vector2 cursorPos)
	{
		return OnMouseClick(cursorPos);
	}

	public bool InjectMouseClickUp(Vector2 cursorPos)
	{
		return OnMouseClickUp(cursorPos);
	}

	protected virtual bool OnMouseClickUp(Vector2 cursorPos)
	{
		return true;
	}

	protected virtual bool OnMouseClick(Vector2 cursorPos)
	{
		return true;
	}

	protected virtual void OnClickableCursorEntered(ComputerClickable clickable)
	{
	}

	protected virtual void OnClickableCursorExited(ComputerClickable clickable)
	{
	}

	protected virtual void OnClickableHighlighted(ComputerClickable clickable)
	{
	}

	protected virtual void OnClickableUnhighlighted(ComputerClickable clickable)
	{
	}

	protected virtual void OnClickableClicked(ComputerClickable clickable)
	{
	}

	protected virtual void OnClickableStopClick(ComputerClickable clickable)
	{
	}

	public void Finish()
	{
		if (this.OnFinish != null)
		{
			this.OnFinish(this);
		}
	}
}
