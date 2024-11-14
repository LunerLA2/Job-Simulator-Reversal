using UnityEngine;

public class WeightScale : MonoBehaviour
{
	[SerializeField]
	private Transform scaleTray;

	[SerializeField]
	private Transform scaleHand;

	[SerializeField]
	private float minDownTrayDistance;

	[SerializeField]
	private float maxDownTrayDistance;

	private void Update()
	{
		float num = scaleTray.transform.localPosition.y - minDownTrayDistance;
		if (num > 0f)
		{
			num = 0f;
		}
		float num2 = num / (maxDownTrayDistance - minDownTrayDistance);
		scaleHand.transform.localEulerAngles = new Vector3(0f, 0f, 360f * num2);
	}
}
