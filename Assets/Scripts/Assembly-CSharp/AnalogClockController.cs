using System;
using UnityEngine;

public class AnalogClockController : MonoBehaviour
{
	[SerializeField]
	[Range(1f, 12f)]
	private int startHour;

	[Range(1f, 60f)]
	[SerializeField]
	private int startMinute;

	[Range(1f, 60f)]
	[SerializeField]
	private int startSecond;

	[SerializeField]
	private bool startAtSystemTime;

	[SerializeField]
	private float timeScale = 1f;

	[SerializeField]
	private Transform hourHand;

	[SerializeField]
	private Transform minuteHand;

	[SerializeField]
	private Transform secondHand;

	[SerializeField]
	[Range(1f, 12f)]
	private int nightStartHour;

	[SerializeField]
	[Range(1f, 60f)]
	private int nightStartMinute;

	[SerializeField]
	[Range(1f, 60f)]
	private int nightStartSecond;

	private void Start()
	{
		if (startAtSystemTime)
		{
			DateTime now = DateTime.Now;
			startHour = now.Hour;
			startMinute = now.Minute;
			startSecond = now.Second;
		}
		if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.EndlessMode))
		{
			startHour = nightStartHour;
			startMinute = nightStartMinute;
			startSecond = nightStartSecond;
		}
		hourHand.localEulerAngles = Vector3.forward * (((float)startHour + (float)startMinute / 60f + (float)startSecond / 3600f) / 12f * 360f);
		minuteHand.localEulerAngles = Vector3.forward * (((float)startMinute + (float)startSecond / 60f) / 60f * 360f);
		secondHand.localEulerAngles = Vector3.forward * ((float)startSecond / 60f * 360f);
	}

	private void Update()
	{
		hourHand.Rotate(0f, 0f, 0.0016666667f * timeScale * Time.deltaTime);
		minuteHand.Rotate(0f, 0f, 0.1f * timeScale * Time.deltaTime);
		secondHand.Rotate(0f, 0f, 6f * timeScale * Time.deltaTime);
	}
}
