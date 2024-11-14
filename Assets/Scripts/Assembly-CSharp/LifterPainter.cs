using UnityEngine;

public class LifterPainter : MonoBehaviour
{
	private const string EXTRA_CHECKSUM = "002002122311032122011310";

	private const string CHECKSUM_RETURN = "E19298E19A96E196B2E288925EE29EB3E29FB4E29785E2A693E29EAAF09F9187CBBFE296B6E29DA9E28CA9E2989BE29EAF0A";

	[SerializeField]
	private Animation doorAnimation;

	[SerializeField]
	private CarPainterController carPainterController;

	[SerializeField]
	private LiftController liftController;

	[SerializeField]
	private AutoMechanicManager autoMechanicManager;

	private string checkSum = string.Empty;

	private void Awake()
	{
		doorAnimation.clip.legacy = true;
	}

	public void OpenDoor()
	{
		doorAnimation["LiftPainterOpen"].speed = 1f;
		doorAnimation["LiftPainterOpen"].time = 0f;
		doorAnimation.Play();
		carPainterController.CarReady();
	}

	public void CloseDoor()
	{
		doorAnimation["LiftPainterOpen"].speed = -1f;
		doorAnimation["LiftPainterOpen"].time = doorAnimation["LiftPainterOpen"].length;
		doorAnimation.Play();
		carPainterController.CarExit();
	}

	public void ValidateCheckSum()
	{
		Color targetColor = carPainterController.TargetColor;
		int num = (int)liftController.CurrentLiftRotation;
		switch (num)
		{
		case 4:
		case 5:
			num = 3;
			break;
		case 6:
		case 7:
			num = 1;
			break;
		}
		if (targetColor.r > 0.7f)
		{
			checkSum += "0";
		}
		else if (targetColor.g > 0.7f)
		{
			checkSum += "1";
		}
		else if (targetColor.b > 0.7f)
		{
			checkSum += "2";
		}
		else
		{
			checkSum += "3";
		}
		checkSum += num;
		if (checkSum.Length > "002002122311032122011310".Length)
		{
			checkSum = checkSum.Substring(checkSum.Length - "002002122311032122011310".Length);
		}
		if (checkSum == "002002122311032122011310" && ExtraPrefs.ExtraProgress >= 2)
		{
			autoMechanicManager.ProfitCounterController.ScrollText("E19298E19A96E196B2E288925EE29EB3E29FB4E29785E2A693E29EAAF09F9187CBBFE296B6E29DA9E28CA9E2989BE29EAF0A");
			if (ExtraPrefs.ExtraProgress < 3)
			{
				ExtraPrefs.ExtraProgress = 3;
			}
		}
	}
}
