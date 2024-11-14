using System;
using System.Collections;
using OwlchemyVR;
using UnityEngine;
using UnityEngine.UI;

public class OfficeDesktopComputerProgram : ComputerProgram
{
	private const float SUCCESS_SCREEN_DURATION = 2f;

	[SerializeField]
	private GameObject loginScreen;

	[SerializeField]
	private WorldItemData loginWorldItemData;

	[SerializeField]
	private PhoneController phoneController;

	private bool rangPhone;

	[SerializeField]
	private GameObject successScreen;

	[SerializeField]
	private GameObject desktopScreen;

	[SerializeField]
	private Text passwordText;

	[SerializeField]
	private ComputerClickable loginButton;

	[SerializeField]
	private GameObject loginRegularText;

	[SerializeField]
	private Text loginTutorialText;

	[SerializeField]
	private GrabbableItem mouseGrabbableItem;

	[SerializeField]
	private Text successText;

	[SerializeField]
	private string correctPassword = "101";

	[SerializeField]
	private int maxPasswordLength = 6;

	[SerializeField]
	private string correctLoginMessage = "Welcome, user!";

	[SerializeField]
	private string goodEnoughLoginMessage = "Good enough!";

	[SerializeField]
	private AudioClip loginSound;

	private bool isHoldingMouse;

	private string password;

	private bool isLoggedIn;

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
		if (isLoggedIn)
		{
			loginScreen.SetActive(false);
			successScreen.SetActive(false);
			desktopScreen.SetActive(true);
			blockScreensaver = false;
			blockAlerts = false;
		}
		else
		{
			loginScreen.SetActive(true);
			successScreen.SetActive(false);
			desktopScreen.SetActive(false);
			blockScreensaver = true;
			blockAlerts = true;
			loginButton.SetInteractive(false);
			loginRegularText.SetActive(true);
			loginTutorialText.gameObject.SetActive(false);
			password = string.Empty;
			passwordText.text = string.Empty;
		}
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

	private void Update()
	{
	}

	private void MouseGrabbed(GrabbableItem item)
	{
		isHoldingMouse = true;
	}

	private void MouseReleased(GrabbableItem item)
	{
		isHoldingMouse = false;
		loginRegularText.SetActive(true);
		loginTutorialText.gameObject.SetActive(false);
	}

	protected override bool OnMouseMove(Vector2 cursorPos)
	{
		if (loginButton.ContainsCursor && isHoldingMouse)
		{
			loginRegularText.SetActive(false);
			loginTutorialText.gameObject.SetActive(true);
			loginTutorialText.text = "Press trigger\nto click!";
		}
		else
		{
			loginRegularText.SetActive(true);
			loginTutorialText.gameObject.SetActive(false);
		}
		return base.OnMouseMove(cursorPos);
	}

	protected override void OnClickableClicked(ComputerClickable clickable)
	{
		if (isLoggedIn)
		{
			if (clickable != null)
			{
				hostComputer.StartProgram((ComputerProgramID)(int)Enum.Parse(typeof(ComputerProgramID), clickable.name, true));
			}
		}
		else if (clickable != null && clickable == loginButton && password != string.Empty)
		{
			StartCoroutine(LoginAsync());
		}
	}

	protected override bool OnKeyPress(string code)
	{
		if (!isLoggedIn)
		{
			if (password.Length < maxPasswordLength)
			{
				password += code;
				passwordText.text += "*";
			}
			if (!rangPhone)
			{
				rangPhone = true;
				if (phoneController != null)
				{
					phoneController.ForceStartRinging();
				}
			}
			loginButton.SetInteractive(true);
		}
		return true;
	}

	public void ForceLogin()
	{
		StartCoroutine(LoginAsync(true));
	}

	private IEnumerator LoginAsync(bool isInstant = false)
	{
		isLoggedIn = true;
		hostComputer.PlaySound(loginSound);
		successText.text = ((!(password == correctPassword)) ? goodEnoughLoginMessage : correctLoginMessage);
		loginScreen.SetActive(false);
		successScreen.SetActive(true);
		GameEventsManager.Instance.ItemActionOccurred(loginWorldItemData, "ACTIVATED");
		if (!isInstant)
		{
			yield return new WaitForSeconds(2f);
		}
		successScreen.SetActive(false);
		desktopScreen.SetActive(true);
		blockScreensaver = false;
		blockAlerts = false;
	}
}
