using OwlchemyVR;
using UnityEngine;

public class PlatformBasedToggle : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Sets Objects On/Off depending on the Platform")]
	private GameObject[] highPerformanceObjects;

	[SerializeField]
	[Tooltip("Sets Objects On/Off depending on the Platform")]
	private GameObject[] lowPerformanceObjects;

	private void Awake()
	{
		SetObjectPerformance(VRPlatform.IsLowPerformancePlatform);
	}

	public void SetObjectPerformance(bool isLow)
	{
		if (highPerformanceObjects.Length > 0)
		{
			for (int i = 0; i < highPerformanceObjects.Length; i++)
			{
				highPerformanceObjects[i].SetActive(!isLow);
			}
		}
		if (lowPerformanceObjects.Length > 0)
		{
			for (int j = 0; j < lowPerformanceObjects.Length; j++)
			{
				lowPerformanceObjects[j].SetActive(isLow);
			}
		}
	}
}
