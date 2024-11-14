using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPositionChanger : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Other positions that this object will be set to")]
	private List<Transform> positions;

	[SerializeField]
	private float minChangeTime = 1f;

	[SerializeField]
	private float maxChangeTime = 3f;

	[Tooltip("Go back to the initial position between other random positions")]
	[SerializeField]
	private bool bounceToInitialPosition;

	private Vector3 initialPosition;

	[SerializeField]
	[Tooltip("How much longer to stay at the initial position than other positions")]
	private float initialPositionTimeMultiplier = 1f;

	private void Start()
	{
		initialPosition = base.transform.position;
		if (positions.Count > 0)
		{
			StartCoroutine(ChangePositionAfterRandomIntervals());
		}
	}

	private IEnumerator ChangePositionAfterRandomIntervals()
	{
		while (true)
		{
			float waitTime2 = Random.Range(minChangeTime, maxChangeTime);
			waitTime2 = ((!bounceToInitialPosition) ? waitTime2 : (waitTime2 * initialPositionTimeMultiplier));
			yield return new WaitForSeconds(waitTime2);
			int randomIndex = Random.Range(0, positions.Count);
			base.gameObject.transform.position = positions[randomIndex].position;
			if (bounceToInitialPosition)
			{
				yield return new WaitForSeconds(Random.Range(minChangeTime, maxChangeTime));
				base.gameObject.transform.position = initialPosition;
			}
		}
	}
}
