using System;
using UnityEngine;

public class VersionInfoStorage : MonoBehaviour
{
	private const int STARTING_YEAR = 2016;

	[SerializeField]
	private string buildNumber = string.Empty;

	public string BuildNumber
	{
		get
		{
			return buildNumber;
		}
	}

	public static VersionInfoStorage LoadVersionInfoStorageFromResources()
	{
		return Resources.Load<VersionInfoStorage>("VersionInfoStorage");
	}

	public void SetBuildNumberAsCurrent()
	{
		buildNumber = GetCurrentBuildNumber();
		Debug.Log("Set Build Number:" + buildNumber);
	}

	public string GetBuildNumber()
	{
		return buildNumber;
	}

	public void ClearBuildNumber()
	{
		buildNumber = string.Empty;
	}

	private string GetCurrentBuildNumber()
	{
		return ConvertDateTimeToBuildNumber(DateTime.UtcNow);
	}

	private string ConvertDateTimeToBuildNumber(DateTime dateTime)
	{
		int num = dateTime.Year - 2016;
		int num2 = num * 12 + dateTime.Month;
		int day = dateTime.Day;
		return num2 + string.Format("{0:00}", day);
	}
}
