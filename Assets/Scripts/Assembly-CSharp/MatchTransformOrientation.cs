using UnityEngine;

public class MatchTransformOrientation : MonoBehaviour
{
	[SerializeField]
	private Transform transformToMatch;

	private void LateUpdate()
	{
		base.transform.position = transformToMatch.position;
		base.transform.rotation = transformToMatch.rotation;
	}
}
