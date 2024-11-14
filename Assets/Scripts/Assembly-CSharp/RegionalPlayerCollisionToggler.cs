using UnityEngine;

public class RegionalPlayerCollisionToggler : MonoBehaviour
{
	[SerializeField]
	private GameObject[] objectsToToggle;

	[SerializeField]
	private PlayerPartDetector[] detectorRegions;

	private bool previouslyDetected;

	private void Update()
	{
		bool flag = false;
		for (int i = 0; i < detectorRegions.Length; i++)
		{
			if (detectorRegions[i].DetectedHands.Count > 0)
			{
				flag = true;
				break;
			}
		}
		if (flag != previouslyDetected)
		{
			for (int j = 0; j < objectsToToggle.Length; j++)
			{
				GameObject gameObject = objectsToToggle[j];
				if (flag)
				{
					if (gameObject.layer == 0)
					{
						gameObject.layer = 13;
					}
					else if (gameObject.layer == 8)
					{
						gameObject.layer = 9;
					}
				}
				else if (gameObject.layer == 13)
				{
					gameObject.layer = 0;
				}
				else if (gameObject.layer == 9)
				{
					gameObject.layer = 8;
				}
			}
		}
		previouslyDetected = flag;
	}
}
