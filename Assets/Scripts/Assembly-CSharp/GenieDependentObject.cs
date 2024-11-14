using UnityEngine;

public class GenieDependentObject : MonoBehaviour
{
	[SerializeField]
	private bool checkOnAwake;

	[SerializeField]
	private bool checkNot;

	[SerializeField]
	private GenieDependentInfo[] settings;

	private void Awake()
	{
		CheckGenieSettings();
	}

	public void CheckGenieSettings()
	{
		if (checkNot)
		{
			CheckNegative();
		}
		else
		{
			CheckPositive();
		}
	}

	private void CheckNegative()
	{
		bool flag = true;
		bool active = true;
		for (int i = 0; i < settings.Length; i++)
		{
			if (GenieManager.AreAnyJobGenieModesActive())
			{
				if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, settings[i].GenieType))
				{
					flag = false;
					break;
				}
				active = settings[i].SetMyStateTo;
			}
			else
			{
				if (settings[i].GenieType == JobGenieCartridge.GenieModeTypes.None)
				{
					flag = false;
					break;
				}
				active = settings[i].SetMyStateTo;
			}
		}
		if (flag)
		{
			base.gameObject.SetActive(active);
		}
	}

	private void CheckPositive()
	{
		for (int i = 0; i < settings.Length; i++)
		{
			if (GenieManager.AreAnyJobGenieModesActive())
			{
				if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, settings[i].GenieType))
				{
					base.gameObject.SetActive(settings[i].SetMyStateTo);
				}
			}
			else if (settings[i].GenieType == JobGenieCartridge.GenieModeTypes.None)
			{
				base.gameObject.SetActive(settings[i].SetMyStateTo);
			}
		}
	}
}
