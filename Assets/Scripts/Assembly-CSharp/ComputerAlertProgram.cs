using System;
using UnityEngine;
using UnityEngine.UI;

public class ComputerAlertProgram : ComputerProgram
{
	private class AlertTask
	{
		public Sprite Graphic;

		public string Message;

		public string ButtonCaption;

		public Action ButtonAction;
	}

	[SerializeField]
	private Image graphicImage;

	[SerializeField]
	private Text messageText;

	[SerializeField]
	private Text okButtonText;

	[SerializeField]
	private ComputerClickable okButton;

	[SerializeField]
	private AudioClip alertSound;

	[SerializeField]
	private Animation alertAnimation;

	private AlertTask nextAlert;

	private AlertTask currentAlert;

	private bool wasCursorVisible;

	public override ComputerProgramID ProgramID
	{
		get
		{
			return ComputerProgramID.Alert;
		}
	}

	public bool HasAlertWaiting
	{
		get
		{
			return nextAlert != null;
		}
	}

	public bool IsShowingAlert
	{
		get
		{
			return currentAlert != null;
		}
	}

	private void OnEnable()
	{
		if (nextAlert != null)
		{
			ShowAlert();
		}
		else
		{
			Debug.LogError("Trying to show blank alert. This should never happen.");
		}
	}

	private void OnDisable()
	{
		if (currentAlert != null)
		{
			nextAlert = currentAlert;
		}
		HideAlert();
	}

	public void SetNextAlert(Sprite graphic, string message, string buttonCaption, Action buttonAction)
	{
		nextAlert = new AlertTask
		{
			Graphic = graphic,
			Message = message,
			ButtonCaption = buttonCaption,
			ButtonAction = buttonAction
		};
	}

	public void ClearNextAlert()
	{
		nextAlert = null;
	}

	private void ShowAlert()
	{
		graphicImage.sprite = nextAlert.Graphic;
		messageText.text = nextAlert.Message;
		okButtonText.text = nextAlert.ButtonCaption;
		wasCursorVisible = hostComputer.IsCursorVisible;
		hostComputer.ShowCursor();
		if (alertSound != null)
		{
			hostComputer.PlaySound(alertSound);
		}
		if (alertAnimation != null)
		{
			alertAnimation.Play();
		}
		currentAlert = nextAlert;
		nextAlert = null;
	}

	private void HideAlert()
	{
		if (currentAlert != null)
		{
			currentAlert = null;
			if (alertAnimation != null)
			{
				alertAnimation.Stop();
			}
			if (!wasCursorVisible)
			{
				hostComputer.HideCursor();
			}
		}
	}

	protected override void OnClickableClicked(ComputerClickable clickable)
	{
		if (clickable == okButton && currentAlert != null)
		{
			if (currentAlert.ButtonAction != null)
			{
				currentAlert.ButtonAction();
			}
			currentAlert = null;
			Finish();
		}
	}
}
